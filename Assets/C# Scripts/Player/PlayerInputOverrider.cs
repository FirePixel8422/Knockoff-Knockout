using UnityEditor;
using UnityEngine;


public class PlayerInputOverrider : MonoBehaviour
{
    [SerializeField] private OverrideMode mode;
    [SerializeField] private DummyRecordingSO recordingSO;

    [EditorReadOnly, SerializeField] private int index;
    [EditorReadOnly, SerializeField] private FrameInput cRawInput;

    [EditorReadOnly, SerializeField] private PlayerInputRouter playerInputRouter;
    [EditorReadOnly, SerializeField] private bool isLeftPlayer;

    private bool IsRecording => mode == OverrideMode.Collect;


    private void Awake()
    {
        playerInputRouter = GetComponent<PlayerInputRouter>();
        isLeftPlayer = GetComponent<PlayerController>().IsLeftPlayer;
    }

    public void OnDirection(Vector2 dirVec)
    {
        if (!IsRecording) return;

        DirectionInput dirInput;

        if (dirVec == Vector2.zero)
        {
            dirInput = DirectionInput.Neutral;
        }
        else if (Mathf.Abs(dirVec.x) > Mathf.Abs(dirVec.y))
        {
            dirInput = dirVec.x >= 0
                ? DirectionInput.Right
                : DirectionInput.Left;
        }
        else
        {
            dirInput = dirVec.y >= 0
                ? DirectionInput.Up
                : DirectionInput.Down;
        }

        cRawInput.DirectionFlag = dirInput;
    }
    public void OnButtonPressed(AttackInputFlags flag)
    {
        if (!IsRecording) return;

        cRawInput.AttackFlags |= flag;
    }


    public void CollectInputs()
    {
        if (recordingSO == null) return;

        switch (mode)
        {
            case OverrideMode.None:
            default:
                return;

            case OverrideMode.Collect:
                if (!playerInputRouter.IsAssigned) return;
                recordingSO.Timeline.Add(cRawInput);
                EditorUtility.SetDirty(recordingSO);
                cRawInput.AttackFlags = AttackInputFlags.None;
                break;

            case OverrideMode.Playback:
                if (playerInputRouter.IsAssigned || index == recordingSO.Timeline.Count) return;
                SendInput(recordingSO.Timeline[index]);
                index.IncrementSmart(recordingSO.Timeline.Count);
                break;
        }
    }
    private void SendInput(FrameInput input)
    {
        bool invert = !isLeftPlayer && recordingSO.DoMirrorForRightPlayer;

        Vector3 dir = input.DirectionFlag switch
        {
            DirectionInput.Left => invert ? Vector2.right : Vector2.left,
            DirectionInput.Right => invert ? Vector2.left : Vector2.right,
            DirectionInput.Up => Vector2.up,
            DirectionInput.Down => Vector2.down,
            _ => Vector2.zero
        };

        playerInputRouter.OnDirection(dir);
        playerInputRouter.OnButtonPressed(input.AttackFlags);
    }


    private enum OverrideMode
    {
        None,
        Collect,
        Playback,
    }
}
