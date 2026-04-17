using Unity.Mathematics;
using Unity.Burst;


/// <summary>
/// Static class with utility methods to check intersections (collision) between generated AABBs from the custom <see cref="FastBoxCollider"/> class
/// </summary>
[BurstCompile]
public static class CollisionUtils
{
    /// <summary>
    /// Called through attacker <see cref="PlayerController"/> every tick (60fps) while any hurtbox is still active
    /// </summary>
    /// <returns>True if ANY collision between any collider of <paramref name="groupA"/> with <paramref name="groupB"/> was found</returns>
    public static bool CheckAABBIntersection(FastBoxCollider[] groupA, FastBoxCollider[] groupB)
    {
        int groupA_Count = groupA.Length;
        int groupB_Count = groupB.Length;

        // Get Target HitBox AABBs
        AABB[] GroupA_AABBs = new AABB[groupA_Count];
        for (int i = 0; i < groupA_Count; i++)
        {
            GroupA_AABBs[i] = groupA[i].GetAABB();
        }
        // Get Player HurtBox AABBs
        AABB[] GroupB_AABBs = new AABB[groupB_Count];
        for (int i = 0; i < groupB_Count; i++)
        {
            GroupB_AABBs[i] = groupB[i].GetAABB();
        }

        for (int i = 0; i < groupA_Count; i++)
        {
            if (!groupA[i].isActiveAndEnabled) continue;

            for (int j = 0; j < groupB_Count; j++)
            {
                if (!groupB[i].isActiveAndEnabled) continue;

                // Any hit?
                if (TestAABB(in GroupA_AABBs[i], in GroupB_AABBs[j]))
                {
                    return true;
                }
            }
        }
        // No intersections (collision) found
        return false;
    }

    [BurstCompile]
    private static bool TestAABB(in AABB a, in AABB b)
    {
        return math.all(a.Min <= b.Max) && math.all(a.Max >= b.Min);
    }

    /// <summary>
    /// Get GuardResult based on what type of attack hit the target player in what FighterState
    /// </summary>
    public static GuardResult GetGuardResult(AttackLevel attackType, FighterState defenderState)
    {
        // If attack is unblockable, defender gets hit OR interrupted
        if (attackType == AttackLevel.Unblockable)
        {
            return defenderState == FighterState.MoveStartup ?
                GuardResult.Interrupted :
                GuardResult.Hit;
        }

        return defenderState switch
        {
            // Defenders is in startup animation fo his own attack, he gets intrerupted
            FighterState.MoveStartup => 
                GuardResult.Interrupted,

            // Defender Blocks Mids and Highs, loses to Lows
            FighterState.Retreating or FighterState.Idle or FighterState.StandingBlockStun =>
                attackType switch
                {
                    AttackLevel.Mid or AttackLevel.High => 
                        GuardResult.StandingBlocked,

                    _ => 
                        GuardResult.Hit,
                },

            // Defender blocks lows, loses to Mids and highs
            FighterState.Crouching or FighterState.CrouchedBlockStun =>
                attackType switch
                {
                    AttackLevel.Low => 
                        GuardResult.LowBlocked,

                    _ => 
                        GuardResult.Hit,
                },

            _ =>
                GuardResult.Hit,
        };
    }
}