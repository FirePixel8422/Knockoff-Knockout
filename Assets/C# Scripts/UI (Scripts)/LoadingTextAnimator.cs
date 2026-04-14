using TMPro;
using UnityEngine;


public class LoadingTextAnimator : UpdateMonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingtext;
    [SerializeField] private float animationSpeed = 0.5f;

    private string loadingText;
    private float time;


    private void Awake()
    {
        loadingText = loadingtext.text;
        time = animationSpeed * 2;
    }


    protected override void OnUpdate()
    {
        time += Time.deltaTime;

        float dotCount = time / animationSpeed % 4;

        loadingtext.text = loadingText + new string('.', (int)dotCount);
    }
}
