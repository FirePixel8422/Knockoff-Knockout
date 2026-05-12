using Unity.Mathematics;
using UnityEngine;


public class FastBoxCollider : MonoBehaviour
{
    [SerializeField] private int id;

    [SerializeField] private ColliderType colliderType;
    [SerializeField] private float3 halfSize = new float3(1);
    [SerializeField] private float3 offset = new float3(1);
    
    public int Id => id;
    public ColliderType Type => colliderType;

    /// <returns>Lightweight converted collider as AABB with IsActive state</returns>
    public HitBoxAABB GetAABB()
    {
        float3 center = (float3)transform.position + offset;

        return new HitBoxAABB(isActiveAndEnabled, center - halfSize, center + halfSize);
    }


#if UNITY_EDITOR
    public float3 Size => halfSize * 2;

    private static readonly Color INVALID_COLOR = new Color(0.6f, 0.2f, 1);
    private static readonly Color HITBOX_COLOR = new Color(0.1f, 0.1f, 1);
    private static readonly Color HURTBOX_COLOR = new Color(1, 0.05f, 0.05f);

    private void OnDrawGizmos()
    {
        Gizmos.color = colliderType switch
        {
            ColliderType.Hitbox => HITBOX_COLOR,
            ColliderType.Hurtbox => HURTBOX_COLOR,
            ColliderType.None or _ => INVALID_COLOR,
        };
        Gizmos.DrawWireMesh(GlobalMeshes.Cube, transform.position, Quaternion.identity, Size);
    }
#endif
}

[System.Serializable]
public struct HitBoxAABB
{
    private readonly byte activeFlag;
    public readonly bool IsActive => activeFlag == 1;

    public float3 Min;
    public float3 Max;

    public HitBoxAABB(bool active, float3 min, float3 max)
    {
        activeFlag = (byte)(active ? 1 : 0);
        Min = min;
        Max = max;
    }
}