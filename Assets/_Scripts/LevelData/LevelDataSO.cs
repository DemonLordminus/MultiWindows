using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level/New LevelData")]
public class LevelDataSO : ScriptableObject
{
    public OneSceneLevelManager.LevelPrefabsData levelData;
}
