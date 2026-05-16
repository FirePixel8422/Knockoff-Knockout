using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Sub HUDManager system handler class that is responsible for updating the player healthbar UI.
/// </summary>
[System.Serializable]
public class HealthBarController
{
    [SerializeField] private Image healthBar, comboDamageBar;
    [SerializeField] private float healthLerp;
    [SerializeField] private float comboDelay, comboLerp;

    [EditorReadOnly, SerializeField] private float maxHealth;
    [EditorReadOnly, SerializeField] private float health;
    public float Health => health;

    private float applyComboDamageBarTime;
    private float appliedHealth;



    public void Init()
    {
        maxHealth = GameRules.CombatSettings.Fighter.StartHealth;
        health = maxHealth;
    }
    public void TakeDamage(float damage)
    {
        health = Mathf.Clamp(health - damage, 0, maxHealth);
        applyComboDamageBarTime = Time.time + comboDelay;
    }

    public void OnUpdate(float deltaTime, float globalTime)
    {
        float healthBar01 = Mathf.Lerp(healthBar.fillAmount, health / maxHealth, healthLerp * deltaTime);
        healthBar.fillAmount = healthBar01;

        if (globalTime > applyComboDamageBarTime)
        {
            appliedHealth = health;
        }

        float comboDamageBar01 = Mathf.Lerp(comboDamageBar.fillAmount, appliedHealth / maxHealth, comboLerp * deltaTime);
        comboDamageBar.fillAmount = comboDamageBar01;
    }

#if UNITY_EDITOR
    [InspectorButton("Take 30 Damage")]
    public void Take30Damage()
    {
        TakeDamage(30);
    }
#endif
}