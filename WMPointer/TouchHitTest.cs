using System;
using System.Runtime.InteropServices;

namespace WMPointer.TouchHitTest
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TOUCH_HIT_TESTING_INPUT
    {
        public uint pointerId;
        public int pointX;
        public int pointY;
        public int boundingBoxLeft;
        public int boundingBoxTop;
        public int boundingBoxRight;
        public int boundingBoxBottom;
        public int nonOccludedBoundingBoxLeft;
        public int nonOccludedBoundingBoxTop;
        public int nonOccludedBoundingBoxRight;
        public int nonOccludedBoundingBoxBottom;
        public uint orientation;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TOUCH_HIT_TESTING_PROXIMITY_EVALUATION
    {
        public ushort score;
        public int adjustedPointX;
        public int adjustedPointY;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct POINT
    {
        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X;
        public int Y;

    }

    public static class Win32
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT rc);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT rc);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ScreenToClient(IntPtr hWnd, ref POINT Point);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT Point);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PtInRect(RECT rc, POINT pt);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool EvaluateProximityToRect(RECT controlBoundingBox, TOUCH_HIT_TESTING_INPUT hitTestingInput, out TOUCH_HIT_TESTING_PROXIMITY_EVALUATION proximityEval);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool EvaluateProximityToPolygon(uint numVertices, POINT[] controlPolygon, TOUCH_HIT_TESTING_INPUT HitTestingInput, out TOUCH_HIT_TESTING_PROXIMITY_EVALUATION pProximityEval);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterTouchHitTestingWindow(IntPtr hWnd, ulong value);
    }
}
