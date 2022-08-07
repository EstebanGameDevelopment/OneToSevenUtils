using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace YourVRExperience.Utils
{
    public interface IInputController
    {
        public bool IsVR { get; }

        public Transform RayPointerVR { get; }

        void Initialize();
        bool EnableMouseRotation();
        float GetMouseAxisHorizontal();
        float GetMouseAxisVertical();
        bool IsPressedAnyKeyToMove();
        float GetAxisHorizontal();
        float GetAxisVertical();
        public bool JumpPressed();
        public bool ShootPressed();
        bool SwitchedCameraPressed();
    }
}