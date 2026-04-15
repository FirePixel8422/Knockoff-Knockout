using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;


/// <summary>
/// Sub Player system handler class that is responsible for tracking all player collider hit and hurt boxes.
/// </summary>
[System.Serializable]
public class PlayerColliderHandler
{
    [SerializeField] private FastBoxCollider[] hitBoxes;
    [SerializeField] private FastBoxCollider[] hurtBoxes;
    public FastBoxCollider[] HitBoxes => hitBoxes;


    /// <summary>
    /// Called through <see cref="PlayerController"/> every tick (60fps) while any hurtbox is still active
    /// </summary>
    public void NEED_GOOD_NAME_FOR_METHOD(FastBoxCollider[] enemyHitBoxes, AttackType attackType, FighterState enemyState)
    {
        int enemyHitBoxCount = enemyHitBoxes.Length;
        int playerHurtBoxCount = hurtBoxes.Length;

        // Get Enemy HitBox AABBs
        AABB[] enemyAABBs = new AABB[enemyHitBoxCount];
        for (int i = 0; i < enemyHitBoxCount; i++)
        {
            enemyAABBs[i] = enemyHitBoxes[i].GetAABB();
        }
        // Get Player HurtBox AABBs
        AABB[] playerAABBs = new AABB[enemyHitBoxCount];
        for (int i = 0; i < enemyHitBoxCount; i++)
        {
            playerAABBs[i] = hurtBoxes[i].GetAABB();
        }

        for (int i = 0; i < playerHurtBoxCount; i++)
        {
            if (!hurtBoxes[i].isActiveAndEnabled) continue;

            for (int j = 0; j < enemyHitBoxCount; j++)
            {
                if (!hurtBoxes[i].isActiveAndEnabled) continue;

                // Any hit?
                if (TestAABB(in playerAABBs[i], in enemyAABBs[j]))
                {
                    ResolveConnectedMove(attackType, enemyState);
                    return;
                }
            }
        }
    }
    private void ResolveConnectedMove(AttackType attackType, FighterState enemyState)
    {
        switch (enemyState)
        {
            case FighterState.MoveStartup:
                // Interrupt, worst scenario
                break;

            case FighterState.:

                break;


            case FighterState.Retreating:
            case FighterState.Idle:

                break;
        }
    }

    [BurstCompile]
    private static bool TestAABB(in AABB a, in AABB b)
    {
        return math.all(a.Min <= b.Max) && math.all(a.Max >= b.Min);
    }
}