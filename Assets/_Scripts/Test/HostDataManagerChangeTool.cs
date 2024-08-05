using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class HostDataManagerChangeTool : MonoBehaviour
{
    public bool _isHost
    {
        get
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                return false;
            }
            return NetworkManager.Singleton.IsHost;;
        }
    }
    // [ShowIf("_isHost",true)]
    // [Button]
    // public void ChangeRate(float newValue = 0.0341f)
    // {
    //     HostDataManager.Instance.rateBetweenWindowsToGame.Value = newValue;
    // }
    [ShowIf("_isHost",true)]
    [Button]
    public void ChangeOrthoSize(float newValue = 10f)
    {
        HostDataManager.Instance.OriginOrthoSize.Value = newValue;
    }

}
