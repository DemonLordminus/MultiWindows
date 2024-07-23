using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class CameraBlackBackGrooundSet : MonoBehaviour
{
    [Required] [SerializeField] private Camera nowCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            nowCamera.clearFlags = CameraClearFlags.SolidColor;
            nowCamera.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0);
            
        }
        else
        {
            nowCamera.clearFlags = CameraClearFlags.SolidColor;
            nowCamera.backgroundColor = new Color(0, 0, 0, 0);
        }
    }


}
