using System;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class InvisableStartScene : Singleton<InvisableStartScene>
{
    public int windowsOriginWidth = 800;
    public int windowsOriginHeight = 630;

    public bool isThrough;
    //[Required][SerializeField] private WindowsAttributeController _winController;
    public IWindowsAttributeChange winController => WindowsAttributeController.Instance.IChange;
    
    IntPtr hWnd;

    #region Win函数常量

    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetSystemMetrics(int nIndex);
    private static int SM_CXSCREEN = 0; //主屏幕分辨率宽度
    private static int SM_CYSCREEN = 1; //主屏幕分辨率高度
    private static int SM_CYCAPTION = 4; //标题栏高度
    private static int SM_CXFULLSCREEN = 16; //最大化窗口宽度（减去任务栏）
    private static int SM_CYFULLSCREEN = 17; //最大化窗口高度（减去任务栏）

    
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

    [DllImport("user32.dll")]
    static extern int SetLayeredWindowAttributes(IntPtr hwnd, int crKey, int bAlpha, int dwFlags);

    [DllImport("Dwmapi.dll")]
    static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    //private const int WS_POPUP = 0x800000;
    private const int GWL_EXSTYLE = -20;
    private const int GWL_STYLE = -16;
    private const int WS_EX_LAYERED = 0x00080000;
    private const int WS_BORDER = 0x00800000;
    private const int WS_CAPTION = 0x00C00000;
    private const int SWP_SHOWWINDOW = 0x0040;
    private const int LWA_COLORKEY = 0x1;
    private const int LWA_ALPHA = 0x00000002;
    private const int WS_EX_TRANSPARENT = 0x20;
    private const int WS_EX_TOOLWINDOW = 0x00000080;
    private const int WS_SYSMENU = 0x00080000;
    private const int WS_MINIMIZEBOX = 0x00020000;
    private const int WS_MAXIMIZEBOX = 0x00010000;

    private const int SWP_NOMOVE = 0x0002;
    private const int SWP_NOSIZE = 0x0001;
    private const int SWP_FRAMECHANGED = 0x0020;
    const uint SWP_NOCOPYBITS = 0x0100;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
    
    // 导入用户32.dll
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    // 常量定义
    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
    // 窗口矩形结构
    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
    
    #endregion

    // public void SetWindowNoTaskbar()
    // {
    //     // int windowLong = GetWindowLong(hWnd, GWL_EXSTYLE);
    //     // SetWindowLong(hWnd, GWL_EXSTYLE, windowLong | WS_EX_TOOLWINDOW);
    //     winController.SetWindowTaskbar(false);
    //     
    // }
    //
    // public void ResetWindowTaskbar()
    // {
    //     // int windowLong = GetWindowLong(hWnd, GWL_EXSTYLE);
    //     // SetWindowLong(hWnd, GWL_EXSTYLE, windowLong & ~WS_EX_TOOLWINDOW);
    //     winController.SetWindowTaskbar(true);
    // }

    [SerializeField] private Camera nowCamera;

    protected override async void Awake()
    {
        base.Awake();
        hWnd = GetActiveWindow();
        DontDestroyOnLoad(gameObject);
  
        
        nowCamera.clearFlags = CameraClearFlags.SolidColor;
        nowCamera.backgroundColor = new Color(0, 0, 0, 0);
        //await UniTask.Yield();
        await UniTask.Delay(10);
        
     
        
        if (!Application.isEditor)
        {
            NetButton.OnClientDone += ShowOnClient;

            // NetButton.OnHostDone += VisibleOnHost;
            NetButton.OnHostDone += async () =>
            {
                await UniTask.Delay(300);
                SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
                // SetLayeredWindowAttributes(hWnd, 0x00000000, 0, LWA_COLORKEY);
                SetLayeredWindowAttributes(hWnd, 0, 0, LWA_ALPHA);
                await UniTask.Delay(300);
                winController.SetWindowsHide(true);
            };
            
            winController.SetWindowsHide(false);
            await UniTask.Yield();
            winController.SetWindowsPositionToCenter();
            // HideOnStart();
            // winController.SetWindowsHide(true);

      
        }
        else
        {
            Resolution resolutions = Screen.currentResolution;
            Debug.Log($"{resolutions.width}，{resolutions.height}");
            Debug.Log("InvisableStartScene");
        }

        
    }
    
    private void Update()
    {
// #if !UNITY_EDITOR
//         if (Input.GetKeyDown(KeyCode.Space))
//         {
//             SetWindowLong(hWnd, GWL_EXSTYLE, ~WS_EX_LAYERED & ~WS_EX_TRANSPARENT);
//             SetLayeredWindowAttributes(hWnd, 0, 255, LWA_ALPHA);
//             SetLayeredWindowAttributes(hWnd, 0x00000000, 0, LWA_COLORKEY);
//         }
//
//         if (Input.GetKeyDown(KeyCode.Keypad1))
//         {
//             SetWindowNoTaskbar();
//         }
//         if (Input.GetKeyDown(KeyCode.Keypad2))
//         {
//             ResetWindowTaskbar();
//         }
// #endif
        // if (Input.GetKeyDown(KeyCode.V))
        //  {
        //      winController.SetWindowsHide(false);
        //  }
        //  if (Input.GetKeyDown(KeyCode.B))
        //  {
        //      winController.SetWindowsHide(true);
        //  }
    }
    public void HideOnStart()
    {
        
        Screen.fullScreen = false;
        winController.SetWindowsPositionToCenter();
        
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;                
        Screen.fullScreen = true;
        
        SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
        // SetLayeredWindowAttributes(hWnd, 0x00000000, 0, LWA_COLORKEY);
        SetLayeredWindowAttributes(hWnd, 0, 0, LWA_ALPHA);
            
        //测试
        winController.SetWindowTaskbar(false);
            
        // winController.SetWinodwsOrder(false,true);
            
        // await UniTask.Delay(1000);
    }

    public async void ShowOnClient()
    {
        winController.SetWindowTaskbar(true);
        Screen.SetResolution(800, 600, false);
                
        Screen.fullScreen = false;
        SetWindowLong(hWnd, GWL_EXSTYLE, ~WS_EX_LAYERED & ~WS_EX_TRANSPARENT);
        SetLayeredWindowAttributes(hWnd, 0, 255, LWA_ALPHA);
        // SetLayeredWindowAttributes(hWnd, 0x00000000, 0, LWA_COLORKEY);
        await UniTask.Yield();


        winController.ChangeWindowsSize(windowsOriginWidth, windowsOriginHeight);

        // winController.SetWinodwsOrder(true,true);
                
        winController.SetWindowsPositionToCenter();
                
        // winController.SetWinodwsOrder(false,false);
        await UniTask.Delay(200);
        winController.SetWinodwsOrder(true,false);
        
    }

    public async void VisibleOnHost()
    {
        if (Application.isEditor)
        {
            print("无形窗口");
            return;
        }

        if (!NetworkManager.Singleton.IsHost)
        {
            Debug.Log("非host调用");
            return;
        }
        
        //绿幕测试
        // nowCamera.clearFlags = CameraClearFlags.SolidColor;
        // nowCamera.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0);
        winController.SetWindowsHide(false);
        await UniTask.Yield();
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        Screen.fullScreen = true;
    
        // winController.SetWindowTaskbar(false);
        // int intExTemp = GetWindowLong(hWnd, GWL_EXSTYLE);
        // SetWindowLong(hWnd, GWL_EXSTYLE, intExTemp | WS_EX_TRANSPARENT | WS_EX_LAYERED);
        // SetWindowLong(hWnd, GWL_STYLE, GetWindowLong(hWnd, GWL_STYLE) & ~WS_BORDER & ~WS_CAPTION);
        // winController.SetWinodwsOrder(true,true);
        //
        // SetLayeredWindowAttributes(hWnd, 0x00000000, 0, LWA_COLORKEY);
        //
        // SetWindowPos(hWnd, new IntPtr(-1), 0, 0, Screen.currentResolution.width,
        //     Screen.currentResolution.height, SWP_SHOWWINDOW);
        // var margins = new MARGINS() { cxLeftWidth = -1 };
        // DwmExtendFrameIntoClientArea(hWnd, ref margins);
        // TODO:HDR关了，可能可用了
        await UniTask.Delay(100);

        // //获取设置当前屏幕分辩率 
        Resolution resolutions = Screen.currentResolution;

        int x = GetSystemMetrics(SM_CXSCREEN);
        int y = GetSystemMetrics(SM_CYSCREEN);

        // //设置当前分辨率 
        //Screen.SetResolution(2560, 1600, true);
        Screen.SetResolution(x, y, true);


        //Screen.fullScreen = true;  //设置成全屏
        await UniTask.Delay(100);

        var margins = new MARGINS() { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(hWnd, ref margins);

        if (isThrough)
        {
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_TRANSPARENT | WS_EX_LAYERED);
        }
        else
        {
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
        }

        // SetWindowLong(hWnd, GWL_STYLE, GetWindowLong(hWnd, GWL_STYLE) & ~WS_BORDER & ~WS_CAPTION);
        winController.SetWindowTaskbar(false);

        // SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_SHOWWINDOW | SWP_NOMOVE | SWP_NOSIZE);

        SetLayeredWindowAttributes(hWnd, 0x007F7F7F, 0, LWA_COLORKEY);
        // SetLayeredWindowAttributes(hWnd, 0x00000000, 0, LWA_COLORKEY);
        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, resolutions.width,
            resolutions.height, SWP_NOMOVE);


        // int style = GetWindowLong(hWnd, GWL_STYLE);
        // style &= ~(WS_BORDER | WS_CAPTION | WS_SYSMENU | WS_MINIMIZEBOX | WS_MAXIMIZEBOX);
        // SetWindowLong(hWnd, GWL_STYLE, style);
        //
        // SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_FRAMECHANGED);

        // await UniTask.Delay(4000);
        // SetForegroundWindow(hWnd);
    }
}