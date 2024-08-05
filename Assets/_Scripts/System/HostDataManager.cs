using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class HostDataManager : NetworkBehaviour
{
    public Transform startPoint;
    public NetworkObject player;    
    // public string[] sceneNames;
    private static HostDataManager instance;

    [SerializeField] private NetworkVariable<StageCurtainState> _nowGameState = new NetworkVariable<StageCurtainState>(StageCurtainState.empty,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<float> OriginOrthoSize = new NetworkVariable<float>(10f,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<float> rateBetweenWindowsToGame = new NetworkVariable<float>(0.0341f,
    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public float rate =>  0.03435f * OriginOrthoSize.Value / 10f;


    public StageCurtainState nowStageCurtainState
    {
        set
        {
            _nowGameState.Value = value;
            if (value == StageCurtainState.hideCurtain)
            {
                Debug.Log("隐藏大窗口");
                SpecialEffectManager.Instance.HideCurtain();
            }
            else if (value == StageCurtainState.showCurtain)
            {
                Debug.Log("显示大窗口");
                SpecialEffectManager.Instance.ShowCurtain();
            }
        }
        get
        {
            return _nowGameState.Value;
        }
    }
    #region 单例

    public static HostDataManager Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }
            else 
            {
                if(!Application.isEditor)
                    Debug.LogWarningFormat("No Instance {0} (Stacktrace: {1})", typeof(HostDataManager), Environment.StackTrace);
                return instance;
            }
        }
    }


    protected virtual void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarningFormat("Duplicate {0} detected. Destroying new instance.", typeof(HostDataManager));
            Destroy(gameObject);
        }
        else
        {
            instance = (HostDataManager)this;
        }
    }
    public static bool IsInitiailzed
    {
        get { return instance != null; }
    }
    protected virtual void OnDestory()
    {
        if(instance == this)
        {
            instance = null;
        }
    }

    #endregion
    
}
public enum StageCurtainState
{
    empty,
    [InspectorName("隐藏大窗口")]
    hideCurtain,
    [InspectorName("显示大窗口")]
    showCurtain,
}