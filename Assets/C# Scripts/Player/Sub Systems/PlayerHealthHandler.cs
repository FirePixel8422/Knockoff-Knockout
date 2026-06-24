using Fire_Pixel.Utility;
using System;
using Unity.Mathematics;
using UnityEngine;


[System.Serializable]
public class PlayerHealthHandler
{
    [EditorReadOnly, SerializeField] private float health;
    [EditorReadOnly, SerializeField] private bool isLeftPlayer;

    public Action<float> OnHealthChanged;
    public Action<bool> OnFighterDied;

    public bool isInvulnerable;
    public const float I_FRAME_DURATION = 2;


    public PlayerHealthHandler(ref Func<float, bool> onDamageTaken, bool isLeftPlayer)
    {
        health = GameRules.CombatSettings.Fighter.StartHealth;

        onDamageTaken += TakeDamage;
        this.isLeftPlayer = isLeftPlayer;
    }
    private PlayerHealthHandler() { }
    public void Dispose()
    {
        OnHealthChanged = null;
        OnFighterDied = null;
    }


    public bool TakeDamage(float damage)
    {
        if (isInvulnerable) return false;

        health = math.max(0, health - damage);
        OnHealthChanged?.Invoke(health);

        if (health > 0) return false;

        OnFighterDied?.Invoke(isLeftPlayer);
        isInvulnerable = true;

        CallbackScheduler.Invoke(I_FRAME_DURATION, () =>
        {
            isInvulnerable = false;

            health = GameRules.CombatSettings.Fighter.StartHealth;
            OnHealthChanged?.Invoke(health);
        });

        return true;
    }
}