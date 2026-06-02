using System;
using UnityEngine;
using UnityEngine.VFX;


/// <summary>
/// Sub Player system handler class that is responsible for updating fighter related particles based on events
/// </summary>
[System.Serializable]
public class PlayerParticleHandler
{
    [SerializeField] private VisualEffect blockParticle;

    [SerializeField] private VisualEffect highHurtParticle;
    [SerializeField] private VisualEffect lowHurtParticle;


    public void Init(ref Action<AttackData, AttackResult, bool> onAttackConnected)
    {
        onAttackConnected += OnAttackConnected;
    }
    private PlayerParticleHandler() { }

    private void OnAttackConnected(AttackData attack, AttackResult result, bool isDefender)
    {
        if (!isDefender) return;

        if (result == AttackResult.Hit ||
            result == AttackResult.CounterHit ||
            result == AttackResult.KnockDown)
        {
            OnHurt(attack.Level);
        }
        else if (result == AttackResult.StandingBlocked ||
                 result == AttackResult.CrouchBlocked)
        {
            OnBlocked();
        }
    }
    private void OnBlocked()
    {
        blockParticle.Play();
    }
    private void OnHurt(AttackLevel level)
    {
        if (level == AttackLevel.Low)
        {
            lowHurtParticle.Play();
        }
        else
        {
            highHurtParticle.Play();
        }
    }
}