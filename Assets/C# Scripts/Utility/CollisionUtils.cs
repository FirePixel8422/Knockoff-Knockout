using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;


/// <summary>
/// Static class with utility methods to check intersections (collision) between generated AABBs from the custom <see cref="FastHitBox"/> class
/// </summary>
[BurstCompile]
public static class CollisionUtils
{
    /// <summary>
    /// Called through attacker <see cref="PlayerController"/> every tick (60fps) while any hurtbox is still active
    /// </summary>
    /// <returns>True if ANY collision between any collider of <paramref name="groupA"/> with <paramref name="groupB"/> was found</returns>
    [BurstCompile]
    public static bool CheckIntersection(in NativeArray<HitBoxOBB> hitboxes, in NativeArray<HurtBoxOBB> hurtboxes)
    {
        int hitBoxCount = hitboxes.Length;
        int hurtBoxCount = hurtboxes.Length;

        for (int i = 0; i < hitBoxCount; i++)
        {
            HitBoxOBB a = hitboxes[i];

            for (int j = 0; j < hurtBoxCount; j++)
            {
                HurtBoxOBB b = hurtboxes[j];

                if (!b.IsActive) continue;

                if (TestOBB(in a, in b))
                    return true;
            }
        }
        // No intersections (collision) found
        return false;
    }

    [BurstCompile]
    public static bool TestOBB(in HitBoxOBB a, in HurtBoxOBB b)
    {
        const float EPS = 1e-4f;

        // A local axes
        float3 aX = math.mul(a.Rotation, new float3(1, 0, 0));
        float3 aY = math.mul(a.Rotation, new float3(0, 1, 0));
        float3 aZ = math.mul(a.Rotation, new float3(0, 0, 1));

        // B local axes
        float3 bX = math.mul(b.Rotation, new float3(1, 0, 0));
        float3 bY = math.mul(b.Rotation, new float3(0, 1, 0));
        float3 bZ = math.mul(b.Rotation, new float3(0, 0, 1));

        float3 aE = a.Extents;
        float3 bE = b.Extents;

        // translation from A to B in world space
        float3 tWorld = b.Center - a.Center;

        // bring into A space
        float3 t = new float3(
            math.dot(tWorld, aX),
            math.dot(tWorld, aY),
            math.dot(tWorld, aZ)
        );

        // rotation matrix R (A -> B)
        float3x3 R = new float3x3(
            math.dot(aX, bX), math.dot(aX, bY), math.dot(aX, bZ),
            math.dot(aY, bX), math.dot(aY, bY), math.dot(aY, bZ),
            math.dot(aZ, bX), math.dot(aZ, bY), math.dot(aZ, bZ)
        );

        float3x3 AbsR = new float3x3(
            math.abs(R.c0) + EPS,
            math.abs(R.c1) + EPS,
            math.abs(R.c2) + EPS
        );

        float3 R0 = R.c0;
        float3 R1 = R.c1;
        float3 R2 = R.c2;

        float3 AR0 = AbsR.c0;
        float3 AR1 = AbsR.c1;
        float3 AR2 = AbsR.c2;

        float ra, rb;

        // =========================
        // A axes (3 tests)
        // =========================
        ra = aE.x;
        rb = bE.x * AR0.x + bE.y * AR0.y + bE.z * AR0.z;
        if (math.abs(t.x) > ra + rb) return false;

        ra = aE.y;
        rb = bE.x * AR1.x + bE.y * AR1.y + bE.z * AR1.z;
        if (math.abs(t.y) > ra + rb) return false;

        ra = aE.z;
        rb = bE.x * AR2.x + bE.y * AR2.y + bE.z * AR2.z;
        if (math.abs(t.z) > ra + rb) return false;

        // =========================
        // B axes (3 tests)
        // =========================
        ra = aE.x * AR0.x + aE.y * AR1.x + aE.z * AR2.x;
        rb = bE.x;
        if (math.abs(t.x * R0.x + t.y * R1.x + t.z * R2.x) > ra + rb) return false;

        ra = aE.x * AR0.y + aE.y * AR1.y + aE.z * AR2.y;
        rb = bE.y;
        if (math.abs(t.x * R0.y + t.y * R1.y + t.z * R2.y) > ra + rb) return false;

        ra = aE.x * AR0.z + aE.y * AR1.z + aE.z * AR2.z;
        rb = bE.z;
        if (math.abs(t.x * R0.z + t.y * R1.z + t.z * R2.z) > ra + rb) return false;

        // =========================
        // Cross products (9 tests)
        // =========================

        // A0 x B0
        ra = aE.y * AR2.x + aE.z * AR1.x;
        rb = bE.y * AR0.z + bE.z * AR0.y;
        if (math.abs(t.z * R1.x - t.y * R2.x) > ra + rb) return false;

        // A0 x B1
        ra = aE.y * AR2.y + aE.z * AR1.y;
        rb = bE.x * AR0.z + bE.z * AR0.x;
        if (math.abs(t.z * R1.y - t.y * R2.y) > ra + rb) return false;

        // A0 x B2
        ra = aE.y * AR2.z + aE.z * AR1.z;
        rb = bE.x * AR0.y + bE.y * AR0.x;
        if (math.abs(t.z * R1.z - t.y * R2.z) > ra + rb) return false;

        // A1 x B0
        ra = aE.x * AR2.x + aE.z * AR0.x;
        rb = bE.y * AR1.z + bE.z * AR1.y;
        if (math.abs(t.x * R2.x - t.z * R0.x) > ra + rb) return false;

        // A1 x B1
        ra = aE.x * AR2.y + aE.z * AR0.y;
        rb = bE.x * AR1.z + bE.z * AR1.x;
        if (math.abs(t.x * R2.y - t.z * R0.y) > ra + rb) return false;

        // A1 x B2
        ra = aE.x * AR2.z + aE.z * AR0.z;
        rb = bE.x * AR1.y + bE.y * AR1.x;
        if (math.abs(t.x * R2.z - t.z * R0.z) > ra + rb) return false;

        // A2 x B0
        ra = aE.x * AR1.x + aE.y * AR0.x;
        rb = bE.y * AR2.z + bE.z * AR2.y;
        if (math.abs(t.y * R0.x - t.x * R1.x) > ra + rb) return false;

        // A2 x B1
        ra = aE.x * AR1.y + aE.y * AR0.y;
        rb = bE.x * AR2.z + bE.z * AR2.x;
        if (math.abs(t.y * R0.y - t.x * R1.y) > ra + rb) return false;

        // A2 x B2
        ra = aE.x * AR1.z + aE.y * AR0.z;
        rb = bE.x * AR2.y + bE.y * AR2.x;
        if (math.abs(t.y * R0.z - t.x * R1.z) > ra + rb) return false;

        return true;
    }
}