using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;


/// <summary>
/// Sub Player system handler class that is responsible for tracking all player collider hit and hurt boxes.
/// </summary>
[System.Serializable]
public class PlayerColliderHandler
{
    private readonly FastHitBox[] hitBoxes;
    private readonly FastHurtBox[] hurtBoxes;
    private readonly int hitBoxCount;
    private readonly int hurtBoxCount;

    private NativeArray<HitBoxOBB> hitBoxOBBs;
    private NativeArray<HurtBoxOBB> hurtBoxOBBs;
    public NativeArray<HitBoxOBB> HitBoxOBBs => hitBoxOBBs;
    public NativeArray<HurtBoxOBB> HurtBoxOBBs => hurtBoxOBBs;

    private readonly HashSet<int> activeHurtBoxIds = new HashSet<int>();


    public PlayerColliderHandler(Transform playerRoot)
    {
        hitBoxes = playerRoot.GetComponentsInChildren<FastHitBox>(true);
        hurtBoxes = playerRoot.GetComponentsInChildren<FastHurtBox>(true);

        hitBoxCount = hitBoxes.Length;
        hurtBoxCount = hurtBoxes.Length;

        hitBoxOBBs = new NativeArray<HitBoxOBB>(hitBoxCount, Allocator.Persistent);
        hurtBoxOBBs = new NativeArray<HurtBoxOBB>(hurtBoxCount, Allocator.Persistent);

        DisableAllHurtBoxes();
    }
    private PlayerColliderHandler() { }

    ~PlayerColliderHandler()
    {
        hitBoxOBBs.Dispose();
        hurtBoxOBBs.Dispose();
    }

    /// <summary>
    /// Set HurtBox OBBs active states. (Does not recalculate internal collider struct array)
    /// </summary>
    public void EnableTargetHurtBoxes(int[] ids)
    {
        int idCount = ids.Length;
        activeHurtBoxIds.Clear();

        for (int i = 0; i < idCount; i++)
        {
            activeHurtBoxIds.Add(ids[i]);
        }
        for (int i = 0; i < hurtBoxCount; i++)
        {
            hurtBoxes[i].enabled = activeHurtBoxIds.Contains(hurtBoxes[i].Id);
        }
    }
    public void DisableAllHurtBoxes()
    {
        for (int i = 0; i < hurtBoxCount; i++)
        {
            hurtBoxes[i].enabled = false;
        }
    }

    /// <summary>
    /// Recalculate Hurtbox OBBs from collider transforms and active states.
    /// </summary>
    public void RecalculateHurtBoxes()
    {
        for (int i = 0; i < hurtBoxCount; i++)
        {
            hurtBoxOBBs[i] = hurtBoxes[i].GetHurtBoxOBB();
        }
    }
    /// <summary>
    /// Recalculate Hitbox OBBs from collider transforms and active states.
    /// </summary>
    public void RecalculateHitBoxes()
    {
        for (int i = 0; i < hitBoxCount; i++)
        {
            hitBoxOBBs[i] = hitBoxes[i].GetHitBoxOBB();
        }
    }
}