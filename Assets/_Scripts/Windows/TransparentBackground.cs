using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;


public class TransparentBackground : MonoBehaviour
{
    private int currentX;
    private int currentY;

    #region Win函数常量

    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

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


    #endregion

    IntPtr hWnd;

    void Awake()
    {
        
    
#if UNITY_EDITOR
        print("unity内运行程序");
#else
    Camera nowCamera = Camera.main;
    nowCamera.clearFlags = CameraClearFlags.SolidColor;
    nowCamera.backgroundColor = Color.black;

    hWnd = GetActiveWindow();
    // int intExTemp = GetWindowLong(hWnd, GWL_EXSTYLE);
    // 保留窗口的边框和标题栏
    // SetWindowLong(hWnd, GWL_EXSTYLE, intExTemp | WS_EX_TRANSPARENT | WS_EX_LAYERED);
    // SetWindowLong(hWnd, GWL_STYLE, GetWindowLong(hWnd, GWL_STYLE));


    // 设置窗口位置和大小，这里可以根据需要调整X, Y, width, height
    //SetWindowPos(hWnd, -1, 100, 100, 800, 600, SWP_SHOWWINDOW);
    
    // SetLayeredWindowAttributes(hWnd, 0x00000000, 0, LWA_COLORKEY);
    

    // var margins = new MARGINS() { cxLeftWidth = -1 };
    // DwmExtendFrameIntoClientArea(hWnd, ref margins);
    ApplyWindowSettings();
#endif
        
    }
    
    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr CreateWindowEx(
        int dwExStyle, string lpClassName, string lpWindowName, int dwStyle,
        int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu,
        IntPtr hInstance, IntPtr lpParam);

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool DestroyWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    static extern bool SetForegroundWindow(IntPtr hWnd);
    
    public static void ForceLoseFocus(IntPtr currentWindowHandle)
    {
        // 创建一个临时窗口
        IntPtr tempWindow = CreateWindowEx(0, "Message", null, 0, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        
        // 将焦点设置到临时窗口
        SetForegroundWindow(tempWindow);

        // 销毁临时窗口
        DestroyWindow(tempWindow);
    }

    private bool isDragNow = false;
#if !UNITY_EDITOR
    void ApplyWindowSettings()
    {
        SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
        SetLayeredWindowAttributes(hWnd, 0x00000000, 0, LWA_COLORKEY);
    }
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            isDragNow = true;
        }
    }
    private void OnMouseUp()
    {
        if(isDragNow)
            ForceLoseFocus(hWnd);
        isDragNow = false;
    }
#endif

    // [DllImport("user32.dll")]
    // public static extern short GetAsyncKeyState(int vKey);
    // private const int VK_LBUTTON = 0x01; //鼠标左键
    // private const int VK_RBUTTON = 0x02; //鼠标右键
    // private const int VK_MBUTTON = 0x04; //鼠标中键
    //
    // private bool _isLeftDown;
    // private async void Update()
    // {
    //     if (GetAsyncKeyState(VK_LBUTTON) != 0)
    //     {
    //         if (!_isLeftDown)
    //         {
    //             _isLeftDown = true;
    //         }
    //
    //     }
    //     
    //     if (GetAsyncKeyState(VK_LBUTTON) == 0 && _isLeftDown)
    //     {
    //         _isLeftDown = false;
    //         if (isDragNow)
    //         {
    //             isDragNow = false;
    //             await Task.Delay(10);
    //             ForceLoseFocus(hWnd);
    //         }
    //     }
    // }
}



