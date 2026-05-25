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

    private FloatLerpState health01;
    private FloatLerpState combo01;

    private float applyComboDamageBarTime;

    private bool isIdle;
    private bool isComboApplied;
    private const float EPSILON = 0.001f;



    public void Init(float startHealth)
    {
        maxHealth = startHealth;

        health01 = new FloatLerpState(1, 1);
        combo01 = new FloatLerpState(1, 1);
    }

    public void OnHealthChanged(float newHealth)
    {
        health01.Target = newHealth / maxHealth;
        applyComboDamageBarTime = Time.time + comboDelay;

        isComboApplied = false;
        isIdle = false;
    }

    public void OnUpdate(float deltaTime, float globalTime)
    {
        if (isIdle) return;

        healthBar.fillAmount = health01.Lerp(healthLerp * deltaTime);

        if (!isComboApplied && globalTime > applyComboDamageBarTime)
        {
            combo01.Target = health01.Current;
            isComboApplied = true;
        }

        comboDamageBar.fillAmount = combo01.Lerp(comboLerp * deltaTime);

        isIdle = isComboApplied && health01.IsCompleted(EPSILON) && combo01.IsCompleted(EPSILON);
    }
}