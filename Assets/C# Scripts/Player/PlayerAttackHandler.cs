using UnityEngine;


/// <summary>
/// Sub Player system handler class that is responsible for performinmg and tracking attack states.
/// </summary>
[System.Serializable]
public class PlayerAttackHandler
{
    [SerializeField] private AttackSequence currentSequence;
    [SerializeField] private bool sequenceActive;

    [SerializeField] private AttackData bufferedAttack;
    public void SetBufferedAttack(AttackData newAttack)
    {
        bufferedAttack = newAttack;
    }



    public void OnFrameTick()
    {
        if (!sequenceActive) return;

        currentSequence.OnFrameTick(out bool sequenceFinished);
        if (sequenceFinished)
        {
            currentSequence = new AttackSequence(bufferedAttack);
        }
    }
}