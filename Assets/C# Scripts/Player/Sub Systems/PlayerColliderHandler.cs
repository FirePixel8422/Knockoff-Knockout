using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;


/// <summary>
/// Sub Player system handler class that is responsible for tracking all player collider hit and hurt boxes.
/// </summary>
[System.Serializable]
public class PlayerColliderHandler
{
    private readonly FastBoxCollider[] hitBoxes;
    private readonly FastBoxCollider[] hurtBoxes;
    private readonly int hitBoxCount;
    private readonly int hurtBoxCount;

    private NativeArray<HitBoxAABB> hitBoxAABBs;
    private NativeArray<HitBoxAABB> hurtBoxAABBs;
    public NativeArray<HitBoxAABB> HitBoxAABBs => hitBoxAABBs;
    public NativeArray<HitBoxAABB> HurtBoxAABBs => hurtBoxAABBs;

    private readonly HashSet<int> activeHurtBoxIds = new HashSet<int>();


    public PlayerColliderHandler(Transform playerRoot)
    {
        FastBoxCollider[] colliders = playerRoot.GetComponentsInChildren<FastBoxCollider>(true);

        int colliderCount = colliders.Length;
        hitBoxCount = 0;
        hurtBoxCount = 0;

        // Get HitBox and HurtBox Counts
        for (int i = 0; i < colliderCount; i++)
        {
            if (colliders[i].Type == ColliderType.Hitbox)
            {
                hitBoxCount += 1;
            }
            else if (colliders[i].Type == ColliderType.Hurtbox)
            {
                hurtBoxCount += 1;
            }
        }

        hitBoxes = new FastBoxCollider[hitBoxCount];
        hurtBoxes = new FastBoxCollider[hurtBoxCount];

        hitBoxAABBs = new NativeArray<HitBoxAABB>(hitBoxCount, Allocator.Persistent);
        hurtBoxAABBs = new NativeArray<HitBoxAABB>(hurtBoxCount, Allocator.Persistent);

        int hitBoxId = 0;
        int hurtBoxId = 0;

        // Store HitBox and HurtBoxs
        for (int i = 0; i < colliderCount; i++)
        {
            if (colliders[i].Type == ColliderType.Hitbox)
            {
                hitBoxes[hitBoxId++] = colliders[i];
            }
            else if (colliders[i].Type == ColliderType.Hurtbox)
            {
                hurtBoxes[hurtBoxId++] = colliders[i];
            }
        }
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

        for (int i = 0; i < ids.Length; i++)
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
            hurtBoxAABBs[i] = hurtBoxes[i].GetAABB();
        }
    }
    /// <summary>
    /// Recalculate Hitbox AABBs from collider transforms and active states.
    /// </summary>
    public void RecalculateHitBoxes()
    {
        for (int i = 0; i < hitBoxCount; i++)
        {
            hitBoxAABBs[i] = hitBoxes[i].GetAABB();
        }
    }


#if UNITY_EDITOR
    public void DebugLoadColliders(Transform playerRoot)
    {

    }
#endif
}