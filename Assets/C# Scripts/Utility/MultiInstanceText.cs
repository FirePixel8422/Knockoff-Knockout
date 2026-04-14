using UnityEngine;
using TMPro;


public class MultiInstanceText : MultiInstanceBehaviour<MultiInstanceText>
{
    [HideInInspector] public TextMeshProUGUI Text;


    protected override void Awake()
    {
        base.Awake();
        Text = GetComponent<TextMeshProUGUI>();
    }
}