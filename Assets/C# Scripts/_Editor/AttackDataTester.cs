#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;


/// <summary>
/// Editor utility MB to quickly visualize what hurtboxes are asigned in <see cref="AttackData.HitBoxIds"/>
/// </summary>
public class AttackDataTester : MonoBehaviour
{
    [SerializeField] private Transform targetPlayer;
    [InlineSO, SerializeField] private AttackSO targetData;

    private FastBoxCollider[] hitboxes;


    private void OnValidate()
    {
        if (targetPlayer == null || targetData == null) return;

        hitboxes = targetPlayer.GetComponentsInChildren<FastBoxCollider>(true);

        int hitboxCount = 0;
        for (int i = 0; i < hitboxes.Length; i++)
        {
            if (hitboxes[i].Type == ColliderType.Hurtbox)
            {
                hitboxes[hitboxCount] = hitboxes[i];
                hitboxCount += 1;
            }
        }
        Array.Resize(ref hitboxes, hitboxCount);

        EditorUtility.SetDirty(targetData);
    }

    private void OnDrawGizmos()
    {
        if (targetData == null || targetData.Value.HurtBoxIds.IsNullOrEmpty()) return;

        int attackHitBoxCount = targetData.Value.HurtBoxIds.Length;
        int hitBoxCount = hitboxes.Length;
        for (int i = 0; i < attackHitBoxCount; i++)
        {
            for (int i2 = 0; i2 < hitBoxCount; i2++)
            {
                float t = Mathf.PingPong((float)UnityEditor.EditorApplication.timeSinceStartup, 1f);
                Gizmos.color = Color.Lerp(Color.purple, Color.orange, t);

                Gizmos.DrawWireMesh(GlobalMeshes.Cube, hitboxes[i2].transform.position, Quaternion.identity, hitboxes[i2].Size);
                Gizmos.DrawWireMesh(GlobalMeshes.Cube, hitboxes[i2].transform.position, Quaternion.identity, hitboxes[i2].Size * 1.025f);
            }
        }
    }
}
#endif