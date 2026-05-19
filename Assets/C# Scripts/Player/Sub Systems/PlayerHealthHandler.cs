using System;
using Unity.Mathematics;
using UnityEngine;


[System.Serializable]
public class PlayerHealthHandler
{
    [EditorReadOnly, SerializeField] private float health;

    public Action<float> OnHealthChanged;


    public PlayerHealthHandler(ref Action<float> onDamageTaken)
    {
        health = GameRules.CombatSettings.Fighter.StartHealth;

        onDamageTaken += TakeDamage;
    }
    private PlayerHealthHandler() { }


    public void TakeDamage(float damage)
    {
        health = math.max(0, health - damage);
        OnHealthChanged?.Invoke(health);
    }
}