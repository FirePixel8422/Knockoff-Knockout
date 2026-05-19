using UnityEngine;


/// <summary>
/// Animation data container struct, holding animHash, blendIn and blendOut transitionFrameCount.
/// </summary>
[System.Serializable]
public struct AnimData
{
    [field: SerializeField]
    public int Hash { get; private set; }

    [field: SerializeField]
    public int BlendIn { get; private set; }

    [field: SerializeField]
    public int BlendOut { get; private set; }

    public AnimData(string animName, int blendIn, int blendOut)
    {
        Hash = Animator.StringToHash(animName);

        BlendIn = blendIn;
        BlendOut = blendOut;
    }
}