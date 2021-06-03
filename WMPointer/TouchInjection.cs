using System;
using System.Runtime.InteropServices;

namespace WMPointer.TouchInjection
{
    public enum TOUCH_FEEDBACK
    {
        DEFAULT = 0x1,
        INDIRECT = 0x2,
        NONE = 0x3,
        MAX_TOUCH_COUNT = 256
    }

    public static class Win32
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool InitializeTouchInjection(int maxCount, TOUCH_FEEDBACK feedbackMode);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool InjectTouchInput(int count, [MarshalAs(UnmanagedType.LPArray), In] POINTER_TOUCH_INFO[] contacts);
    }
}
