using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace YourVRExperience.Utils
{
    public class InputController : MonoBehaviour, IInputController
    {
        public const string EVENT_INPUTCONTROLLER_ENABLE_MOBILE_HUD = "EVENT_INPUTCONTROLLER_ENABLE_MOBILE_HUD";
        public const string EVENT_INPUTCONTROLLER_HAS_STARTED = "EVENT_INPUTCONTROLLER_HAS_STARTED";

        private bool m_isVR = false;
        private Transform m_rayPointerVR;
        private MobileInputManager m_mobileJoysticks;
#if ENABLE_MOBILE
        private bool m_triggeredCameraChange = false;
        private bool m_triggeredJump = false;
        private bool m_triggeredAction = false;
#endif

        public virtual bool IsVR
        {
            get { return m_isVR; }
        }

        public virtual Transform RayPointerVR
        {
            get
            {
                return m_rayPointerVR;
            }
        }

        void Start()
        {
            SystemEventController.Instance.DispatchSystemEvent(EVENT_INPUTCONTROLLER_HAS_STARTED, this.gameObject);
        }

        public void Initialize()
        {
#if ENABLE_MOBILE
        m_mobileJoysticks = Instantiate(Resources.Load("Mobile/MobileHUD") as GameObject).GetComponent<MobileInputManager>();
        m_mobileJoysticks.ActionEvent += OnMobileActionEvent;
        m_mobileJoysticks.JumpEvent += OnMobileJumpEvent;
        m_mobileJoysticks.CameraEvent += OnMobileCameraEvent;
#endif
            SystemEventController.Instance.Event += OnSystemEvent;
        }

        void OnDestroy()
        {
#if ENABLE_MOBILE
        if (m_mobileJoysticks != null)
        {
            m_mobileJoysticks.ActionEvent -= OnMobileActionEvent;
            m_mobileJoysticks.JumpEvent -= OnMobileJumpEvent;
            m_mobileJoysticks.CameraEvent -= OnMobileCameraEvent;
        }
#endif
            if (SystemEventController.Instance != null) SystemEventController.Instance.Event -= OnSystemEvent;
        }

        private void OnSystemEvent(string _nameEvent, object[] _parameters)
        {
            if (_nameEvent == EVENT_INPUTCONTROLLER_ENABLE_MOBILE_HUD)
            {
                bool activate = (bool)_parameters[0];
                if (m_mobileJoysticks != null)
                {
                    m_mobileJoysticks.gameObject.SetActive(activate);
                }
            }
        }

        public virtual bool EnableMouseRotation()
        {
#if ENABLE_MOBILE
        return false;
#else
            return true;
#endif
        }

#if ENABLE_MOBILE
        private void OnMobileCameraEvent()
        {
            m_triggeredCameraChange = true;
        }

        private void OnMobileJumpEvent()
        {
            m_triggeredJump = true;
        }

        private void OnMobileActionEvent()
        {
            m_triggeredAction = true;
        }
#endif

        public bool IsPressedAnyKeyToMove()
        {
            Vector2 joystick = GetMovementJoystick();
            if (joystick.x != 0) return true;
            if (joystick.y != 0) return true;

            return false;
        }

        public float GetAxisHorizontal()
        {
            Vector2 joystick = GetMovementJoystick();
            return joystick.x;
        }

        public float GetAxisVertical()
        {
            Vector2 joystick = GetMovementJoystick();
            return joystick.y;
        }

        public float GetMouseAxisHorizontal()
        {
            Vector2 rotationJoystick = GetRotationJoystick();
            return rotationJoystick.x;
        }
       
        public float GetMouseAxisVertical()
        {
            Vector2 rotationJoystick = GetRotationJoystick();
            return rotationJoystick.y;
        }

        public virtual bool JumpPressed()
        {
#if ENABLE_MOBILE
        if (m_triggeredJump)
        {
            m_triggeredJump = false;
            return true;
        }
        else
        {
            return false;
        }
#else
            return Input.GetButtonDown("Fire2") || Input.GetKeyDown(KeyCode.Space);
#endif
        }

        public virtual bool ShootPressed()
        {
#if ENABLE_MOBILE
        if (m_triggeredAction)
        {
            m_triggeredAction = false;
            return true;
        }
        else
        {
            return false;
        }
#else
            return Input.GetButtonDown("Fire1") || Input.GetMouseButtonDown(0);
#endif
        }

        public virtual bool SwitchedCameraPressed()
        {
#if ENABLE_MOBILE
        if (m_triggeredCameraChange)
        {
            m_triggeredCameraChange = false;
            return true;
        }
        else
        {
            return false;
        }
#else
            return Input.GetKeyDown(KeyCode.Y);
#endif
        }


        protected virtual Vector2 GetMovementJoystick()
        {
#if ENABLE_MOBILE
        return new Vector2(m_mobileJoysticks.MoveJoystick.Horizontal, m_mobileJoysticks.MoveJoystick.Vertical);
#else
            return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
#endif
        }

        private Vector2 GetRotationJoystick()
        {
#if ENABLE_MOBILE
        return new Vector2(m_mobileJoysticks.RotateJoystick.Horizontal / 25, m_mobileJoysticks.RotateJoystick.Vertical / 25);
#else
            return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
#endif
        }
    }
}