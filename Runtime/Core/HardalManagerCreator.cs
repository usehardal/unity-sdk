using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
namespace Hardal.Signal
{
    public static class HardalManagerCreator
    {
        private const string MenuPath = "GameObject/Hardal/Create Hardal Manager";
        private const string GameObjectName = "HardalManager";

        [MenuItem(MenuPath)]
        public static void CreateHardalManager()
        {
            // Check if instance already exists
            if (HardalManager.Instance != null)
            {
                Debug.LogWarning("[Hardal] HardalManager already exists in the scene!");
                Selection.activeGameObject = HardalManager.Instance.gameObject;
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
    }
}
#endif 