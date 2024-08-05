using Unity.Netcode;
using UnityEngine;

public class ChangeCurtainState : MonoBehaviour
{
    [SerializeField] private StageCurtainState _curtainState;
    // 只在Host上有效
    void Start()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HostDataManager.Instance.nowStageCurtainState = _curtainState;
        }
    }
    
}
