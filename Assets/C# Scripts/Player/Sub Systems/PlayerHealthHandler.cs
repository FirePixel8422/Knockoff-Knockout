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


    public PlayerHealthHandler(ref Action<float> onDamageTaken, bool isLeftPlayer)
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


    public void TakeDamage(float damage)
    {
        health = math.max(0, health - damage);
        OnHealthChanged?.Invoke(health);

        if (health > 0) return;

        OnFighterDied?.Invoke(isLeftPlayer);

        health = GameRules.CombatSettings.Fighter.StartHealth;
        OnHealthChanged?.Invoke(health);
    }
}