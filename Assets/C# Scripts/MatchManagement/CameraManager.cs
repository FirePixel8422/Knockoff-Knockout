using UnityEngine;


public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }


    private PlayerMovementHandler[] playerMoveHandlers;
    public Vector3 GetPlayerCenter() => (playerMoveHandlers[0].CurrentTransformPos + playerMoveHandlers[1].CurrentTransformPos) * 0.5f;


    [SerializeField] private Transform viewCenterTransform;
    [SerializeField] private float cameraLerpSpeed;

    [SerializeField] private float maxPlayerSpacing;
    [SerializeField] private float arenaRadius;

    private Vector3LerpState viewPositionState;


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
        Gizmos.DrawWireSphere(Vector3.zero, arenaRadius);
    }
#endif


    private void Awake()
    {
        Instance = this;

        viewPositionState = new Vector3LerpState(viewCenterTransform.position, viewCenterTransform.position);

        PlayerManager.PostPlayersInitialized += () =>
        {
            playerMoveHandlers = new PlayerMovementHandler[GlobalGameData.MAX_PLAYERS];
            for (int i = 0; i < GlobalGameData.MAX_PLAYERS; i++)
            {
                playerMoveHandlers[i] = PlayerManager.Instance.Players[i].MovementHandler;
            }
        };
    }

    public void UpdateCamera(float deltaTime)
    {
        viewPositionState.Target = GetPlayerCenter();

        if (viewPositionState.IsCompleted(0.01f)) return;

        viewCenterTransform.position = viewPositionState.Lerp(cameraLerpSpeed * deltaTime);
    }

    public Vector3 ClampMovementToCameraBounds(Vector3 currentPosition, Vector3 addedMovement)
    {
        Vector3 center = GetPlayerCenter();

        Vector3 targetPosition = currentPosition + addedMovement;

        float halfSpacing = maxPlayerSpacing * 0.5f;

        // ---------- PLAYER SPACING CONSTRAINT ----------
        Vector3 offsetFromCenter = targetPosition - center;
        offsetFromCenter.y = 0;

        if (offsetFromCenter.sqrMagnitude > halfSpacing * halfSpacing)
        {
            Vector3 dir = offsetFromCenter.normalized;
            targetPosition = center + dir * halfSpacing;
            targetPosition.y = currentPosition.y;
        }

        // ---------- ARENA SPHERE CONSTRAINT ----------
        Vector3 fromArenaCenter = targetPosition;
        fromArenaCenter.y = 0;

        if (fromArenaCenter.sqrMagnitude > arenaRadius * arenaRadius)
        {
            Vector3 dir = fromArenaCenter.normalized;
            targetPosition = dir * arenaRadius;
            targetPosition.y = currentPosition.y;
        }

        return targetPosition;
    }

    public Vector3 GetForwardDir()
    {
        Vector3 self = playerMoveHandlers[0].CurrentTransformPos;
        Vector3 other = playerMoveHandlers[1].CurrentTransformPos;

        Vector3 dir = other - self;
        dir.y = 0;

        return dir.normalized;
    }
}
