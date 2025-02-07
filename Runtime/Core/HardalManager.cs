using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
namespace Hardal.Signal
{
public class HardalManager : MonoBehaviour
{
    private static HardalManager _instance;
    public static HardalManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("HardalManager");
                _instance = go.AddComponent<HardalManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    [SerializeField]
    private string endpoint;

    [SerializeField]
    private bool autoPageview = true;

    [SerializeField]
    private bool fetchFromGA4 = false;

    [SerializeField]
    private bool fetchFromFBPixel = false;

    [SerializeField]
    private bool fetchFromRTB = false;

    private bool _isInitialized = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (!_isInitialized)
        {
            InitializeHardal();
        }
    }

    public void InitializeHardal(string customEndpoint = null)
    {
        if (_isInitialized)
        {
            Debug.LogWarning("[HardalManager] Hardal is already initialized");
            return;
        }

        if (!string.IsNullOrEmpty(customEndpoint))
        {
            endpoint = customEndpoint;
        }

        if (string.IsNullOrEmpty(endpoint))
        {
            Debug.LogError("[HardalManager] No endpoint provided. Please set the endpoint in the inspector or provide it programmatically.");
            return;
        }

        var config = new Hardal.HardalConfig
        {
            endpoint = endpoint,
            options = new Hardal.HardalOptions
            {
                autoPageview = autoPageview,
                fetchFromGA4 = fetchFromGA4,
                fetchFromFBPixel = fetchFromFBPixel,
                fetchFromRTB = fetchFromRTB
            }
        };

        Hardal.Instance.Init(config);
        _isInitialized = true;
        
        Debug.Log("[HardalManager] Hardal initialized successfully");
    }

    public async Task TrackEvent(string eventName, Dictionary<string, object> properties = null)
    {
        if (!_isInitialized)
        {
            Debug.LogError("[HardalManager] Hardal is not initialized. Please call InitializeHardal first.");
            return;
        }

        Debug.Log($"[HardalManager] Tracking event: {eventName}\nProperties: {FormatProperties(properties)}");

        try 
        {
            await Hardal.Instance.TrackEvent(eventName, properties);
            Debug.Log($"[HardalManager] Successfully sent event: {eventName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[HardalManager] Failed to send event {eventName}: {e.Message}");
        }
    }

    // Convenience method for tracking events without async/await
    public void TrackEventNonAsync(string eventName, Dictionary<string, object> properties = null)
    {
        if (!_isInitialized)
        {
            Debug.LogError("[HardalManager] Hardal is not initialized. Please call InitializeHardal first.");
            return;
        }

        Debug.Log($"[HardalManager] Tracking event (non-async): {eventName}\nProperties: {FormatProperties(properties)}");

        #pragma warning disable CS4014
        TrackEvent(eventName, properties);
        #pragma warning restore CS4014
    }

    private string FormatProperties(Dictionary<string, object> properties)
    {
        if (properties == null || properties.Count == 0)
        {
            return "none";
        }

        return string.Join("\n", properties.Select(kvp => $"  {kvp.Key}: {FormatValue(kvp.Value)}"));
    }

    private string FormatValue(object value)
    {
        if (value == null)
            return "null";
        
        if (value is Dictionary<string, object> dict)
            return "{\n" + string.Join(",\n", dict.Select(kvp => $"    {kvp.Key}: {FormatValue(kvp.Value)}")) + "\n  }";
        
        if (value is IEnumerable<object> list)
            return "[" + string.Join(", ", list.Select(FormatValue)) + "]";
        
        return value.ToString();
    }

    public bool IsInitialized()
    {
        return _isInitialized;
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
} 
}