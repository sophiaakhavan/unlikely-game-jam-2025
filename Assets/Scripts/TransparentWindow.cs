using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// See: https://pinvoke.net/default.aspx/user32.SetWindowLong
// See: https://pinvoke.net/default.aspx/dwmapi.DwmExtendFrameIntoClientArea
// See: https://pinvoke.net/default.aspx/user32.SetWindowPos
// See: https://pinvoke.net/default.aspx/user32.SetLayeredWindowAttributes

// See: https://learn.microsoft.com/en-us/windows/win32/winmsg/extended-window-styles

public class TransparentWindow : MonoBehaviour
{
<<<<<<< Updated upstream
    //[DllImport("user32.dll")]
    //public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    //private void Start()
    //{
    //    MessageBox(new IntPtr(0), "Hello World!", "Hello Dialog", 0);
    //}
=======
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    const int GWL_EXSTYLE = -20;
    const uint WS_EX_LAYERED = 0x00080000;

    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

    const uint LWA_COLORKEY = 0x00000001;


    private void Start()
    {
#if !UNITY_EDITOR
        IntPtr hWnd = GetActiveWindow();

        // Create "sheet of glass" effect
        MARGINS margins = new MARGINS { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(hWnd, ref margins);

        SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);        // allow click-through (on transparent areas)
        SetLayeredWindowAttributes(hWnd, 0, 0, LWA_COLORKEY);   // allow interaction 

        //SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, 0);
#endif

        Application.runInBackground = true;
    }
>>>>>>> Stashed changes
}
