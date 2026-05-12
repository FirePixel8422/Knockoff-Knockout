#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor utility MB to quickly visualize what hurtBoxes are assigned in AttackData.HurtBoxIds.
/// </summary>
public class AttackDataTester : MonoBehaviour
{
    [SerializeField] private Transform targetPlayer;
    [InlineSO, SerializeField] private AttackSO targetData;

    private FastBoxCollider[] hurtBoxes;



    private void OnDrawGizmos()
    {
        if (targetPlayer == null || targetData == null || targetData.Value.HurtBoxIds.IsNullOrEmpty())
            return;

        HashSet<int> ids = new HashSet<int>(targetData.Value.HurtBoxIds);

        FastBoxCollider[] all = targetPlayer.GetComponentsInChildren<FastBoxCollider>(true);

        List<FastBoxCollider> filtered = new List<FastBoxCollider>(all.Length);

        for (int i = 0; i < all.Length; i++)
        {
            FastBoxCollider box = all[i];

            if (box.Type == ColliderType.Hurtbox && ids.Contains(box.Id))
            {
                filtered.Add(box);
            }
        }

        hurtBoxes = filtered.ToArray();
        EditorUtility.SetDirty(targetData);

        float t = Mathf.PingPong((float)EditorApplication.timeSinceStartup, 1f);
        Color c = Color.Lerp(Color.purple, Color.orange, t);

        Gizmos.color = c;

        for (int i = 0; i < hurtBoxes.Length; i++)
        {
            FastBoxCollider box = hurtBoxes[i];

            Gizmos.DrawWireMesh(GlobalMeshes.Cube, box.transform.position, Quaternion.identity, box.Size);
            Gizmos.DrawWireMesh(GlobalMeshes.Cube, box.transform.position, Quaternion.identity, box.Size * 1.025f);
        }
    }
}
#endif