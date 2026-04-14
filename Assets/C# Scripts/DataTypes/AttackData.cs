using Unity.Mathematics;



[System.Serializable]
public struct AttackData
{
#if UNITY_EDITOR
    public string Name;
#endif

    public float Damage;
    public float Knockack;

    public AttackType Type;
    public FrameData FrameData;

    public AttackSO Combo;
}

[System.Serializable]
public struct FrameData
{
    public int StartUpFrames;
    public int ActiveFrames;
    public int RecoveryFrames;
    public int2 StunFrames;
}