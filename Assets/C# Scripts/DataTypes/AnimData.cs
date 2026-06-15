using UnityEngine;


/// <summary>
/// Animation data container struct, holding animHash, allowSelfInterrupt, blendIn and blendOut transitionFrameCount.
/// </summary>
[System.Serializable]
public struct AnimData
{
    public int Hash;
    public bool AllowSelfInterrupt;

    public int BlendIn;
    public int BlendOut;

    public AnimData(string animName, bool allowSelfInterrupt, int blendIn, int blendOut)
    {
        Hash = Animator.StringToHash(animName);
        AllowSelfInterrupt = allowSelfInterrupt;

        BlendIn = blendIn;
        BlendOut = blendOut;

#if UNITY_EDITOR
        Name = animName;
#endif
    }

#if UNITY_EDITOR
    public string Name;
#endif
}