using Unity.Netcode;
using UnityEngine;

public class WorldStartPointFollowPlayer : MonoBehaviour
{
    
    // Update is called once per frame
    void Update()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            var player = HostDataManager.Instance.player;
            if (player)
            {
                transform.position = player.transform.position;

            }
        }
    }
}
