using System;
using TMPro;
using UnityEngine;

public class GameExistTest : MonoBehaviour
{
    // 后续改成networkbehaviour测试吧
    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        NetButton.OnClientDone += () => {Destroy(gameObject);}; //没取消订阅 懒得管了
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalInput.GetKeyDown(GlobalKeyCode.D) || Input.GetKeyDown(KeyCode.D))
        {
            text.gameObject.SetActive(true);
            //Destroy(this);
            
        }

       
    }
}
