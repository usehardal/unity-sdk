<p align="center">
  <a href="https://usehardal.com/">
    <picture>
      <source media="(prefers-color-scheme: dark)" srcset="https://imge.usehardal.com/cdn/logo/new/svg/hnav0hronsb1fttdnmph.svg?raw=1">
      <source media="(prefers-color-scheme: light)" srcset="https://imge.usehardal.com/cdn/logo/new/svg/ptlior6hfvknmblekenh.svg?raw=1">
      <img src="https://imge.usehardal.com/cdn/logo/new/svg/ptlior6hfvknmblekenh.svg?raw=1" alt="Hardal" width="180">
    </picture>
  </a>
</p>

# Hardal Signal SDK for Unity

Track gameplay events such as game starts, level completions, and purchases, and send them to a configured Hardal Signal endpoint. The SDK provides a persistent `HardalManager` component, asynchronous event tracking, and optional debug logging.

## Getting started

You need Unity 2020.3 or later and a Hardal Signal endpoint. Install the package, add `HardalManager` to a scene, and configure the endpoint before sending events.

## Contents

- [Installation](#installation)
- [Setup](#setup)
- [Usage](#usage)
- [Debug mode](#debug-mode)
- [Examples](#examples)
- [Important notes](#important-notes)
- [Best practices](#best-practices)
- [Support](#support)

## Installation

Clone this repository and, in Unity Package Manager, select **Add package from disk** and choose its [package.json](package.json).

If your configured Unity package registry hosts `com.hardal.signal` version `1.0.0`, you can also add this entry to `Packages/manifest.json`:
  ```json
  {
    "dependencies": {
      "com.hardal.signal": "1.0.0"
    }
  }
  ```

## Setup

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

## Usage

### Initialize Hardal
```csharp
// The SDK automatically initializes on Start()
// But you can manually initialize it with a custom endpoint:
HardalManager.Instance.InitializeHardal("https://your-custom-endpoint.com");
```

### Track Events

```csharp
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

## Debug mode
Enable debug logging in the Inspector to see detailed information about:
- Initialization status
- Event tracking
- Network requests
- Errors and warnings

To enable debug mode:
1. Select the HardalManager GameObject
2. Check "Enable Debug Logs" in the Inspector
3. View logs in the Unity Console with "[Hardal]" prefix

## Examples

### Basic implementation
```csharp
using UnityEngine;
using System.Collections.Generic;
using Hardal.Signal;

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
### Advanced implementation

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;
using Hardal.Signal;

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

## Important notes
- Only one instance of HardalManager should exist in your project
- The manager persists between scenes using DontDestroyOnLoad
- Always check if the SDK is initialized before tracking events
- Use try-catch blocks when working with async methods
- Enable debug logs during development for better visibility

## Best practices
1. Initialize the SDK early in your game lifecycle
2. Use meaningful event names
3. Be consistent with property names
4. Handle async operations properly
5. Test with debug mode enabled during development
## Support

Maintained by [Hardal](https://github.com/usehardal).

- [Hardal documentation](https://docs.usehardal.com)
- [Report an issue](https://github.com/usehardal/unity-sdk/issues)
- [Hardal website](https://usehardal.com)

