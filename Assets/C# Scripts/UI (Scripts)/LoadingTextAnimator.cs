using Fire_Pixel.Utility;
using TMPro;
using UnityEngine;


public class LoadingTextAnimator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingTextObj;
    [SerializeField] private float animationSpeed = 0.5f;

    private string loadingText;
    private float time;


    private void Start()
    {
        loadingText = loadingTextObj.text;
        time = animationSpeed * 2;
    }

    private void OnEnable() => CallbackScheduler.RegisterUpdate(OnUpdate);
    private void OnDisable() => CallbackScheduler.RegisterUpdate(OnUpdate);


    private void OnUpdate()
    {
        time += Time.deltaTime;

        float dotCount = time / animationSpeed % 4;

        loadingTextObj.text = loadingText + new string('.', (int)dotCount);
    }
}
