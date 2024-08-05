using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class ControllerWithWindowPosition : MonoBehaviour
{
    [SerializeField] WindowPositionGetter positionGetter;
    [SerializeField] Transform CameraRoot;
    [SerializeField] float rateBetweenWindowsToGame;//1对应0.00342 100对应0.0343
    public Vector2 offset;
    [SerializeField] float rateBetweenWindowsToGameX, rateBetweenWindowsToGameY;
    bool isFocus = false;
    private void Start()
    {
        // CameraUpdate();
        //InvokeRepeating("CameraUpdate",0.5f,0.05f);
        // NewCameraUpdate();
        // HostDataManager.Instance.rateBetweenWindowsToGame.OnValueChanged += OnHostRateChange;
        HostDataManager.Instance.OriginOrthoSize.OnValueChanged += OnHostCameraSizeUpdate;
    }
    
    public void CameraUpdate(bool isOnlyOnce = false)
    {
    //     //Debug.Log("test");
    //
    //     CameraRoot.transform.position = positionGetter.GetWindowPosition() * rateBetweenWindowsToGame + (Vector2)HostDataManager.Instance.startPoint.position;
    //     //CameraRoot.transform.position = SystemManager.Instance.cameraGroup.nowCamera.cameraToWorldMatrix
    //     //var pos = positionGetter.GetWindowPosition();
    //     //CameraRoot.transform.position =  new Vector2(pos.x * rateBetweenWindowsToGameX, pos.y * rateBetweenWindowsToGameY);
    //     //CameraUpdate();   
    //     if (!isFocus || isOnlyOnce)
    //     {
            return;
    //     }
    //     await Task.Delay(10);
    //     CameraUpdate(isOnlyOnce);
    }
    private void Update()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            Vector2 posWithRate = positionGetter.GetWindowPosition() * new Vector2(rateBetweenWindowsToGame,rateBetweenWindowsToGame);
            CameraRoot.transform.position = posWithRate + (Vector2)HostDataManager.Instance.startPoint.position + offset;
        }
        else
        {
            CameraRoot.transform.position = (Vector2)HostDataManager.Instance.startPoint.position + offset*2;
        }
    }
    // private async void NewCameraUpdate()
    // {
    //     Vector2 posWithRate = positionGetter.GetWindowPosition() * new Vector2(rateBetweenWindowsToGameX,rateBetweenWindowsToGameY);
    //     // 考虑加入检测，相同时不执行移动
    //     await Task.Delay(50);
    //     CameraRoot.transform.position = posWithRate + (Vector2)HostDataManager.Instance.startPoint.position;
    //     NewCameraUpdate();
    // }

    // public void ChangeRate(float rate)
    // {
    //     rateBetweenWindowsToGame = rate;
    // }
    public void ChangeRate(float rateX,float rateY)
    {
        rateBetweenWindowsToGameX = rateX;
        rateBetweenWindowsToGameY = rateY;
    }
    public void OnHostRateChange(float previous,float current)
    {
        rateBetweenWindowsToGame = current;
    }
    public void OnHostCameraSizeUpdate(float previous,float current)
    {
        rateBetweenWindowsToGame = 0.03435f * current / 10f;
    }

    private void OnApplicationFocus(bool focus)
    {
        isFocus = focus;
        //if(focus)
        //{
        //    CameraUpdate();
        //}
    }
}
