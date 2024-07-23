using UnityEngine;

public class ChangeLevelState : MonoBehaviour
{
    [SerializeField] private LevelState _nextLevelState;
    public void ChangeLevelTo()
    {
        Debug.Log($"ChangeLevelTo{_nextLevelState}");
        NetdataManager.host.LevelStateChange(_nextLevelState);
    }
}
