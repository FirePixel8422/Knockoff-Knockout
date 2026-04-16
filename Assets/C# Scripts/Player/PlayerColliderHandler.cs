using UnityEngine;


/// <summary>
/// Sub Player system handler class that is responsible for tracking all player collider hit and hurt boxes.
/// </summary>
[System.Serializable]
public class PlayerColliderHandler
{
    public FastBoxCollider[] HitBoxes { get; private set; }
    public FastBoxCollider[] HurtBoxes { get; private set; }


    public void Init(Transform playerRoot)
    {
        FastBoxCollider[] colliders = playerRoot.GetComponentsInChildren<FastBoxCollider>();

        int colliderCount = colliders.Length;
        int hitBoxCount = 0;
        int hurtBoxCount = 0;

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

        HitBoxes = new FastBoxCollider[hitBoxCount];
        HurtBoxes = new FastBoxCollider[hurtBoxCount];
        int hitBoxId = 0;
        int hurtBoxId = 0;

        // Store HitBox and HurtBoxs
        for (int i = 0; i < colliderCount; i++)
        {
            if (colliders[i].Type == ColliderType.Hitbox)
            {
                HitBoxes[hitBoxId++] = colliders[i];
            }
            else if (colliders[i].Type == ColliderType.Hurtbox)
            {
                HurtBoxes[hurtBoxId++] = colliders[i];
            }
        }
    }
}