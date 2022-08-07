using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YourVRExperience.Utils
{
    public class MobileInputManager : StateMachine
    {
        public delegate void ActionButtonEvent();
        public delegate void JumpButtonEvent();
        public delegate void CameraButtonEvent();

        public event ActionButtonEvent ActionEvent;
        public event JumpButtonEvent JumpEvent;
        public event CameraButtonEvent CameraEvent;

        public TouchJoystick MoveJoystick;
        public TouchJoystick RotateJoystick;
        public Button ActionButton;
        public Button JumpButton;
        public Button CameraButton;

        protected virtual void Start()
        {
            if (ActionButton != null) ActionButton.onClick.AddListener(OnActionButton);
            if (JumpButton != null) JumpButton.onClick.AddListener(OnJumpButton);
            if (CameraButton != null) CameraButton.onClick.AddListener(OnCameraButton);
        }


        private void OnCameraButton()
        {
            if (CameraEvent != null) CameraEvent();
        }

        private void OnJumpButton()
        {
            if (JumpEvent != null) JumpEvent();
        }

        private void OnActionButton()
        {
            if (ActionEvent != null) ActionEvent();
        }

    }
}