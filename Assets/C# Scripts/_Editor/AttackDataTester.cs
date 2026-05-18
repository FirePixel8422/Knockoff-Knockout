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



    private void OnDrawGizmos()
    {
        if (targetPlayer == null || targetData == null || targetData.Value.HurtBoxIds.IsNullOrEmpty())
            return;

        HashSet<int> ids = new HashSet<int>(targetData.Value.HurtBoxIds);

        FastHurtBox[] hurtBoxes = targetPlayer.GetComponentsInChildren<FastHurtBox>(true);
        List<FastHurtBox> filtered = new List<FastHurtBox>(hurtBoxes.Length);

        for (int i = 0; i < hurtBoxes.Length; i++)
        {
            FastHurtBox box = hurtBoxes[i];

            if (ids.Contains(box.Id))
            {
                filtered.Add(box);
                box.SkipNextGizmoDraw = true;
            }
        }

        hurtBoxes = filtered.ToArray();
        EditorUtility.SetDirty(targetData);

        float t = Mathf.PingPong((float)EditorApplication.timeSinceStartup, 1f);
        Gizmos.color = Color.Lerp(Color.purple, Color.blue, t);

        for (int i = 0; i < hurtBoxes.Length; i++)
        {
            FastHurtBox box = hurtBoxes[i];

            Gizmos.DrawWireMesh(GlobalMeshes.Cube, box.transform.position, Quaternion.identity, box.Size);
            Gizmos.DrawWireMesh(GlobalMeshes.Cube, box.transform.position, Quaternion.identity, box.Size * 1.025f);
        }
    }
}
#endif