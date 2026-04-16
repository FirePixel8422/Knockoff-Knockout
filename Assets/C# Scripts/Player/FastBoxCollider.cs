using Unity.Mathematics;
using UnityEngine;



public class FastBoxCollider : MonoBehaviour
{
    [SerializeField] private ColliderType colliderType;
    [SerializeField] private float3 size = new float3(1);

    public ColliderType Type => colliderType;
    public AABB GetAABB()
    {
        float3 half = size * 0.5f;
        float3 center = transform.position;

        return new AABB
        {
            Min = center - half,
            Max = center + half
        };
    }


#if UNITY_EDITOR
    static readonly Color INVALID_COLOR = new Color(0.6f, 0.2f, 1);
    static readonly Color HITBOX_COLOR = new Color(0.1f, 0.1f, 1);
    static readonly Color HURTBOX_COLOR = new Color(1, 0.05f, 0.05f);

    private void OnDrawGizmos()
    {
        Gizmos.color = colliderType switch
        {
            ColliderType.Hitbox => HITBOX_COLOR,
            ColliderType.Hurtbox => HURTBOX_COLOR,
            ColliderType.None or _ => INVALID_COLOR,
        };
        Gizmos.DrawWireMesh(GlobalMeshes.Cube, transform.position, Quaternion.identity, size);
    }
#endif
}

[System.Serializable]
public struct AABB
{
    public float3 Min;
    public float3 Max;
}