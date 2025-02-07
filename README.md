## Installation

### Via Unity Package Manager (Recommended)
1. Open the Package Manager (Window > Package Manager)
2. Click the "+" button > "Add package from git URL..."
3. Enter: `https://github.com/yourcompany/hardal.git`

### Manual Installation
1. Download the latest release
2. Extract the .unitypackage file
3. Import all assets into your project


## Getting Started

1. Add HardalManager to your scene:

Drag the "HardalManager" prefab from Packages/Hardal/Runtime/Prefabs to your scene  

2. Initialize HardalManager:

// Option 1: Using prefab
Drag the "HardalManager" prefab from Packages/Hardal/Runtime/Prefabs to your scene

// Option 2: Runtime initialization
HardalManager.Instance.InitializeHardal("your-endpoint-url");

3. Track events:

// Track simple event
HardalManager.Instance.TrackEventNonAsync("level_start");

// Track event with properties
HardalManager.Instance.TrackEventNonAsync("player_death", new Dictionary<string, object>
{
["level"] = currentLevel,
["cause"] = "enemy_collision"
});


## Configuration


## Additional Features

1. Custom endpoint:

HardalManager.Instance.InitializeHardal("your-custom-endpoint-url");


2. Event tracking:

// Track simple event
HardalManager.Instance.TrackEventNonAsync("level_start");

// Track event with properties

// Track event with properties
HardalManager.Instance.TrackEventNonAsync("player_death", new Dictionary<string, object>
{
["level"] = currentLevel,
["cause"] = "enemy_collision"
});

3. Event tracking:

// Track simple event
HardalManager.Instance.TrackEventNonAsync("level_start");

// Track event with properties
HardalManager.Instance.TrackEventNonAsync("player_death", new Dictionary<string, object>
{
["level"] = currentLevel,
["cause"] = "enemy_collision"
});     


