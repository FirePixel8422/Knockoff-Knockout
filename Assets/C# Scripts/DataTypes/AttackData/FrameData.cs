using UnityEngine;


[System.Serializable]
public struct FrameData
{
    [Header("Move (Attack) Startup, Duration, Recovery and Cancel Window")]
    public int Startup;
    public int Active;
    public int Recovery;
    public int CancelWindow;

    [Header("Move (Attack) 'OnHit', 'OnBlock' and 'OnCounter' Stun")]
    public int HitStun;
    public int BlockStun;
    public int CounterStun;

    [Header("Move (Attack) HitStop and BlockStop for dramatic effect when connecting it")]
    public int HitStop;
    public int BlockStop;
}