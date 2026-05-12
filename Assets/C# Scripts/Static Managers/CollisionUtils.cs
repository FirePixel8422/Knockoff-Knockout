using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;


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
    [BurstCompile]
    public static bool CheckAABBIntersection(in NativeArray<HitBoxAABB> groupA, in NativeArray<HitBoxAABB> groupB)
    {
        int groupACount = groupA.Length;
        int groupBCount = groupB.Length;

        HitBoxAABB hitboxA;
        HitBoxAABB hitboxB;

        for (int a = 0; a < groupACount; a++)
        {
            hitboxA = groupA[a];

            if (!hitboxA.IsActive) continue;

            for (int b = 0; b < groupBCount; b++)
            {
                hitboxB = groupB[b];

                if (!hitboxB.IsActive) continue;

                // Check for a box intersection
                if (TestAABB(in hitboxA, in hitboxB))
                {
                    return true;
                }
            }
        }
        // No intersections (collision) found
        return false;
    }

    [BurstCompile]
    private static bool TestAABB(in HitBoxAABB a, in HitBoxAABB b)
    {
        return math.all(a.Min <= b.Max) && math.all(a.Max >= b.Min);
    }
}