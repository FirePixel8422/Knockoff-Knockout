using UnityEngine;

public class PauseMenuController : UpdateMonoBehaviour
{
    public static PauseMenuController Instance { get; private set; }


    [SerializeField] private Vector3 enabledPos, disabledPos;
    [SerializeField] private float animateLerpSpeed;
        
    private Vector3LerpState positionState;
    private bool isEnabled;



    private void Awake()
    {
        Instance = this;
        positionState = new Vector3LerpState(disabledPos);
    }

    public void Continue()
    {
        if (isEnabled) return;

        Disable();
    }
    public void Restart()
    {
        if (isEnabled) return;

        Disable();
    }

    [InspectorButton("Enable")]
    public void Enable()
    {
        isEnabled = true;
        positionState.Target = enabledPos;

        gameObject.SetActive(true);
    }
    [InspectorButton("Disable")]
    public void Disable()
    {
        isEnabled = false;
        positionState.Target = disabledPos;
    }

    protected override void OnUpdate()
    {
        if (positionState.IsCompleted(0.01f))
        {
            if (!isEnabled)
            {
                gameObject.SetActive(false);
            }
            return;
        }

        transform.localPosition = positionState.Lerp(animateLerpSpeed * Time.deltaTime);
    }
}
