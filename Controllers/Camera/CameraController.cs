using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YourVRExperience.Utils
{
    public class CameraController : StateMachine
    {
        public const string EVENT_CAMERA_SWITCHED_TO_1ST_PERSON = "EVENT_CAMERA_SWITCHED_TO_1ST_PERSON";
        public const string EVENT_CAMERA_SWITCHED_TO_3RD_PERSON = "EVENT_CAMERA_SWITCHED_TO_3RD_PERSON";
        public const string EVENT_CAMERA_SWITCHED_TO_FREE_CAMERA = "EVENT_CAMERA_SWITCHED_TO_FREE_CAMERA";
        
        public const string EVENT_CAMERA_PLAYER_READY_FOR_CAMERA = "EVENT_CAMERA_PLAYER_READY_FOR_CAMERA";

        public enum CAMERA_STATES { CAMERA_1ST_PERSON = 0, CAMERA_3RD_PERSON, CAMERA_FROZEN, CAMERA_FREE }

        public const float SPEED_ROTATION = 10f;

        private static CameraController _instance;
        public static CameraController Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = GameObject.FindObjectOfType<CameraController>();
                }
                return _instance;
            }
        }

        public Vector3 Offset = new Vector3(0, 3, 5);
        public float Speed = 20;

        protected ICameraPlayer m_player;
        protected Camera m_gameCamera;
        protected GameObject XRRig;

        public Camera GameCamera
        {
            get
            {
                if (m_gameCamera == null)
                {
                    m_gameCamera = Camera.main;
                    if (m_gameCamera == null)
                    {
                        CameraFinder cameraFinder = GameObject.FindObjectOfType<CameraFinder>();
                        if (cameraFinder != null)
                        {
                            m_gameCamera = cameraFinder.MainCamera;
                        }
                        
                    }
                }
                return m_gameCamera;
            }
        }

        public GameObject ContainerCamera
        {
            get
            {
#if ENABLE_OCULUS
            if (m_inputControls.IsVR)
            {
                if (XRRig == null) XRRig = GameObject.FindObjectOfType<OVRCameraRig>().gameObject;
                return XRRig;
            }
            else
            {
                return GameCamera.gameObject;
            }
#else
                return GameCamera.gameObject;
#endif
            }
        }
        protected IInputController m_inputControls;
        public IInputController InputControls
        {
            get { return m_inputControls; }
        }

        void Awake()
        {
            m_state = (int)CAMERA_STATES.CAMERA_1ST_PERSON;

            SystemEventController.Instance.Event += OnSystemEvent;
        }

        void OnDestroy()
        {
            m_inputControls = null;
            if (SystemEventController.Instance != null) SystemEventController.Instance.Event -= OnSystemEvent;
        }

        private void OnSystemEvent(string _nameEvent, object[] _parameters)
        {
            if (_nameEvent == InputController.EVENT_INPUTCONTROLLER_HAS_STARTED)
            {
                m_inputControls = ((GameObject)_parameters[0]).GetComponent<IInputController>();
            }
            if (_nameEvent == EVENT_CAMERA_PLAYER_READY_FOR_CAMERA)
            {
                ICameraPlayer player = (ICameraPlayer)_parameters[0];
                if (player.IsOwner())
                {
                    m_player = player;
                }
            }
            if (_nameEvent == ScreenController.EVENT_SCREENCONTROLLER_REQUEST_CAMERA_DATA)
            {
                GameObject targetScreen = (GameObject)_parameters[0];
                SystemEventController.Instance.DispatchSystemEvent(ScreenController.EVENT_SCREENCONTROLLER_RESPONSE_CAMERA_DATA, targetScreen, GameCamera.transform.position, GameCamera.transform.forward);
            }
        }

        public void SetCameraTo1stPerson()
        {
            ChangeState((int)CAMERA_STATES.CAMERA_1ST_PERSON);
        }

        protected virtual bool SwitchCameraState()
        {
            bool changed = false;
            if (m_inputControls.SwitchedCameraPressed())
            {
                changed = true;
                switch ((CAMERA_STATES)m_state)
                {
                    case CAMERA_STATES.CAMERA_1ST_PERSON:
                        ChangeState((int)CAMERA_STATES.CAMERA_3RD_PERSON);
                        break;

                    case CAMERA_STATES.CAMERA_3RD_PERSON:
                        ChangeState((int)CAMERA_STATES.CAMERA_1ST_PERSON);
                        break;
                }
            }
            return changed;
        }

        public bool IsFirstPersonCamera()
        {
            return m_state == (int)CAMERA_STATES.CAMERA_1ST_PERSON;
        }

        protected void CameraFollowAvatar()
        {
            Offset = Quaternion.AngleAxis(m_inputControls.GetMouseAxisHorizontal() * SPEED_ROTATION, Vector3.up) * Offset;
            if (m_player != null)
            {
                GameCamera.transform.position = m_player.GetGameObject().transform.position + Offset;
                GameCamera.transform.forward = (m_player.GetGameObject().transform.position - GameCamera.transform.position).normalized;
            }
        }

        public void FreezeCamera(bool _activateFreeze)
        {
            if (_activateFreeze)
            {
                ChangeState((int)CAMERA_STATES.CAMERA_FROZEN);
            }
            else
            {
                RestorePreviousState();
            }
        }

        protected override void ChangeState(int newState)
        {
            base.ChangeState(newState);

            switch ((CAMERA_STATES)m_state)
            {
                case CAMERA_STATES.CAMERA_1ST_PERSON:
#if !ENABLE_MOBILE
                    Cursor.lockState = CursorLockMode.Locked;
#endif
                    SystemEventController.Instance.DispatchSystemEvent(EVENT_CAMERA_SWITCHED_TO_1ST_PERSON);
                    break;

                case CAMERA_STATES.CAMERA_3RD_PERSON:
#if !ENABLE_MOBILE
                    Cursor.lockState = CursorLockMode.Locked;
#endif
                    SystemEventController.Instance.DispatchSystemEvent(EVENT_CAMERA_SWITCHED_TO_3RD_PERSON);
                    break;

                case CAMERA_STATES.CAMERA_FROZEN:
                    break;
            }
        }

        protected virtual void Update()
        {
            if ((m_player == null) || (m_inputControls == null)) return;

            switch ((CAMERA_STATES)m_state)
            {
                case CAMERA_STATES.CAMERA_1ST_PERSON:
                    SwitchCameraState();
                    if (m_player != null)
                    {
                        ContainerCamera.transform.position = m_player.PositionCamera;
                        if (m_inputControls.IsVR)
                        {
                            m_player.ForwardCamera = new Vector3(GameCamera.transform.forward.x, 0, GameCamera.transform.forward.z);
                        }
                        else
                        {
                            GameCamera.transform.forward = m_player.ForwardCamera;
                        }
                    }
                    break;

                case CAMERA_STATES.CAMERA_3RD_PERSON:
                    SwitchCameraState();
                    CameraFollowAvatar();
                    break;

                case CAMERA_STATES.CAMERA_FROZEN:
                    break;
            }
        }
    }
}