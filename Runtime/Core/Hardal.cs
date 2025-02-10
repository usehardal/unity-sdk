using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
namespace Hardal.Signal
{
public class Hardal : MonoBehaviour
{
    private static Hardal _instance;
    private string _endpoint;
    private HardalConfig _config;
    private Queue<EventQueueItem> _eventQueue = new Queue<EventQueueItem>();
    private bool _isProcessingQueue;
        
    [Serializable]
    public class HardalConfig
    {
        public string endpoint;
    }


    private class EventQueueItem
    {
        public string eventName;
        public Dictionary<string, object> properties;
        public TaskCompletionSource<bool> completionSource;
    }

    public static Hardal Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("Hardal");
                _instance = go.AddComponent<Hardal>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public void Init(HardalConfig config)
    {
        _config = config;
        _endpoint = config.endpoint;

        if (string.IsNullOrEmpty(_endpoint))
        {
            Debug.LogError("[Hardal] No endpoint provided in configuration");
            return;
        }
    }

    private async Task<string> GenerateServerDistinctId()
    {
        try
        {
            string deviceInfo = $"{SystemInfo.deviceModel}|{SystemInfo.deviceType}|{SystemInfo.operatingSystem}|{SystemInfo.processorType}|{SystemInfo.graphicsDeviceName}";
            byte[] bytes = Encoding.UTF8.GetBytes(deviceInfo);
            
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(bytes);
                string clientHash = BitConverter.ToString(hash).Replace("-", "").ToLower();
                return $"hr_tmp_{clientHash.Substring(0, 32)}";
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Hardal] Error generating client hash: {ex}");
            return $"hr_{DateTime.Now.Ticks}_{UnityEngine.Random.Range(0, 999999)}";
        }
    }

    private async Task<Dictionary<string, object>> GetBaseEventData()
    {
        string serverDistinctId = await GenerateServerDistinctId();

        return new Dictionary<string, object>
        {
            ["distinct"] = new Dictionary<string, object>
            {
                ["server_distinct_id"] = serverDistinctId
            },
            ["device"] = new Dictionary<string, object>
            {
                ["model"] = SystemInfo.deviceModel,
                ["type"] = SystemInfo.deviceType.ToString(),
                ["operating_system"] = SystemInfo.operatingSystem,
                ["processor"] = SystemInfo.processorType,
                ["graphics_device"] = SystemInfo.graphicsDeviceName,
                ["memory"] = SystemInfo.systemMemorySize
            },
            ["screen"] = new Dictionary<string, object>
            {
                ["resolution"] = $"{Screen.width}x{Screen.height}",
                ["dpi"] = Screen.dpi
            },
            ["app"] = new Dictionary<string, object>
            {
                ["version"] = Application.version,
                ["platform"] = Application.platform.ToString(),
                ["unity_version"] = Application.unityVersion
            },
            ["timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    public async Task TrackEvent(string eventName, Dictionary<string, object> properties = null)
    {
        var tcs = new TaskCompletionSource<bool>();
        _eventQueue.Enqueue(new EventQueueItem
        {
            eventName = eventName,
            properties = properties,
            completionSource = tcs
        });

        if (!_isProcessingQueue)
        {
            _ = ProcessEventQueue();
        }

        await tcs.Task;
    }

    private async Task ProcessEventQueue()
    {
        _isProcessingQueue = true;

        while (_eventQueue.Count > 0)
        {
            var item = _eventQueue.Dequeue();
            try
            {
                var baseEventData = await GetBaseEventData();
                
                // Create the event data structure without the "data" wrapper
                var eventData = new Dictionary<string, object>
                {
                    ["event_name"] = item.eventName,
                    ["properties"] = new Dictionary<string, object>(baseEventData)
                };

                // Add custom properties if they exist
                if (item.properties != null)
                {
                    foreach (var prop in item.properties)
                    {
                        ((Dictionary<string, object>)eventData["properties"])[prop.Key] = prop.Value;
                    }
                }

                // Convert to JSON directly without the "data" wrapper
                string json = SerializeDictionary(eventData);
                
                Debug.Log($"[Hardal] Sending event data: {json}"); // Log the actual JSON being sent

                string endpointUrl = $"{_endpoint}/push/hardal";

                using (UnityWebRequest request = new UnityWebRequest(endpointUrl, "POST"))
                {
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.SetRequestHeader("Content-Type", "application/json");

                    await request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        Debug.Log($"[Hardal] Successfully sent event {item.eventName}");
                        item.completionSource.SetResult(true);
                    }
                    else
                    {
                        string errorMessage = $"Request failed: {request.error}\nResponse: {request.downloadHandler.text}";
                        Debug.LogError($"[Hardal] {errorMessage}");
                        throw new Exception(errorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Hardal] Failed to send event {item.eventName}: {ex}");
                item.completionSource.SetResult(false);
            }
        }

        _isProcessingQueue = false;
    }

    private string SerializeDictionary(Dictionary<string, object> dict)
    {
        if (dict == null) return "null";
        var entries = dict.Select(kvp => $"\"{kvp.Key}\":{SerializeValue(kvp.Value)}");
        return "{" + string.Join(",", entries) + "}";
    }

    private string SerializeValue(object value)
    {
        if (value == null) return "null";
        if (value is string) return $"\"{value}\"";
        if (value is bool) return value.ToString().ToLower();
        if (value is int || value is float || value is double) return value.ToString();
        if (value is Dictionary<string, object> dict) return SerializeDictionary(dict);
        if (value is IEnumerable<object> list) return "[" + string.Join(",", list.Select(SerializeValue)) + "]";
        return $"\"{value}\"";
    }
}
}