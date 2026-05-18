using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Sub HUDManager system handler class that is responsible for updating the player healthbar UI.
/// </summary>
[System.Serializable]
public class HealthBarUIController
{
    [SerializeField] private Image healthBar, comboDamageBar;
    [SerializeField] private float healthLerp;
    [SerializeField] private float comboDelay, comboLerp;

    [EditorReadOnly, SerializeField] private float maxHealth;
    [EditorReadOnly, SerializeField] private float health;

    private float applyComboDamageBarTime;
    private float appliedHealth;



    public void Init(float startHealth)
    {
        maxHealth = startHealth;
        health = startHealth;
    }

    public void OnHealthChanged(float newHealth)
    {
        health = newHealth;
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
}