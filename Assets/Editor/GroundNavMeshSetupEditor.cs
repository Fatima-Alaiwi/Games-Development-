using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public static class GroundNavMeshSetupEditor
{
    private const string GroundLayerName = "Ground";
    private const string SurfaceName = "Ground NavMesh Surface";

    [MenuItem("Tools/Future Level NavMesh/Setup Ground NavMesh")]
    public static void SetupGroundNavMesh()
    {
        int groundLayer = LayerMask.NameToLayer(GroundLayerName);
        if (groundLayer < 0)
        {
            EditorUtility.DisplayDialog(
                "Ground Layer Missing",
                "No layer named 'Ground' exists. Create the Ground layer first, then assign your street objects to it.",
                "OK");
            return;
        }

        int modifiedCount = MarkGroundObjectsWalkable(groundLayer);
        NavMeshSurface surface = GetOrCreateGroundSurface(groundLayer);

        EditorUtility.SetDirty(surface);
        EditorUtility.DisplayDialog(
            "Ground NavMesh Setup Complete",
            $"Marked {modifiedCount} Ground-layer 3D objects and child meshes as Walkable.\n\nSelect '{SurfaceName}' and click Bake on its NavMeshSurface component.",
            "OK");
    }

    private static int MarkGroundObjectsWalkable(int groundLayer)
    {
        int modifiedCount = 0;
        GameObject[] objects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in objects)
        {
            if (!IsGroundObjectOrChildOfGroundObject(obj, groundLayer) || !Is3DSceneObject(obj))
            {
                continue;
            }

            if (obj.layer != groundLayer)
            {
                Undo.RecordObject(obj, "Set Child Object To Ground Layer");
                obj.layer = groundLayer;
                EditorUtility.SetDirty(obj);
            }

            NavMeshModifier modifier = obj.GetComponent<NavMeshModifier>();
            if (modifier == null)
            {
                modifier = Undo.AddComponent<NavMeshModifier>(obj);
            }

            Undo.RecordObject(modifier, "Mark Ground Object Walkable");
            modifier.overrideArea = true;
            modifier.area = NavMesh.GetAreaFromName("Walkable");
            EditorUtility.SetDirty(modifier);

            modifiedCount++;
        }

        return modifiedCount;
    }

    private static bool Is3DSceneObject(GameObject obj)
    {
        return obj.scene.IsValid()
            && (obj.GetComponent<MeshRenderer>() != null
                || obj.GetComponent<MeshFilter>() != null
                || obj.GetComponent<Collider>() != null);
    }

    private static bool IsGroundObjectOrChildOfGroundObject(GameObject obj, int groundLayer)
    {
        Transform current = obj.transform;

        while (current != null)
        {
            if (current.gameObject.layer == groundLayer)
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }

    private static NavMeshSurface GetOrCreateGroundSurface(int groundLayer)
    {
        NavMeshSurface surface = Object.FindFirstObjectByType<NavMeshSurface>();

        if (surface == null)
        {
            GameObject surfaceObject = new GameObject(SurfaceName);
            Undo.RegisterCreatedObjectUndo(surfaceObject, "Create Ground NavMesh Surface");
            surface = surfaceObject.AddComponent<NavMeshSurface>();
        }
        else
        {
            Undo.RecordObject(surface, "Configure Ground NavMesh Surface");
            surface.gameObject.name = SurfaceName;
        }

        surface.collectObjects = CollectObjects.All;
        surface.useGeometry = NavMeshCollectGeometry.RenderMeshes;
        surface.layerMask = 1 << groundLayer;

        return surface;
    }
}
