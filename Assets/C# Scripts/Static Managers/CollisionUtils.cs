using Unity.Mathematics;
using Unity.Burst;


/// <summary>
/// Static class with utility methods to check intersections (collision) between generated AABBs from the custom <see cref="FastBoxCollider"/> class
/// </summary>
[BurstCompile]
public static class CollisionUtils
{
    /// <summary>
    /// Called through <see cref="PlayerController"/> every tick (60fps) while any hurtbox is still active
    /// </summary>
    /// <returns>True if ANY collision was found</returns>
    public static bool CheckAABBIntersection(FastBoxCollider[] hitBoxes, FastBoxCollider[] hurtBoxes)
    {
        int hitBoxCount = hitBoxes.Length;
        int hurtBoxCount = hurtBoxes.Length;

        // Get Target HitBox AABBs
        AABB[] hitBoxAABBs = new AABB[hitBoxCount];
        for (int i = 0; i < hitBoxCount; i++)
        {
            hitBoxAABBs[i] = hitBoxes[i].GetAABB();
        }
        // Get Player HurtBox AABBs
        AABB[] hurtBoxAABBs = new AABB[hurtBoxCount];
        for (int i = 0; i < hitBoxCount; i++)
        {
            hurtBoxAABBs[i] = hurtBoxes[i].GetAABB();
        }

        for (int i = 0; i < hurtBoxCount; i++)
        {
            if (!hurtBoxes[i].isActiveAndEnabled) continue;

            for (int j = 0; j < hitBoxCount; j++)
            {
                if (!hurtBoxes[i].isActiveAndEnabled) continue;

                // Any hit?
                if (TestAABB(in hurtBoxAABBs[i], in hitBoxAABBs[j]))
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