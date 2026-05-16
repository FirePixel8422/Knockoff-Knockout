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

    private NativeArray<HitBoxAABB> hitBoxAABBs;
    private NativeArray<HurtBoxAABB> hurtBoxAABBs;
    public NativeArray<HitBoxAABB> HitBoxAABBs => hitBoxAABBs;
    public NativeArray<HurtBoxAABB> HurtBoxAABBs => hurtBoxAABBs;

    private readonly HashSet<int> activeHurtBoxIds = new HashSet<int>();


    public PlayerColliderHandler(Transform playerRoot)
    {
        hitBoxes = playerRoot.GetComponentsInChildren<FastHitBox>(true);
        hurtBoxes = playerRoot.GetComponentsInChildren<FastHurtBox>(true);

        hitBoxCount = hitBoxes.Length;
        hurtBoxCount = hurtBoxes.Length;

        hitBoxAABBs = new NativeArray<HitBoxAABB>(hitBoxCount, Allocator.Persistent);
        hurtBoxAABBs = new NativeArray<HurtBoxAABB>(hurtBoxCount, Allocator.Persistent);
    }

    ~PlayerColliderHandler()
    {
        hitBoxAABBs.Dispose();
        hurtBoxAABBs.Dispose();
    }

    /// <summary>
    /// Set HurtBox AABBs active states.
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
        RecalculateHurtBoxes();
    }
    public void DisableAllHurtBoxes()
    {
        for (int i = 0; i < hurtBoxCount; i++)
        {
            hurtBoxes[i].enabled = false;
        }
        RecalculateHurtBoxes();
    }

    /// <summary>
    /// Recalculate Hurtbox AABBs from collider transforms and active states.
    /// </summary>
    private void RecalculateHurtBoxes()
    {
        for (int i = 0; i < hurtBoxCount; i++)
        {
            hurtBoxAABBs[i] = hurtBoxes[i].GetHurtBoxAABB();
        }
    }
    /// <summary>
    /// Recalculate Hitbox AABBs from collider transforms and active states.
    /// </summary>
    public void RecalculateHitBoxes()
    {
        for (int i = 0; i < hitBoxCount; i++)
        {
            hitBoxAABBs[i] = hitBoxes[i].GetHitBoxAABB();
        }
    }
}