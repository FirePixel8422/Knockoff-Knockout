using UnityEngine;


public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
    }
}
