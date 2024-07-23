using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

// 由netdataManage激活 每个客户端单独独立 
public class OneSceneLevelManager : Singleton<OneSceneLevelManager>
{
    // public List<LevelDataSO> allLevelData; //不用
    public List<LevelDataStateFullData> levelDataStateFull;
    public GameObject nowActiveLevelGameObject;
    public Vector3 startPosition; //默认为0,0,0
    public NetworkObject nowActivePlayer;
    
    [Serializable]
    public class LevelDataStateFullData
    {
        public LevelState nowState;
        public List<LevelDataSO> allLevelDataInState;
        public LevelDataSO RepeatWhenMore;
        [AssetsOnly] public NetworkObject newPlayer;
    }
    public async void LevelSwitch(LevelState state,int levelIndex = 0,bool justCleraActiveGO = false)
    {
        if (justCleraActiveGO)
        {
            if(nowActiveLevelGameObject!=null){Destroy(nowActiveLevelGameObject);}
            return;
        }
        if(state == LevelState.empty) return;


        int nowGameID = NetdataManager.instance.GameId.Value;
        GameObject newLeveLObject = null;
        NetworkObject newPlayerObject = null;
        // foreach (var so in allLevelData)
        // {
        //     if (so.levelData.gameId == nowGameID && so.levelData.levelState == state && so.levelData.smallLevelIndex == levelIndex)
        //     {
        //         newLeveLObject = so.levelData.levelPrefab;
        //         break;
        //     }
        // }

        foreach (var stateFull in levelDataStateFull)
        {
            if (stateFull.nowState == state)
            {
                if (nowGameID!=0)
                {
                    newLeveLObject = stateFull.RepeatWhenMore.levelData.levelPrefab;
                }
                else
                {
                    newPlayerObject = stateFull.newPlayer;
                }
                foreach (var so in stateFull.allLevelDataInState)
                {
                    if (so.levelData.gameId == nowGameID && so.levelData.levelState == state && so.levelData.smallLevelIndex == levelIndex)
                    {
                        newLeveLObject = so.levelData.levelPrefab;
                        break;
                    }
                }
                break;
            }
        }

        if (newLeveLObject != null)
        {
            if(nowActiveLevelGameObject!=null){Destroy(nowActiveLevelGameObject);}
            nowActiveLevelGameObject = Instantiate(newLeveLObject, Vector3.zero + startPosition,quaternion.identity);
        }
        
        if (nowGameID == 0)
        {
            ulong newOwnerID = 1;
            Vector3 newPlayerPos = new Vector3(-100, 0, 0);
            if (nowActivePlayer != null)
            {
                newPlayerPos = nowActivePlayer.transform.position;
                newOwnerID = nowActivePlayer.OwnerClientId;
                nowActivePlayer.Despawn(true);
            }

            if (newPlayerObject != null)
            {
               nowActivePlayer = Instantiate(newPlayerObject,newPlayerPos,Quaternion.identity);
               nowActivePlayer.Spawn(false);
               nowActivePlayer.ChangeOwnership(newOwnerID);
            }
        }
    }

[Serializable]
    public class LevelPrefabsData
    {
        [AssetsOnly]
        public GameObject levelPrefab;
        public int gameId;
        public LevelState levelState;//也许换成字符串
        public int smallLevelIndex; //关卡内不同场景的索引
    }
    
}

//整体的大关切换 可用于跳关部分
public enum LevelState
{
    start_SingleWindow = 0,
    moreThanOneWindow = 1,
    blackAndWhite = 2,
    respawn = 3,
    resize = 4,
    newWorld = 5,
    TestFinal = 6,
    empty = -1,
}