using UnityEngine;


[System.Serializable]
public struct FrameData
{
    [Header("Move (Attack) Startup, Duration, Recovery")]
    public int Startup;
    public int Active;
    public int Recovery;

    [Header("Move (Attack) Window (Duration) after first active frame to cancel into string")]
    public int CancelWindow;

    [Header("Move (Attack) 'OnHit', 'OnBlock' and 'OnCounter' Stun")]
    public int HitStun;
    public int BlockStun;
    public int CounterStun;
}