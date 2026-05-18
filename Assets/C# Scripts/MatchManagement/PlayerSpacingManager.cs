using Unity.Mathematics;
using UnityEngine;



public class PlayerSpacingManager : MonoBehaviour
{
    public static PlayerSpacingManager Instance { get; private set; }
    private void Awake() => Instance = this;


    [SerializeField] private float maxPushRange = 3;
    [Range(0, 1)]
    [SerializeField] private float moveDirImpact = 0.5f;
    [SerializeField] private float pushStrengthMultiplier = 1;

    private PlayerController[] players;


#if UNITY_EDITOR
    [SerializeField] private bool drawPushSphereGizmos;

    private void OnDrawGizmos()
    {
        if (!drawPushSphereGizmos) return;

        PlayerManager playerManager = this.FindObjectOfType<PlayerManager>();

        Transform playerA = playerManager.Players[0].transform;
        Transform playerB = playerManager.Players[1].transform;

        Gizmos.color = playerManager.PlayerColors[0];
        Gizmos.DrawSphere(playerA.position, maxPushRange * 0.5f);

        Gizmos.color = playerManager.PlayerColors[1];
        Gizmos.DrawSphere(playerB.position, maxPushRange * 0.5f);
    }
#endif

    private void Start()
    {
        players = PlayerManager.Instance.Players;
    }

    public void OnUpdate()
    {
        Vector3 playerPosA = players[0].transform.position;
        Vector3 playerPosB = players[1].transform.position;

        float playerDist = Vector3.Distance(playerPosA, playerPosB);
        if (playerDist >= maxPushRange) return;

        float pushStrength = math.saturate(playerDist / maxPushRange) * pushStrengthMultiplier;

        Vector3 moveDirA = players[0].MovementHandler.LastMoveDir;
        Vector3 pushDirA = playerPosA - playerPosB;
        Vector3 targetPushA = Vector3.Lerp(pushDirA, moveDirA, moveDirImpact);

        Vector3 pushDirB = playerPosB - playerPosA;
        Vector3 moveDirB = players[1].MovementHandler.LastMoveDir;
        Vector3 targetPushB = Vector3.Lerp(pushDirB, moveDirB, moveDirImpact);

        players[0].MovementHandler.MovePlayer(targetPushA * pushStrength);
        players[1].MovementHandler.MovePlayer(targetPushB * pushStrength);
    }
}