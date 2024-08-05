using System;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class WindowResize : MonoBehaviour
{
    public static event Action<Vector2> onWindowsResize; 
    
    [Header("预设值")]
    [Tooltip("用于设定多次屏幕大小切换的冷却时间")]
    [Range(0, 1f)]
    public float resizeTimeOut = 0.5f;

    private float lastResizeTime = 0f;
    [Required][SerializeField] private CameraGroup _cameraGroup;
    private void Awake()
    {
        lastResizeTime = Time.time;
    }
    
 
    private void OnRectTransformDimensionsChange()
    {
        if (Time.time - lastResizeTime >= resizeTimeOut)
        {
            //参数设置
            lastResizeTime = Time.time;
 
            //回调方法
            _cameraGroup.AutoSetCameraOrthographic();
            
            //打印日志
            // Debug.Log($"屏幕尺寸更新为{Screen.width}*{Screen.height}");
            
            onWindowsResize?.Invoke(new Vector2(Screen.width,Screen.height));
        }

    }
    
}
