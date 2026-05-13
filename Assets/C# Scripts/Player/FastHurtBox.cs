using Unity.Mathematics;
using UnityEngine;


public class FastHurtBox : FastHitBox
{
    [SerializeField] private int id;
    
    public int Id => id;
    public override ColliderType Type => ColliderType.Hurtbox;


    /// <returns>Lightweight converted collider as HurtBoxAABB with IsActive state</returns>
    public HurtBoxAABB GetHurtBoxAABB()
    {
        float3 center = (float3)transform.position;

        return new HurtBoxAABB(isActiveAndEnabled, center - halfSize, center + halfSize);
    }
}


[System.Serializable]
public struct HurtBoxAABB
{
    private readonly byte activeFlag;
    public readonly bool IsActive => activeFlag == 1;

    public float3 Min;
    public float3 Max;

    public HurtBoxAABB(bool active, float3 min, float3 max)
    {
        activeFlag = (byte)(active ? 1 : 0);
        Min = min;
        Max = max;
    }
}