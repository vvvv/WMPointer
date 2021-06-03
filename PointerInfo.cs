using System;
using System.Runtime.InteropServices;

namespace WMPointer
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct POINTER_INFO
    {
        public INPUT_TYPE pointerType;
        public uint pointerId;
        public uint frameId;
        public MESSAGE_FLAGS pointerFlags;
        public IntPtr sourceDevice;
        public IntPtr hwndTarget;
        public int ptPixelLocationX;
        public int ptPixelLocationY;
        public int ptHimetricLocationX;
        public int ptHimetricLocationY;
        public int ptPixelLocationRawX;
        public int ptPixelLocationRawY;
        public int ptHimetricLocationRawX;
        public int ptHimetricLocationRawY;
        public uint dwTime;
        public uint historyCount;
        public int inputData;
        public MODIFIER_KEY_STATE dwKeyStates;
        public UInt64 PerformanceCount;
        public BUTTON_CHANGE_TYPE ButtonChangeType;
    }

    [Flags]
    public enum MESSAGE_FLAGS
    {
        NEW = 0x00000001,
        INRANGE = 0x00000002,
        INCONTACT = 0x00000004,
        FIRSTBUTTON = 0x00000010,
        SECONDBUTTON = 0x00000020,
        THIRDBUTTON = 0x00000040,
        FOURTHBUTTON = 0x00000080,
        FIFTHBUTTON = 0x00000100,
        PRIMARY = 0x00002000,
        CONFIDENCE = 0x00000400,
        CANCELED = 0x00000800,
        DOWN = 0x00010000,
        UPDATE = 0x00020000,
        UP = 0x00040000,
        WHEEL = 0x00080000,
        HWHEEL = 0x00100000,
        CAPTURECHANGED = 0x00200000,
        HASTRANSFORM = 0x00400000
    }

    [Flags]
    public enum MODIFIER_KEY_STATE
    {
        NONE = 0x0000,
        LBUTTON = 0x0001,
        RBUTTON = 0x0002,
        SHIFT = 0x0004,
        CTRL = 0x0008,
        MBUTTON = 0x0010,
        XBUTTON1 = 0x0020,
        XBUTTON2 = 0x0040
    }

    public enum BUTTON_CHANGE_TYPE : ulong
    {
        NONE,
        FIRSTBUTTON_DOWN,
        FIRSTBUTTON_UP,
        SECONDBUTTON_DOWN,
        SECONDBUTTON_UP,
        THIRDBUTTON_DOWN,
        THIRDBUTTON_UP,
        FOURTHBUTTON_DOWN,
        FOURTHBUTTON_UP,
        FIFTHBUTTON_DOWN,
        FIFTHBUTTON_UP
    }

    public static partial class Win32
    {
        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool GetPointerInfo(uint pointerID, out POINTER_INFO pointerInfo);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool GetPointerInfoHistory(uint pointerID, ref int entriesCount, IntPtr zero);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern bool GetPointerInfoHistory(uint pointerID, ref int entriesCount, [MarshalAs(UnmanagedType.LPArray), In, Out] POINTER_INFO[] pointerInfo);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetPointerFrameInfo(uint pointerID, ref int pointerCount, IntPtr zero);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetPointerFrameInfo(uint pointerID, ref int pointerCount, [MarshalAs(UnmanagedType.LPArray), In, Out] POINTER_INFO[] pointerInfo);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetPointerFrameInfoHistory(uint pointerID, ref int entriesCount, ref int pointerCount, IntPtr zero);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetPointerFrameInfoHistory(uint pointerID, ref int entriesCount, ref int pointerCount, [MarshalAs(UnmanagedType.LPArray), In, Out] POINTER_INFO[] pointerInfo);
    }
}
