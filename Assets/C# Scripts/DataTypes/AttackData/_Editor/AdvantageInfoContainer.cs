


#if UNITY_EDITOR
[System.Serializable]
public struct AdvantageInfoContainer
{
    [EditorReadOnly] public string AttackDuration;
    [EditorReadOnly] public string OnHit;
    [EditorReadOnly] public string OnBlock;
    [EditorReadOnly] public string OnCounter;
    [EditorReadOnly] public string OnWhiff;
}
#endif