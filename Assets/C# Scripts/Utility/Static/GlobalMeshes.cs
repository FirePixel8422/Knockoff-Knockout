using UnityEngine;

/// <summary>
/// Static class that creates a static mesh Instance for a cube and sphere
/// </summary>
public static class GlobalMeshes
{
    public static Mesh Cube { get; private set; }
    public static Mesh Sphere { get; private set; }


    static GlobalMeshes()
    {
        Cube = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
        Sphere = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
    }
}