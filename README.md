# Hardal SDK Documentation

## Table of Contents
1. Installation
2. Setup
3. Basic Usage
4. Debug Mode
5. Example Usage

## 1. Installation
The Hardal SDK can be installed via Unity Package Manager:
- Add the package to your project via Package Manager
- Or add the following line to your `manifest.json`:
  ```json
  {
    "dependencies": {
      "com.hardal.signal": "1.0.0"
    }
  }
  ```

## 2. Setup

### Option A: Using the Menu Item
1. In Unity Editor, go to `GameObject > Hardal > Create Hardal Manager`
2. This will create a new GameObject with HardalManager component
3. In the Inspector, set your endpoint URL
4. Optionally enable debug logs

### Option B: Manual Setup
1. Create an empty GameObject in your scene
2. Add the HardalManager component to it
3. Configure the endpoint URL in the Inspector
4. The GameObject will persist between scenes

## 3. Basic Usage

### Initialize Hardal
```cs
// The SDK automatically initializes on Start()
// But you can manually initialize it with a custom endpoint:
HardalManager.Instance.InitializeHardal("https://your-custom-endpoint.com");
```

### Track Events

```cs
// Track a simple event
await HardalManager.Instance.TrackEvent("game_started");
// Track event with properties
Dictionary<string, object> properties = new Dictionary<string, object> {
  { "level", 5 }, { "score", 1000 }, { "character", "warrior" }
};
await HardalManager.Instance.TrackEvent("level_completed", properties);
// Non-async version
HardalManager.Instance.TrackEventNonAsync("item_purchased", properties);
```

# 4. Debug Mode
Enable debug logging in the Inspector to see detailed information about:
- Initialization status
- Event tracking
- Network requests
- Errors and warnings

To enable debug mode:
1. Select the HardalManager GameObject
2. Check "Enable Debug Logs" in the Inspector
3. View logs in the Unity Console with "[Hardal]" prefix

## 5. Example Usage

### Basic Implementation
```cs
public class GameManager : MonoBehaviour {
  private void Start() {
    // Track game start
    HardalManager.Instance.TrackEventNonAsync("game_started");
  }
  public void OnLevelComplete(int level, int score) {
    var properties = new Dictionary<string, object> {
      { "level", level }, { "score", score }, { "time_spent", Time.time }
    };
    HardalManager.Instance.TrackEventNonAsync("level_completed", properties);
  }
}
```
### Advanced Implementation

```cs
public class AnalyticsManager : MonoBehaviour {
  private async void TrackPurchase(string itemId, float price,
                                   string currency) {
    var properties = new Dictionary<string, object> {
      { "item_id", itemId },
      { "price", price },
      { "currency", currency },
      { "platform", Application.platform.ToString() },
      { "timestamp", DateTime.UtcNow }
    };
    try {
      await HardalManager.Instance.TrackEvent("purchase_completed", properties);
    } catch (Exception e) {
      Debug.LogError($"Failed to track purchase: {e.Message}");
    }
  }
}
```

## Important Notes
- Only one instance of HardalManager should exist in your project
- The manager persists between scenes using DontDestroyOnLoad
- Always check if the SDK is initialized before tracking events
- Use try-catch blocks when working with async methods
- Enable debug logs during development for better visibility

## Best Practices
1. Initialize the SDK early in your game lifecycle
2. Use meaningful event names
3. Be consistent with property names
4. Handle async operations properly
5. Test with debug mode enabled during development