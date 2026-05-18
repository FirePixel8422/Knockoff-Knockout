using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;


public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    private void Awake() => Instance = this;


    private Transform[] playerTransforms;
    private Camera mainCam;
    public Vector3 GetPlayerCenter() => (playerTransforms[0].position + playerTransforms[1].position) * 0.5f;


    [SerializeField] private float cameraLerpSpeed;


#if UNITY_EDITOR
    [SerializeField] private bool drawCameraGizmos;

    private void OnDrawGizmos()
    {
        if (!drawCameraGizmos) return;

        PlayerManager playerManager = this.FindObjectOfType<PlayerManager>();

        Transform playerA = playerManager.Players[0].transform;
        Transform playerB = playerManager.Players[1].transform;


        Gizmos.color = playerManager.PlayerColors[0];
        Gizmos.DrawWireMesh(GlobalMeshes.Cube, playerA.position, playerA.rotation, new Vector3(0.35f, 0.01f, 0.35f));

        Gizmos.color = playerManager.PlayerColors[1];
        Gizmos.DrawWireMesh(GlobalMeshes.Cube, playerB.position, playerB.rotation, new Vector3(0.35f, 0.01f, 0.35f));


        Vector3 center = (playerA.position + playerB.position) * 0.5f;
        Vector3 direction = (playerB.position - playerA.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

        Gizmos.color = Color.white;
        Gizmos.DrawWireMesh(GlobalMeshes.Cube, center, rotation, new Vector3(0.35f, 0.01f, 0.35f));
    }
#endif


    private void Start()
    {
        playerTransforms = new Transform[GlobalGameData.MAX_PLAYERS];
        for (int i = 0; i < GlobalGameData.MAX_PLAYERS; i++)
        {
            playerTransforms[i] = PlayerManager.Instance.Players[i].transform;
        }
    }

    public Vector3 GetForwardDir()
    {
        Vector3 self = playerTransforms[0].position;
        Vector3 other = playerTransforms[1].position;

        Vector3 dir = other - self;
        dir.y = 0;

        return dir.normalized;
    }
}
