using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WMPointer.InteractionContext
{
    public class InertiaParameters
    {
        public float TranslationDeceleration;
        public float TranslationDisplacement;
        public float RotationDeceleration;
        public float RotationAngle;
        public float ExpansionDeceleration;
        public float ExpansionExpansion;

        public InertiaParameters()
        {
            TranslationDeceleration = 0.00463f;
            TranslationDisplacement = float.MaxValue;
            RotationDeceleration = 0.000015f;
            RotationAngle = float.MaxValue;
            ExpansionDeceleration = 0.00463f;
            ExpansionExpansion = float.MaxValue;
        }
    }

    public class Interactable : IDisposable
    {
        HashSet<uint> pointers;
        public IntPtr HWnd { get; private set; }
        IntPtr context = IntPtr.Zero;
        Win32.INTERACTION_CONTEXT_OUTPUT_CALLBACK callback;
        INTERACTION_CONTEXT_OUTPUT output;
        InertiaParameters inertiaParams = new InertiaParameters();
        public bool IsTouched => pointers.Count > 0;
        public bool IsActive { get; private set; }
        public bool IsInertia { get; private set; }

        public event EventHandler<INTERACTION_CONTEXT_OUTPUT> Interaction;

        public Interactable()
        {
            pointers = new HashSet<uint>();
            Win32.CreateInteractionContext(out context);
            callback = CallbackFunction;
            Win32.RegisterOutputCallbackInteractionContext(context, Marshal.GetFunctionPointerForDelegate(callback), context);
        }

        public void Configure(INTERACTION_CONTEXT_CONFIGURATION[] cfg)
        {
            if (context != IntPtr.Zero && (!IsActive) && pointers.Count == 0)
                Win32.SetInteractionConfigurationInteractionContext(context, cfg.Length, cfg);
        }

        void CallbackFunction(IntPtr ic, IntPtr outputPtr)
        {
            output = (INTERACTION_CONTEXT_OUTPUT)Marshal.PtrToStructure(outputPtr, typeof(INTERACTION_CONTEXT_OUTPUT));
            IsInertia = output.InteractionFlags.HasFlag(INTERACTION_FLAGS.INERTIA);
            var ended = output.InteractionFlags.HasFlag(INTERACTION_FLAGS.END);

            if (!IsActive)
                IsActive = output.InteractionFlags.HasFlag(INTERACTION_FLAGS.BEGIN) || IsInertia;
            if (ended)
            {
                IsActive = false;
                IsInertia = false;
            }
            OnData(output);
            Interaction?.Invoke(this, output);
        }

        protected virtual void OnData(INTERACTION_CONTEXT_OUTPUT data) { }

        public void AddPointer(IntPtr hWnd, uint pointerId)
        {
            HWnd = hWnd;
            Win32.AddPointerInteractionContext(context, pointerId);
            pointers.Add(pointerId);
        }

        public void ProcessPointer(uint pointerId)
        {
            if (pointers.Contains(pointerId))
            {
                int entriesCount = 0;
                int pointerCount = 0;
                WMPointer.Win32.GetPointerFrameInfoHistory(pointerId, ref entriesCount, ref pointerCount, IntPtr.Zero);

                POINTER_INFO[] piArr = new POINTER_INFO[entriesCount * pointerCount];
                WMPointer.Win32.GetPointerFrameInfoHistory(pointerId, ref entriesCount, ref pointerCount, piArr);
                IntPtr hr = Win32.ProcessPointerFramesInteractionContext(context, entriesCount, pointerCount, piArr);
            }
        }

        public void ProcessInertia()
        {
            if (IsInertia)
                Win32.ProcessInertiaInteractionContext(context);
        }

        public void StopInteraction()
        {
            Win32.StopInteractionContext(context);
        }

        public void RemovePointer(uint pointerId)
        {
            if (pointers.Contains(pointerId))
            {
                Win32.RemovePointerInteractionContext(context, pointerId);
                pointers.Remove(pointerId);
            }
        }

        public void Dispose()
        {
            Win32.DestroyInteractionContext(context);
            context = IntPtr.Zero;
        }

        public InertiaParameters GetInertiaParameter()
        {
            Win32.GetInertiaParameterInteractionContext(context, INERTIA_PARAMETER.TRANSLATION_DECELERATION, out inertiaParams.TranslationDeceleration);
            Win32.GetInertiaParameterInteractionContext(context, INERTIA_PARAMETER.TRANSLATION_DISPLACEMENT, out inertiaParams.TranslationDisplacement);
            Win32.GetInertiaParameterInteractionContext(context, INERTIA_PARAMETER.ROTATION_DECELERATION, out inertiaParams.RotationDeceleration);
            Win32.GetInertiaParameterInteractionContext(context, INERTIA_PARAMETER.ROTATION_ANGLE, out inertiaParams.RotationAngle);
            Win32.GetInertiaParameterInteractionContext(context, INERTIA_PARAMETER.EXPANSION_DECELERATION, out inertiaParams.ExpansionDeceleration);
            Win32.GetInertiaParameterInteractionContext(context, INERTIA_PARAMETER.EXPANSION_EXPANSION, out inertiaParams.ExpansionExpansion);
            return inertiaParams;
        }

        public void SetInertiaParameter(InertiaParameters param)
        {
            if (inertiaParams.TranslationDeceleration != param.TranslationDeceleration)
            {
                Win32.SetInertiaParameterInteractionContext(context, INERTIA_PARAMETER.TRANSLATION_DECELERATION, param.TranslationDeceleration);
                inertiaParams.TranslationDeceleration = param.TranslationDeceleration;
            }
            if (inertiaParams.TranslationDisplacement != param.TranslationDisplacement)
            {
                Win32.SetInertiaParameterInteractionContext(context, INERTIA_PARAMETER.TRANSLATION_DISPLACEMENT, param.TranslationDisplacement);
                inertiaParams.TranslationDisplacement = param.TranslationDisplacement;
            }
            if (inertiaParams.RotationDeceleration != param.RotationDeceleration)
            {
                Win32.SetInertiaParameterInteractionContext(context, INERTIA_PARAMETER.ROTATION_DECELERATION, param.RotationDeceleration);
                inertiaParams.RotationDeceleration = param.RotationDeceleration;
            }
            if (inertiaParams.RotationAngle != param.RotationAngle)
            {
                Win32.SetInertiaParameterInteractionContext(context, INERTIA_PARAMETER.ROTATION_ANGLE, param.RotationAngle);
                inertiaParams.RotationAngle = param.RotationAngle;
            }
            if (inertiaParams.ExpansionDeceleration != param.ExpansionDeceleration)
            {
                Win32.SetInertiaParameterInteractionContext(context, INERTIA_PARAMETER.EXPANSION_DECELERATION, param.ExpansionDeceleration);
                inertiaParams.ExpansionDeceleration = param.ExpansionDeceleration;
            }
            if (inertiaParams.ExpansionExpansion != param.ExpansionExpansion)
            {
                Win32.SetInertiaParameterInteractionContext(context, INERTIA_PARAMETER.EXPANSION_EXPANSION, param.ExpansionExpansion);
                inertiaParams.ExpansionExpansion = param.ExpansionExpansion;
            }
        }
    }
}
