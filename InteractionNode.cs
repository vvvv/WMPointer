#region usings
using System;
using System.Reactive.Linq;

using System.ComponentModel.Composition;
using VVVV.PluginInterfaces.V2;
using VVVV.Core.Logging;
using VVVV.Utils.VMath;
using WMPointer.InteractionContext;
#endregion usings

namespace WMPointer.Nodes
{
    [PluginInfo(Name = "Configuration", Category = "Pointer.Interaction", Tags = "")]
    public class InteractionConfigurationNode : IPluginEvaluate, IPartImportsSatisfiedNotification
    {
        [Input("Manipulation", DefaultEnumEntry = "AllInertia")]
        public ISpread<ISpread<ManipulationFlags>> FManipulation;

        [Output("Configuration")]
        public ISpread<InteractionConfiguration> FConfig;

        public void OnImportsSatisfied()
        {
            FConfig.SliceCount = 0;
        }

        public void Evaluate(int spreadMax)
        {
            spreadMax = FManipulation.SliceCount;

            FConfig.ResizeAndDismiss(spreadMax, () => new InteractionConfiguration());
            for (int i=0; i<spreadMax; i++)
            {
                FConfig[i].Manipulation = INTERACTION_CONFIGURATION_FLAGS.MANIPULATION;
                foreach (var f in FManipulation[i])
                    FConfig[i].Manipulation |= (INTERACTION_CONFIGURATION_FLAGS)f;
            }
        }
    }

    [PluginInfo(Name = "Inertia", Category = "Pointer.Interaction", Tags = "")]
    public class InteractionInertiaNode : IPluginEvaluate, IPartImportsSatisfiedNotification
    {
        [Input("Translation Deceleration", DefaultValue = 0.00463f)]
        public ISpread<float> FTranslationDeceleration;
        [Input("Translation Displacement", DefaultValue = float.MaxValue)]
        public ISpread<float> FTranslationDisplacement;
        [Input("Rotation Deceleration", DefaultValue = 0.000015f)]
        public ISpread<float> FRotationDeceleration;
        [Input("Rotation Angle", DefaultValue = float.MaxValue)]
        public ISpread<float> FRotationAngle;
        [Input("Expansion Deceleration", DefaultValue = 0.00463f)]
        public ISpread<float> FExpansionDeceleration;
        [Input("Expansion Expansion", DefaultValue = float.MaxValue)]
        public ISpread<float> FExpansionExpansion;

        [Output("Inertia")]
        public ISpread<InertiaParameters> FInertia;

        public void OnImportsSatisfied()
        {
            FInertia.SliceCount = 0;
        }

        public void Evaluate(int spreadMax)
        {
            FInertia.ResizeAndDismiss(spreadMax, (int i) => new InertiaParameters());

            for (int i = 0; i < spreadMax; i++)
            {
                FInertia[i].TranslationDeceleration = FTranslationDeceleration[i];
                FInertia[i].TranslationDisplacement = FTranslationDisplacement[i];
                FInertia[i].RotationDeceleration = FRotationDeceleration[i];
                FInertia[i].RotationAngle = FRotationAngle[i];
                FInertia[i].ExpansionDeceleration = FExpansionDeceleration[i];
                FInertia[i].ExpansionExpansion = FExpansionExpansion[i];
            }
        }
    }


    [PluginInfo(Name = "Interaction", Category = "Pointer", Tags = "")]
    public class PointerInteractionNode : IPluginEvaluate, IPartImportsSatisfiedNotification, IDisposable
    {
        #region fields & pins
        [Input("Pointer Device", IsSingle = true)]
        public ISpread<PointerDevice> FDevice;

        [Input("Transform In")]
        public ISpread<Matrix4x4> FIn;

        [Input("Configuration")]
        public ISpread<InteractionConfiguration> FConfig;

        [Input("Inertia")]
        public ISpread<InteractionContext.InertiaParameters> FInertia;

        [Input("Reset", IsBang = true)]
        public ISpread<bool> FReset;

        [Output("Transform Out")]
        public ISpread<Matrix4x4> FOut;

        [Output("Is Active")]
        public ISpread<bool> FIsActive;

        [Output("Is Inertia")]
        public ISpread<bool> FIsInertia;

        [Output("Debug")]
        public ISpread<float> FDebug;


        [Import()]
        public ILogger FLogger;

        PointerDevice FPointerDevice;
        IDisposable FSubscription;
        Spread<InteractionTransform> FInteractions = new Spread<InteractionTransform>();
        #endregion fields & pins


        public void OnImportsSatisfied()
        {
        }


        //called when data for any output pin is requested
        public void Evaluate(int spreadMax)
        {
            var pointerDevice = FDevice[0] ?? PointerDevice.Empty;
            if (pointerDevice != FPointerDevice)
            {
                Unsubscribe();
                FPointerDevice = pointerDevice;
                Subscribe();
            }

            spreadMax = FIn.CombineWith(FConfig).CombineWith(FInertia);
            FInteractions.ResizeAndDispose(spreadMax, (int i) => new InteractionTransform(FIn[i]));

            FOut.SliceCount = spreadMax;
            FIsActive.SliceCount = spreadMax;
            FIsInertia.SliceCount = spreadMax;
            FDebug.SliceCount = spreadMax;
            for (int i=0; i< spreadMax; i++)
            {
                if (FReset[i])
                    FInteractions[i].Reset(FIn[i]);

                if (FConfig[i] != null)
                    FInteractions[i].Configure(FConfig[i].Configuration);
                if (FInertia[i] != null)
                    FInteractions[i].SetInertiaParameter(FInertia[i]);

                FInteractions[i].ProcessInertia();
                FOut[i] = FInteractions[i].Transform;
                FIsActive[i] = FInteractions[i].IsActive;
                FIsInertia[i] = FInteractions[i].IsInertia;
            }
        }

        private void Subscribe()
        {
            if (FPointerDevice != null)
            {
                FSubscription = FPointerDevice.Notifications
                    .Where(o => o.PointerType != INPUT_TYPE.NONE)
                    .Subscribe(i => GenerateInfo(i));
            }
        }

        void GenerateInfo(PointerNotification notification)
        {
            int x = notification.RawX;
            int y = notification.RawY;
            foreach (var it in FInteractions)
            {
                if (notification.Message == WMExtension.POINTERDOWN)
                {
                    var score = it.TryAddPointer(notification.HWnd, notification.Id, x, y);
                    it.ProcessPointer(notification.Id);
                }
                else if (notification.Message == WMExtension.POINTERUPDATE)
                {
                    it.ProcessPointer(notification.Id);
                }
                else if (notification.Message == WMExtension.POINTERUP)
                {
                    it.ProcessPointer(notification.Id);
                    it.RemovePointer(notification.Id);
                }
            }
        }

        private void Unsubscribe()
        {
            if (FSubscription != null)
            {
                FSubscription.Dispose();
                FSubscription = null;
            }
        }

        public void Dispose()
        {
            Unsubscribe();
        }
    }
}