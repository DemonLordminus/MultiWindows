using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelGroupData", menuName = "Level/New LevelGroupData")]
public class LevelGroupDataSO : ScriptableObject
{
    public LevelState nowState;
    public List<LevelDataSO> allLevelDataInState;
    public LevelDataSO RepeatWhenMore;
    [AssetsOnly] public NetworkObject newPlayer;
}
