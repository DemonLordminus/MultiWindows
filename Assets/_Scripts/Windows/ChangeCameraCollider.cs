using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ChangeCameraCollider : MonoBehaviour
{
    // 用这个处理本地情况
    
    // [SerializeField] private NetworkObject networkCollider;
    // private NetworkObject _networkObjectIns;
    public BoxCollider2D cameraCollider;

    private bool _isRegister = false;

    public static event Action<Vector2> onColliderUpdate;

    public static Action colliderRefresh; 
    // public Image testShowCollider;

    public float sizeRate => HostDataManager.Instance.rate;
    private int titleHeight;
    private async void Start()
    {
        await UniTask.Delay(500);//应该等待连接完成
        //     _networkObjectIns = Instantiate(networkCollider);
        //     _networkObjectIns.SpawnWithOwnership();
        titleHeight = WindowsAttributeController.Instance.IChange.GetTitleHeight();;
        if (!NetworkManager.Singleton.IsHost && !Application.isEditor)
        {
            WindowResize.onWindowsResize += UpdateCollider;
            _isRegister = true;
            UpdateCollider(new Vector2(Screen.width, Screen.height));
        }
        else
        {
            cameraCollider.enabled = false;
            // Destroy(testShowCollider.transform.parent.gameObject);
        }

        // cameraCollider.offset = new Vector2(0, titleHeight * sizeRate);
        if (NetworkManager.Singleton.IsHost)
        {
            cameraCollider.enabled = false;
        }

        colliderRefresh += async () =>
        {
            cameraCollider.enabled = false;
            await UniTask.Yield();
            cameraCollider.enabled = true;
        };

    }

    private void OnDestroy()
    {
        if (_isRegister)
        {
            WindowResize.onWindowsResize -= UpdateCollider;
        }
    }


    [Button]
    public void UpdateCollider(Vector2 newRect)
    {
        newRect *= sizeRate;
        // var win = WindowsAttributeController.Instance.IChange.GetNowFrameAttribute();
        cameraCollider.size = newRect;
        onColliderUpdate?.Invoke(newRect);
        // Debug.Log($"相机碰撞体更新为{newRect}");
        
       // testShowCollider.rectTransform.sizeDelta = newRect;
       // testShowCollider.rectTransform.position = cameraCollider.transform.position;
    }

    private async void OnApplicationFocus(bool hasFocus)
    {
        try
        {
            if (hasFocus)
            {
                if (!NetworkManager.Singleton.IsHost && !Application.isEditor)
                {
                    cameraCollider.enabled = false;
                    await UniTask.Yield();
                    cameraCollider.enabled = true;
                }
                else
                {
                    cameraCollider.enabled = false;
                }
            }
        }
        catch (MissingReferenceException e)
        {
            Debug.LogWarning("ChangeCameraCollider启动常规报错");
        }
    }
}
  