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

    private PlayerMovementHandler[] playerMoveHandlers;


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
        playerMoveHandlers = new PlayerMovementHandler[GlobalGameData.MAX_PLAYERS];
        for (int i = 0; i < GlobalGameData.MAX_PLAYERS; i++)
        {
            playerMoveHandlers[i] = PlayerManager.Instance.Players[i].MovementHandler;
        }
    }

    public void TickUpdate()
    {
        Vector3 playerPosA = playerMoveHandlers[0].CurrentTransformPos;
        Vector3 playerPosB = playerMoveHandlers[1].CurrentTransformPos;

        float playerDist = Vector3.Distance(playerPosA, playerPosB);
        if (playerDist >= maxPushRange) return;

        float pushStrength = math.saturate(playerDist / maxPushRange) * pushStrengthMultiplier;

        Vector3 moveDirA = playerMoveHandlers[0].LastMoveDir;
        Vector3 pushDirA = (playerPosA - playerPosB).normalized;
        Vector3 targetPushA = Vector3.Lerp(pushDirA, moveDirA, moveDirImpact);

        Vector3 pushDirB = (playerPosB - playerPosA).normalized;
        Vector3 moveDirB = playerMoveHandlers[1].LastMoveDir;
        Vector3 targetPushB = Vector3.Lerp(pushDirB, moveDirB, moveDirImpact);

        playerMoveHandlers[0].MovePlayer(targetPushA * pushStrength);
        playerMoveHandlers[1].MovePlayer(targetPushB * pushStrength);
    }
}