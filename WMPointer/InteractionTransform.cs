using System;
using System.Drawing;
using VVVV.Utils.VMath;
using WMPointer.InteractionContext;

namespace WMPointer
{
    public class InteractionTransform : Interactable
    {
        static Vector3D A = new Vector3D(-0.5, -0.5, 0);
        static Vector3D B = new Vector3D(-0.5, 0.5, 0);
        static Vector3D C = new Vector3D(0.5, 0.5, 0);
        static Vector3D D = new Vector3D(0.5, -0.5, 0);

        public Matrix4x4 Transform { get; private set; }
        public InteractionTransform(Matrix4x4 transform) : base()
        {
            Transform = transform;

            INTERACTION_CONTEXT_CONFIGURATION[] cfg = new INTERACTION_CONTEXT_CONFIGURATION[]
            {
                new INTERACTION_CONTEXT_CONFIGURATION(INTERACTION.MANIPULATION,
                    INTERACTION_CONFIGURATION_FLAGS.MANIPULATION |
                    INTERACTION_CONFIGURATION_FLAGS.MANIPULATION_TRANSLATION_X |
                    INTERACTION_CONFIGURATION_FLAGS.MANIPULATION_TRANSLATION_Y |
                    INTERACTION_CONFIGURATION_FLAGS.MANIPULATION_ROTATION |
                    INTERACTION_CONFIGURATION_FLAGS.MANIPULATION_SCALING |
                    INTERACTION_CONFIGURATION_FLAGS.MANIPULATION_TRANSLATION_INERTIA |
                    INTERACTION_CONFIGURATION_FLAGS.MANIPULATION_ROTATION_INERTIA |
                    INTERACTION_CONFIGURATION_FLAGS.MANIPULATION_SCALING_INERTIA)

                //new INTERACTION_CONTEXT_CONFIGURATION(INTERACTION.TAP,
                //    INTERACTION_CONFIGURATION_FLAGS.TAP |
                //    INTERACTION_CONFIGURATION_FLAGS.TAP_DOUBLE)

                //new INTERACTION_CONTEXT_CONFIGURATION(INTERACTION.DRAG, INTERACTION_CONFIGURATION_FLAGS.DRAG)
            };

            Configure(cfg);
        }

        public int TryAddPointer(IntPtr hWnd, uint pointerId, int pointX, int pointY)
        {
            var a = Transform * A;
            var b = Transform * B;
            var c = Transform * C;
            var d = Transform * D;

            TouchHitTest.RECT p = new TouchHitTest.RECT();
            TouchHitTest.Win32.GetWindowRect(hWnd, out p);

            TouchHitTest.RECT r = new TouchHitTest.RECT();
            TouchHitTest.Win32.GetClientRect(hWnd, out r);
            var wH = r.Right / 2;
            var hH = r.Bottom / 2;

            Point[] poly = new Point[4];
            poly[3] = new Point((int)(a.x * wH + p.Left + wH), (int)(-a.y * hH + p.Top + hH));
            poly[2] = new Point((int)(b.x * wH + p.Left + wH), (int)(-b.y * hH + p.Top + hH));
            poly[1] = new Point((int)(c.x * wH + p.Left + wH), (int)(-c.y * hH + p.Top + hH));
            poly[0] = new Point((int)(d.x * wH + p.Left + wH), (int)(-d.y * hH + p.Top + hH));

            TouchHitTest.TOUCH_HIT_TESTING_INPUT t = new TouchHitTest.TOUCH_HIT_TESTING_INPUT();
            t.pointerId = pointerId;
            t.pointX = pointX;
            t.pointY = pointY;
            t.boundingBoxLeft = t.nonOccludedBoundingBoxLeft = pointX - 1;
            t.boundingBoxTop  = t.nonOccludedBoundingBoxTop =  pointY -1;
            t.nonOccludedBoundingBoxRight = t.boundingBoxRight = pointX + 1;
            t.boundingBoxBottom = t.nonOccludedBoundingBoxBottom = pointY + 1;
            t.orientation = 90;

            TouchHitTest.TOUCH_HIT_TESTING_PROXIMITY_EVALUATION e = new TouchHitTest.TOUCH_HIT_TESTING_PROXIMITY_EVALUATION();
            TouchHitTest.Win32.EvaluateProximityToPolygon(4, poly, t, out e);

            if (e.score == 0)
                base.AddPointer(hWnd, pointerId);
            return e.score;
        }

        protected override void OnData(INTERACTION_CONTEXT_OUTPUT data)
        {
            MANIPULATION_TRANSFORM mt = data.Manipulation.Delta;

            TouchHitTest.RECT rect;
            TouchHitTest.Win32.GetClientRect(HWnd, out rect);
            var tr = VMath.Translate(mt.TranslationX / rect.Right * 2, -mt.TranslationY / rect.Bottom * 2, 0);
            //var anchor = VMath.Transform(ico.X / rect.Right, -ico.Y / rect.Bottom);
            Transform = VMath.RotateZ(-mt.Rotation) * VMath.Scale(mt.Scale, mt.Scale, 1) * Transform * tr;
        }

        public void Reset(Matrix4x4 transform)
        {
            StopInteraction();
            Transform = transform;
        }
    }
}
