 using Sirenix.OdinInspector;
 using Unity.Netcode;
using UnityEngine;

public class SpecialEffectManager : Singleton<SpecialEffectManager>
{

    private InvisableStartScene _invisableStartScene = InvisableStartScene.Instance;
    
    public void HideCurtain()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            WindowsAttributeController.Instance.IChange.SetWindowsHide(true);
        }
        
    }

    public void ShowCurtain()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            // WindowsAttributeController.Instance.IChange.SetWindowsHide(false);
            _invisableStartScene.VisibleOnHost();
        }
    }
    
    //
    // public void MakeHostPlayerSpriteVisible(bool isVisible)
    // {
    //     _changeOwnerShipInPlayer.SetSpriteRenderVisible(isVisible);
    // }
    //
    // public void MakeHostPlayerNoMove(bool isNoMove)
    // {
    //     if (isNoMove)
    //     {
    //         playerRb2D.constraints = RigidbodyConstraints2D.FreezePositionX;
    //         playerRb2D.constraints = RigidbodyConstraints2D.FreezePositionY;
    //     }
    //     else
    //     {
    //         playerRb2D.constraints = ~RigidbodyConstraints2D.FreezePositionX;
    //         playerRb2D.constraints = ~RigidbodyConstraints2D.FreezePositionY;
    //     }
    // }
    public void WindowResizeSwitch(bool isCanResize)
    {
        WindowsAttributeController.Instance.IChange.SetWindowResizeMode(isCanResize);
    }

    // public void ChangePlayerOwnerShip(int gameID)
    // {
    //     var targetClientID = NetIdManager.Instance.idRefrenceClientID[gameID];
    //     _changeOwnerShipInPlayer.ChangeOwnership(targetClientID);
    // }

    public NetdataManager FindNetDataManager(int gameID)
    {
        var dataManagers =
            NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(
                NetIdManager.Instance.idRefrenceClientID[gameID]);
        return dataManagers.GetComponent<NetdataManager>();
    }

}
