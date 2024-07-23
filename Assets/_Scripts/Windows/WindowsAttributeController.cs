using System;
using System.Runtime.InteropServices;
using UnityEngine;


// 交由单例进行管理
// 包装了用于控制窗口的相关函数，比如位置、大小、边框等等
// 父对象应该为不销毁的类型
public class WindowsAttributeController : Singleton<WindowsAttributeController>
{
    public IWindowsAttributeChange IChange
    {
        get
        {
            if (_IChange == null)
            {
                // IChange = Application.isEditor
                //     ? new WindowAttributeControllerEditorMode()
                //     : new WindowAttributeControllerPlayMode();
#if UNITY_EDITOR
                _IChange = new WindowAttributeControllerEditorMode();
                Debug.Log("WindowAttributeController:Editor模式启动");
#else
                _IChange = new WindowAttributeControllerPlayMode();
#endif
            }

            return _IChange;
        }
    }
    private IWindowsAttributeChange _IChange;
//     private void Awake()
//     {
// //         // IChange = Application.isEditor
// //         //     ? new WindowAttributeControllerEditorMode()
// //         //     : new WindowAttributeControllerPlayMode();
// // #if UNITY_EDITOR
// //         IChange = new WindowAttributeControllerEditorMode();
// //         Debug.Log("WindowAttributeController:Editor模式启动");
// // #else
// //         IChange = new WindowAttributeControllerPlayMode();
// // #endif
//     }

    
    // 临时测试代码
    private Vector2Int offset;
    private void Update()
    {
        bool isPress = false;
        int up = 0, left = 0;
        int moveRate = 5;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            isPress = true;
            up = -moveRate;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            isPress = true;
            up = moveRate;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            isPress = true;
            left = -moveRate;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            isPress = true;
            left = moveRate;
        }

        if (isPress)
        {
            IChange.AdjustWindowRelativePosition(left, up);
        }
        
        if (!Application.isEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                CursorPoint cp;
                GetCursorPos(out cp);
                FrameAttribute win = IChange.GetNowFrameAttribute();
                offset = (new Vector2Int(cp.X - win.posX, cp.Y - win.posY));
            }
            if (Input.GetMouseButton(0))
            {
                CursorPoint cp;
                GetCursorPos(out cp);
                IChange.ChanageWindowsPos(cp.X - offset.x,cp.Y - offset.y);
            }
        }
    }
    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out CursorPoint lpPoint);
     
    [StructLayout(LayoutKind.Sequential)]
    public struct CursorPoint
    {
        public int X;
        public int Y;
        public CursorPoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }


}

 



// 采用接口，用于editor模式
public interface IWindowsAttributeChange
{
    public void ChanageWindowsPos(Vector2Int windowsPosition);
    public void ChanageWindowsPos(int posX, int posY);
    public void ChangeWindowsSize(Vector2Int windowsSize);
    public void ChangeWindowsSize(int width, int height);

    public void SetWinodwsOrder(bool isFront, bool isMost);

    // 相对位置
    public void AdjustWindowRelativePosition(int deltaX, int deltaY);
    public void SetWindowTaskbar(bool isHasTaskBar);
    public void SetWindowsPositionToCenter();
    public void LockindowsPositionToCenter();
    public FrameAttribute GetNowFrameAttribute();
    public int GetTitleHeight();
    public void SetWindowResizeMode(bool isCanResize);
}
public struct FrameAttribute
{
    public int posX;
    public int posY;
    public int height;
    public int width;
}
internal class WindowAttributeControllerPlayMode : IWindowsAttributeChange
{
    #region 定义
    
    private const int WS_SIZEBOX = 0x00040000; // 可调整大小的窗口样式

    // 定义WinAPI函数
    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy,
        uint uFlags);

    // 常量定义
    private static readonly IntPtr HWND_TOP = new IntPtr(0);
    private const uint SWP_SHOWWINDOW = 0x0040;

    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    // 获取当前窗口句柄
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOZORDER = 0x0004;

    private const int GWL_EXSTYLE = -20;
    private const int GWL_STYLE = -16;
    private const int WS_EX_LAYERED = 0x00080000;
    private const int WS_BORDER = 0x00800000;
    private const int WS_CAPTION = 0x00C00000;
    private const int LWA_COLORKEY = 0x1;
    private const int LWA_ALPHA = 0x00000002;
    private const int WS_EX_TRANSPARENT = 0x20;
    private const int WS_EX_TOOLWINDOW = 0x00000080;
    
    
// GetSystemMetrics实际获取的是系统记录的分辨率，不是物理分辨率，如屏幕2560*1600，显示缩放200%，这里获取到的是1280*800
    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetSystemMetrics(int nIndex);
    private static int SM_CXSCREEN = 0; //主屏幕分辨率宽度
    private static int SM_CYSCREEN = 1; //主屏幕分辨率高度
    private static int SM_CYCAPTION = 4; //标题栏高度
    private static int SM_CXFULLSCREEN = 16; //最大化窗口宽度（减去任务栏）
    private static int SM_CYFULLSCREEN = 17; //最大化窗口高度（减去任务栏）


    #endregion

    // 实用函数 获取当前窗口的各种信息
    public static FrameAttribute GetFrameAttribute(IntPtr _hWnd)
    {
        FrameAttribute framAttribute = new FrameAttribute();
        RECT rect;
        if (GetWindowRect(_hWnd, out rect))
        {
            framAttribute.posX = rect.Left;
            framAttribute.posY  = rect.Top;
            framAttribute.width = rect.Right - rect.Left;
            framAttribute.height = rect.Bottom - rect.Top;
        }

        return framAttribute;
    }

    public void LockindowsPositionToCenter()
    {
        FrameAttribute win = GetNowFrameAttribute();
        int offsetX = win.width/2;
        int offsetY = win.height/2;
        int x = GetSystemMetrics(SM_CXSCREEN);
        int y = GetSystemMetrics(SM_CYSCREEN);
        ChanageWindowsPos(new Vector2Int(x / 2 - offsetX, y / 2 - offsetY));
    }

    public FrameAttribute GetNowFrameAttribute()
    {
        return GetFrameAttribute(hWnd);
    }

    public int GetTitleHeight()
    {
        return GetSystemMetrics(SM_CYCAPTION);
    }

    public void SetWindowResizeMode(bool isCanResize)
    {
        int style = GetWindowLong(hWnd, GWL_STYLE);
        if (isCanResize)
        {
            style |= WS_SIZEBOX; // 添加可调整大小样式
        }
        else
        {
            style &= ~WS_SIZEBOX; // 移除可调整大小样式
        }
        SetWindowLong(hWnd, GWL_STYLE, style);
    }

    // 获取当前窗口句柄
    private IntPtr hWnd = GetActiveWindow();
    //
    // public void ChanageWindowsAttribute(Vector2Int windowsPosition, Vector2Int windowsSize)
    // {
    //     SetWindowPos(hWnd, HWND_TOP, windowsPosition.x, windowsPosition.y, windowsSize.x, windowsSize.y, SWP_SHOWWINDOW);
    // }

    public void ChanageWindowsPos(Vector2Int windowsPosition)
    {

        SetWindowPos(hWnd, HWND_TOP, windowsPosition.x, windowsPosition.y, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
        
    }

    public void ChanageWindowsPos(int posX, int posY)
    {
        ChanageWindowsPos(new Vector2Int(posX, posY));
    }

    public void ChangeWindowsSize(Vector2Int windowsSize)
    {
        int title = GetSystemMetrics(SM_CYCAPTION);
        SetWindowPos(hWnd, HWND_TOP, 0, 0, windowsSize.x + 2, windowsSize.y + title + 2, SWP_NOMOVE);
        
    }

    public void ChangeWindowsSize(int width, int height)
    {
        ChangeWindowsSize(new Vector2Int(width, height));
    }


    public void SetWinodwsOrder(bool isFront, bool isMost)
    {
        IntPtr order = HWND_TOP;
        if (isFront)
        {
            if (isMost)
            {
                order = HWND_TOPMOST;
            }
            else
            {
                order = HWND_TOP;
            }
        }
        else
        {
            if (isMost)
            {
                order = HWND_BOTTOM;
            }
            else
            {
                order = HWND_NOTOPMOST;
            }
        }



        SetWindowPos(hWnd, order, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        
    }

    // 窗口矩形结构
    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public void AdjustWindowRelativePosition(int deltaX, int deltaY)
    {
        RECT rect;
        FrameAttribute win = GetNowFrameAttribute();

        int newX = win.posX + deltaX;
        int newY = win.posY + deltaY;


        SetWindowPos(hWnd, HWND_TOP, newX, newY, win.width, win.height, SWP_SHOWWINDOW);
    }

    public void SetWindowTaskbar(bool isHasTaskBar)
    {
        if (isHasTaskBar)
        {
            int windowLong = GetWindowLong(hWnd, GWL_EXSTYLE);
            SetWindowLong(hWnd, GWL_EXSTYLE, windowLong & ~WS_EX_TOOLWINDOW);
        }
        else
        {
            int windowLong = GetWindowLong(hWnd, GWL_EXSTYLE);
            SetWindowLong(hWnd, GWL_EXSTYLE, windowLong | WS_EX_TOOLWINDOW);
        }
    }
    Resolution primaryRes = Screen.currentResolution;
    
    public void SetWindowsPositionToCenter()
    {
        FrameAttribute win = GetNowFrameAttribute();
        int offsetX = win.width/2;
        int offsetY = win.height/2;
 
        ChanageWindowsPos(new Vector2Int(primaryRes.width / 2 - offsetX, primaryRes.height / 2 - offsetY));
        
    }
}

internal class WindowAttributeControllerEditorMode : IWindowsAttributeChange
{
    public void ChanageWindowsPos(Vector2Int windowsPosition)
    {
    }

    public void ChanageWindowsPos(int posX, int posY)
    {
    }

    public void ChangeWindowsSize(Vector2Int windowsSize)
    {
    }

    public void ChangeWindowsSize(int width, int height)
    {
    }

    public void SetWinodwsOrder(bool isFront, bool isMost)
    {
    }

    public void AdjustWindowRelativePosition(int deltaX, int deltaY)
    {
        Debug.Log($"尝试移动窗口相对位置{deltaX},{deltaY}");
    }

    public void SetWindowTaskbar(bool isHasTaskBar)
    {
    }

    public void SetWindowsPositionToCenter()
    {
        
    }

    public void LockindowsPositionToCenter()
    {
        
    }

    public FrameAttribute GetNowFrameAttribute()
    {
        return new FrameAttribute();
    }

    public int GetTitleHeight()
    {
        return 30;
    }

    public void SetWindowResizeMode(bool isCanResize)
    {
        
    }
}