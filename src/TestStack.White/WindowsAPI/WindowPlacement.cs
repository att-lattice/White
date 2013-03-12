using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace White.Core.WindowsAPI
{
    /// <summary>
    /// Intended for White Internal use only
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowPlacement
    {
        public uint Length;
        public uint Flags;
        public uint ShowCMD;
        public Point MinmizedPosition;
        public Point MaximizedPosition;
        public Rectangle NormalPosition;
    }

    /// <summary>
    /// Intended for White Internal use only
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowInfo
    {
        public uint size;
        public Rectangle layout;
        public Rectangle clientLayout;
        public uint style;
        public uint exStyle;
        public uint activeStatus;
        public uint borderWidth;
        public uint borderHeight;
        public ushort atom;
        public ushort windowVersion;
    }

    /// <summary>
    /// Intended for White Internal use only
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MouseInput
    {
        private readonly int dx;
        private readonly int dy;
        private readonly int mouseData;
        private readonly int dwFlags;
        private readonly int time;
        private readonly IntPtr dwExtraInfo;

        public MouseInput(int dwFlags, IntPtr dwExtraInfo)
        {
            this.dwFlags = dwFlags;
            this.dwExtraInfo = dwExtraInfo;
            dx = 0;
            dy = 0;
            time = 0;
            mouseData = 0;
        }
    }

    /// <summary>
    /// Intended for White Internal use only
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyboardInput
    {
        private readonly short wVk;
        private readonly short wScan;
        private readonly KeyUpDown dwFlags;
        private readonly int time;
        private readonly IntPtr dwExtraInfo;

        public KeyboardInput(short wVk, KeyUpDown dwFlags, IntPtr dwExtraInfo)
        {
            this.wVk = wVk;
            wScan = 0;
            this.dwFlags = dwFlags;
            time = 0;
            this.dwExtraInfo = dwExtraInfo;
        }

        public enum KeyUpDown
        {
            KEYEVENTF_KEYDOWN = 0x0000,
            KEYEVENTF_EXTENDEDKEY = 0x0001,
            KEYEVENTF_KEYUP = 0x0002,
        }

        public enum SpecialKeys
        {
            //http://pinvoke.net/default.aspx/user32/SendInput.html, http://delphi.about.com/od/objectpascalide/l/blvkc.htm
            
            LBUTTON = 0x01, //Left mouse button
            RBUTTON = 0x02, //Right mouse button
            CANCEL = 0x03, //Control-break processing
            MBUTTON = 0x04, //Middle mouse button (three-button mouse)
            BACKSPACE = 0x08, //BACKSPACE key
            TAB = 0x09, //TAB key
            CLEAR = 0x0C, //CLEAR key
            RETURN = 0x0D, //ENTER key
            SHIFT = 0x10,
            CONTROL = 0x11,
            ALT = 0x12,
            LEFT_ALT = 0xA4,
            RIGHT_ALT = 0xA5,
            ESCAPE = 0x1B,
            SPACE = 0x20, //SPACEBAR
            PAGEUP = 0x21,//PAGE UP key (PRIOR)
            PAGEDOWN = 0x22, //(NEXT)
            END = 0x23,  
            HOME = 0x24,
            LEFT = 0x25,//LEFT ARROW key
            UP = 0x26,
            RIGHT = 0x27,//RIGHT ARROW key
            DOWN = 0x28,
            SELECT = 0x29, //SELECT key           
            PRINT = 0x2A, 
            EXECUTE = 0x2B,
            PRINTSCREEN = 0x2C, //PRINT SCREEN key (SNAPSHOT)
            INSERT = 0x2D,
            DELETE = 0x2E,
            HELP = 0x2F,
            CAPS = 0x14,
            
            A = 0x41,
            B = 0x42,
            C = 0x43,
            D = 0x44,
            E = 0x45,
            F = 0x46,
            G = 0x47,
            H = 0x48,
            I = 0x49,
            J = 0x4A,
            K = 0x4B,
            L = 0x4C,
            M = 0x4D,
            N = 0x4E,
            O = 0x4F,
            P = 0x50,
            Q = 0x51,
            R = 0x52,
            S = 0x53,
            T = 0x54,
            U = 0x55,
            V = 0x56,
            W = 0x57,
            X = 0x58,
            Y = 0x59,
            Z = 0x5A,

            LWIN = 0x5B,
            RWIN = 0x5C,
            APPS = 0x5D,

            F1 = 0x70,
            F2 = 0x71,
            F3 = 0x72,
            F4 = 0x73,
            F5 = 0x74,
            F6 = 0x75,
            F7 = 0x76,
            F8 = 0x77,
            F9 = 0x78,
            F10 = 0x79,
            F11 = 0x7A,
            F12 = 0x7B,
            F13 = 0x7C,
            F14 = 0x7D,
            F15 = 0x7E,
            F16 = 0x7F,
            F17 = 0x80,
            F18 = 0x81,
            F19 = 0x82,
            F20 = 0x83,
            F21 = 0x84,
            F22 = 0x85,
            F23 = 0x86,
            F24 = 0x87,

            NUMLOCK = 0x90,
            SCROLL = 0x91,            
            LSHIFT = 0xA0, //Left SHIFT key
            RSHIFT = 0xA1, //Right SHIFT key
            LCONTROL = 0xA2, //Left CONTROL key
            RCONTROL = 0xA3, //Right CONTROL key
            LMENU = 0xA4, //Left MENU key
            PLAY = 0xFA, //Play key
            ZOOM = 0xFB //Zoom key

        }
    }

    /// <summary>
    /// Intended for White Internal use only
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HardwareInput
    {
        private int uMsg;
        private short wParamL;
        private short wParamH;
    }

    /// <summary>
    /// Intended for White Internal use only
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CursorInfo
    {
        public uint size;
        public uint flags;
        public IntPtr handle;
        public Point point;

        public static CursorInfo New()
        {
            CursorInfo info = new CursorInfo();
            info.size = (uint) Marshal.SizeOf(typeof (CursorInfo));
            return info;
        }
    }

    /// <summary>
    /// Intended for White Internal use only
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    /// <summary>
    /// Intended for White Internal use only
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct COLORREF
    {
        public byte R;
        public byte G;
        public byte B;

        public override string ToString()
        {
            return string.Format("R={0},G={1},B={2}", R, G, B);
        }
    }
}