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

        [Header("Configuration")]
        [SerializeField]
        [Tooltip("The endpoint URL for the Hardal service")]
        private string endpoint = "https://your-default-endpoint.com";

        [Header("Debug Settings")]
        [SerializeField] private bool enableDebugLogs = false;

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
                Log("Hardal is already initialized", LogType.Warning);
                return;
            }

            if (!string.IsNullOrEmpty(customEndpoint))
            {
                endpoint = customEndpoint;
            }

            if (string.IsNullOrEmpty(endpoint))
            {
                Log("No endpoint provided. Please set the endpoint in the inspector or provide it programmatically.", LogType.Error);
                return;
            }

            var config = new Hardal.HardalConfig
            {
                endpoint = endpoint
            };

            Hardal.Instance.Init(config);
            _isInitialized = true;
            
            Log("Hardal initialized successfully");
        }

        public async Task TrackEvent(string eventName, Dictionary<string, object> properties = null)
        {
            if (!_isInitialized)
            {
                Log("Hardal is not initialized. Please call InitializeHardal first.", LogType.Error);
                return;
            }

            Log($"Tracking event: {eventName}\nProperties: {FormatProperties(properties)}");

            try 
            {
                await Hardal.Instance.TrackEvent(eventName, properties);
                Log($"Successfully sent event: {eventName}");
            }
            catch (Exception e)
            {
                Log($"Failed to send event {eventName}: {e.Message}", LogType.Error);
            }
        }

        // Convenience method for tracking events without async/await
        public void TrackEventNonAsync(string eventName, Dictionary<string, object> properties = null)
        {
            if (!_isInitialized)
            {
                Log("Hardal is not initialized. Please call InitializeHardal first.", LogType.Error);
                return;
            }

            Log($"Tracking event (non-async): {eventName}\nProperties: {FormatProperties(properties)}");

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

        private void Log(string message, LogType type = LogType.Log)
        {
            if (!enableDebugLogs) return;
            
            switch (type)
            {
                case LogType.Log:
                    Debug.Log($"[Hardal] {message}");
                    break;
                case LogType.Warning:
                    Debug.LogWarning($"[Hardal] {message}");
                    break;
                case LogType.Error:
                    Debug.LogError($"[Hardal] {message}");
                    break;
            }
        }

#if UNITY_EDITOR
        // This will show a button in the Inspector
        [UnityEditor.CustomEditor(typeof(HardalManager))]
        public class HardalManagerEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                HardalManager manager = (HardalManager)target;
                
                UnityEditor.EditorGUILayout.Space();
                UnityEditor.EditorGUILayout.HelpBox(
                    "This component should exist only once in your project. It will persist between scenes.",
                    UnityEditor.MessageType.Info);
            }
        }
#endif
    }
}