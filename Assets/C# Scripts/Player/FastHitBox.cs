using Unity.Mathematics;
using UnityEngine;


public class FastHitBox : MonoBehaviour
{
    [SerializeField] protected float3 halfSize = new float3(1);
    
    public virtual ColliderType Type => ColliderType.Hitbox;

    /// <returns>Lightweight converted collider as HitBoxAABB with IsActive state</returns>
    public HitBoxAABB GetHitBoxAABB()
    {
        float3 center = (float3)transform.position;

        return new HitBoxAABB(center - halfSize, center + halfSize);
    }


#if UNITY_EDITOR
    public float3 Size => halfSize * 2;

    public bool SkipNextGizmoDraw;
    public bool DrawGizmos;

    private static readonly Color INVALID_COLOR = new Color(0.6f, 0.2f, 1, 0.5f);
    private static readonly Color HITBOX_COLOR = new Color(0.25f, 0.85f, 0.5f, 0.15f);
    private static readonly Color HURTBOX_COLOR = new Color(1, 0.5f, 0, 0.5f);

    private void OnDrawGizmos()
    {
        if (!DrawGizmos) return;

        if (SkipNextGizmoDraw)
        {
            SkipNextGizmoDraw = false;
            return;
        }

        Gizmos.color = Type switch
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
    public float3 Min;
    public float3 Max;

    public HitBoxAABB(float3 min, float3 max)
    {
        Min = min;
        Max = max;
    }
}