using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;



public class RebindController : MonoBehaviour
{
    [SerializeField] private InputActionReference actionReference;
    [SerializeField] private TextMeshProUGUI rebindKeyText;


    private void Awake()
    {
        GetComponentInChildren<Button>(true).onClick.AddListener(StartRebind);

        RebindManager.PostRebindsLoaded += () =>
        {
            if (ExtensionMethods.TryFindAction(actionReference.asset, actionReference.action.name, out InputAction action))
            {
                rebindKeyText.text = action.GetBindingDisplayString(0);
            }
        };
    }
    public void StartRebind()
    {
        RebindManager.Instance.StartRebind(actionReference.action.name, actionReference.action.bindings[0].id.ToString(), rebindKeyText);
    }
}