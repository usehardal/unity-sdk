using UnityEngine;
using UnityEditor;

namespace Hardal.Signal.Editor
{
    public static class HardalManagerCreator
    {
        private const string MenuPath = "GameObject/Hardal/Create Hardal Manager";
        private const string AssetMenuPath = "Assets/Create/Hardal/Hardal Manager";
        private const string GameObjectName = "HardalManager";

        [MenuItem(MenuPath)]
        [MenuItem(AssetMenuPath)]
        public static void CreateHardalManager()
        {
            // Check if instance already exists
            var existingManager = Object.FindObjectOfType<HardalManager>();
            if (existingManager != null)
            {
                Debug.LogWarning("[Hardal] HardalManager already exists in the scene!");
                Selection.activeGameObject = existingManager.gameObject;
                return;
            }

            // Create the GameObject and add HardalManager component
            GameObject go = new GameObject(GameObjectName);
            go.AddComponent<HardalManager>();
            
            // Register the creation for undo
            Undo.RegisterCreatedObjectUndo(go, "Create Hardal Manager");
            
            // Select the newly created object
            Selection.activeGameObject = go;
            
            // Position the object at a sensible location in the hierarchy
            SceneView.lastActiveSceneView?.Frame(new Bounds(go.transform.position, Vector3.one), false);
        }

        // Validate the menu items
        [MenuItem(MenuPath, true)]
        [MenuItem(AssetMenuPath, true)]
        private static bool ValidateCreateHardalManager()
        {
            return Object.FindObjectOfType<HardalManager>() == null;
        }
    }
} 