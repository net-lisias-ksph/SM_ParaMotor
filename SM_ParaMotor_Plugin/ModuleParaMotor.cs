
using System.Collections;
using UnityEngine;

namespace SM_ParaMotor
{
    public class ModuleParaMotor : ModuleEngines
    {

//        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "GROUND LAUNCH"),
//         UI_Toggle(controlEnabled = true, scene = UI_Scene.All, disabledText = "", enabledText = "True")]
//        public bool groundTakeOff = false;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "THROTTLE %"),
         UI_FloatRange(controlEnabled = true, scene = UI_Scene.All, minValue = 0, maxValue = 100, stepIncrement = 1f)]
        public float _throttle = 0.0f;

        public float jumpForce = 0.0f;
        private bool takingOff = false;

        public override void OnStart(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                part.force_activate();
                Setup();
            }
            base.OnStart(state);
        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (part.vessel.isEVA)
                {
                    if (!takingOff)
                    {
                        StartCoroutine(GroundLaunch());
                    }
                    else
                    {
                        GetParaProp();
                    }

                    if (part.vessel.Landed)
                    {
                        CheckChute();
                    }
                }
            }
        }

        private void Setup()
        {
            var chute = vessel.FindPartModuleImplementing<ModuleEvaChute>();
            chute.enabled = false;
//            var kerbal = part.vessel.FindPartModuleImplementing<KerbalEVA>();
//            jumpForce = kerbal.maxJumpForce;
        }

        private void CheckChute()
        {
            var chute = vessel.FindPartModuleImplementing<ModuleEvaChute>();

            if (chute.enabled)
            {
                takingOff = false;
                chute.Repack();
                chute.enabled = false;
            }
        }

        IEnumerator GroundLaunch()
        {
            var kerbal = part.vessel.FindPartModuleImplementing<KerbalEVA>();
            var chute = part.vessel.FindPartModuleImplementing<ModuleEvaChute>();
            
            if (kerbal.maxJumpForce <= 5)
            {
                kerbal.maxJumpForce = 5;
            }
            
            if (kerbal.JetpackDeployed)
            {
                kerbal.ToggleJetpack();
            }

            if (!part.vessel.Landed)
            {
                takingOff = true;
                _throttle = 50;
                var throttle = _throttle / 100;
                currentThrottle = throttle;
                yield return new WaitForSeconds(1);
                chute.enabled = true;
                chute.Deploy();
            }
        }

        private void GetParaProp()
        {
            var throttle = _throttle / 100;
            currentThrottle = throttle;
        }
    }
}