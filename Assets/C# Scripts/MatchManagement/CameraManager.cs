using UnityEngine;


public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    private void Awake() => Instance = this;


    [EditorReadOnly, SerializeField] private Transform[] playerTransforms;
    [EditorReadOnly, SerializeField] private Camera mainCam;


    private void Start()
    {
        playerTransforms = new Transform[GlobalGameData.MAX_PLAYERS];
        for (int i = 0; i < GlobalGameData.MAX_PLAYERS; i++)
        {
            playerTransforms[i] = PlayerManager.Instance.Players[i].transform;
        }
    }

    public Vector3 GetForwardDir(Transform transform)
    {
        int playerId = -1;
        for (int i = 0; i < GlobalGameData.MAX_PLAYERS; i++)
        {
            if (playerTransforms[i] == transform)
            {
                playerId = i;
                break;
            }
        }

        Vector3 self = playerTransforms[0].position;
        Vector3 other = playerTransforms[1].position;

        Vector3 dir = other - self;
        dir.y = 0;

        return dir.normalized;
    }
}
