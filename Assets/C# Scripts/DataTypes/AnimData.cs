using UnityEngine;


/// <summary>
/// Animation data container struct, holding animHash, blendIn and blendOut transitionFrameCount.
/// </summary>
[System.Serializable]
public struct AnimData
{
    public int Hash;

    public int BlendIn;
    public int BlendOut;

    public AnimData(string animName, int blendIn, int blendOut)
    {
        Hash = Animator.StringToHash(animName);

        BlendIn = blendIn;
        BlendOut = blendOut;
    }
}