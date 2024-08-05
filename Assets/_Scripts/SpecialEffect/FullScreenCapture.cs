using System;
using System.Diagnostics;
using UnityEngine;
using System.Drawing; // 需要添加对System.Drawing的引用
using System.Drawing.Imaging;
using System.IO;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Debug = UnityEngine.Debug;
using Graphics = System.Drawing.Graphics;

public class FullScreenCapture : MonoBehaviour
{
    private async void Start()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            return;
        }
        await UniTask.Delay(5000);
        CaptureFullScreen();
        Debug.Log("照片保存");
        
        OpenFolder(Path.Combine(Application.persistentDataPath, "full_screenshot.png"));
    }

    public void CaptureFullScreen()
    {
        // 创建一个Bitmap对象
        using (Bitmap bitmap = new Bitmap(Screen.currentResolution.width, Screen.currentResolution.height))
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // 从屏幕复制图像
                g.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
            }

            // 保存为PNG格式
            string filePath = Path.Combine(Application.persistentDataPath, "full_screenshot.png");
            bitmap.Save(filePath, ImageFormat.Png);
        }
    }
    private void OpenFolder(string filePath)
    {
        string directory = Path.GetDirectoryName(filePath);
        Process.Start("explorer.exe", directory); // Windows平台
    }
}
