using Fire_Pixel.Utility;
using TMPro;
using UnityEngine;


public class MatchStartAnimator : MonoBehaviour
{
    public static MatchStartAnimator Instance { get; private set; }
    private void Awake() => Instance = this;


    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private Transform scalingTransform;

    [SerializeField] private float growScale = 5;
    [SerializeField] private float countDownSpeed = 1;
    [SerializeField] private float startDelay = 0.5f;

    [EditorReadOnly, SerializeField] private float cTimer;

    public const int TIMER_START = 3;



    [InspectorButton("Start")]
    public void StartTimer()
    {
#if UNITY_EDITOR
        DebugLogger.LogWarning("You cant start the sequence unless youre in playmode");
#endif

        cTimer = Time.time + (float)(TIMER_START / countDownSpeed);
        CallbackScheduler.RegisterUpdate(CountDownTimer);
    }
    private void CountDownTimer()
    {
        float timeLeft = cTimer - Time.time;
        float timeLeftCeil = Mathf.Ceil(timeLeft * countDownSpeed);

        numberText.text = timeLeftCeil.ToString();

        float t = timeLeftCeil - (timeLeft * countDownSpeed);
        float scale = Mathf.Lerp(1, growScale, t);

        scalingTransform.localScale = Vector3.one * scale;

        if (timeLeft <= 0)
        {
            numberText.text = "FIGHT";
            CallbackScheduler.UnRegisterUpdate(CountDownTimer);

            CallbackScheduler.Invoke(startDelay, () =>
            {
                numberText.text = "";
            });
        }
    }
}
