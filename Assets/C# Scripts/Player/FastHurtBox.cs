using Unity.Mathematics;
using UnityEngine;


public class FastHurtBox : FastHitBox
{
    [SerializeField] private int id = -1;
    
    public int Id => id;
    public override ColliderType Type => ColliderType.Hurtbox;


    /// <returns>Lightweight converted collider as HurtBoxOBB with IsActive state</returns>
    public HurtBoxOBB GetHurtBoxOBB()
    {
        if (!isActiveAndEnabled)
        {
            // If collider is inactive, spare some calculations be returning an empty collider struct
            return HurtBoxOBB.Null;
        }

        cachedTransform.GetPositionAndRotation(out Vector3 center, out Quaternion rotation);
        return new HurtBoxOBB(true, center, halfSize, rotation);
    }
}


[System.Serializable]
public struct HurtBoxOBB
{
    private readonly byte activeFlag;
    public readonly bool IsActive => activeFlag != 0;
    public float3 Center;
    public float3 Extents;
    public quaternion Rotation;

    public HurtBoxOBB(bool active, float3 center, float3 extents, quaternion rotation)
    {
        activeFlag = (byte)(active ? 1 : 0);
        Center = center;
        Extents = extents;
        Rotation = rotation;
    }
    public static HurtBoxOBB Null => new HurtBoxOBB();
}