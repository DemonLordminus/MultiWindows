using UnityEngine;

public class ChangePlayerOwnerShip : MonoBehaviour
{
    [SerializeField] private int gameID;

    public void ChangeOwnerShipForPlayer(GameObject player)
    {
        var ownershipChange = player.GetComponent<ChangeOwnerShipInPlayer>();
        ownershipChange.ChangeOwnership(gameID,true);
    }
    public void ChangeOwnerShipForPlayer()
    {
        ChangeOwnerShipForPlayer(HostDataManager.Instance.player.gameObject);

    }
}
