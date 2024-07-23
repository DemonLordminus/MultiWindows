using System;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class OpenNewApplication : MonoBehaviour
{
    public string applicationPath;
    public static bool isCanNotClose = false;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        NetButton.OnClientDone += () => { isCanNotClose = false; };
        // 获取当前进程
        Process currentProcess = Process.GetCurrentProcess();
        
        // 获取当前进程的可执行文件路径
        applicationPath = currentProcess.MainModule.FileName;
        
        // NetButton.OnHostDone += CreateNewApplication;
        NetdataManager.OnHostManagerInitComplete += CreateNewApplication;
    }

    private void OnDestroy()
    {
        // NetButton.OnHostDone -= CreateNewApplication;
        NetdataManager.OnHostManagerInitComplete -= CreateNewApplication;
    }

    public async void CreateNewApplication()
    {
        if (!Application.isEditor)
        {
            Application.OpenURL(applicationPath);
        }
    }

    private void OnApplicationQuit()
    {
        if (isCanNotClose)
        {
            CreateNewApplication();
        }
    }
}
