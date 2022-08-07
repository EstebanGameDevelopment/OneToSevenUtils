using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YourVRExperience.Utils
{
    public class CameraControllerFree : CameraController
    {
        public float Sensitivity = 7F;
        private float m_rotationY = 0F;

        protected override bool SwitchCameraState()
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
                        ChangeState((int)CAMERA_STATES.CAMERA_FREE);
                        break;

                    case CAMERA_STATES.CAMERA_FREE:
                        ChangeState((int)CAMERA_STATES.CAMERA_1ST_PERSON);
                        break;
                }
            }
            return changed;
        }

        private void MoveCameraFree()
        {
            float axisVertical = m_inputControls.GetAxisVertical();
            float axisHorizontal = m_inputControls.GetAxisHorizontal();
            Vector3 forward = axisVertical * GameCamera.transform.forward * Speed * Time.deltaTime;
            Vector3 lateral = axisHorizontal * GameCamera.transform.right * Speed * Time.deltaTime;
            GameCamera.transform.position += forward + lateral;
        }

        private void RotateCameraFree()
        {
            float rotationX = GameCamera.transform.localEulerAngles.y + m_inputControls.GetMouseAxisHorizontal() * Sensitivity;
            m_rotationY = m_rotationY + m_inputControls.GetMouseAxisVertical() * Sensitivity;
            m_rotationY = Mathf.Clamp(m_rotationY, -60, 60);
            Quaternion rotation = Quaternion.Euler(-m_rotationY, rotationX, 0);
            GameCamera.transform.forward = rotation * Vector3.forward;
        }


        protected override void ChangeState(int newState)
        {
            base.ChangeState(newState);

            switch ((CAMERA_STATES)m_state)
            {
                case CAMERA_STATES.CAMERA_FREE:
                    Cursor.lockState = CursorLockMode.None;
                    SystemEventController.Instance.DispatchSystemEvent(EVENT_CAMERA_SWITCHED_TO_FREE_CAMERA);
                    break;
            }
        }


        protected override void Update()
        {
            if ((m_player == null) || (m_inputControls == null)) return;

            switch ((CAMERA_STATES)m_state)
            {
                case CAMERA_STATES.CAMERA_FREE:
                    if (SwitchCameraState()) return;
                    bool shouldRun = Input.GetKey(KeyCode.LeftShift);
#if ENABLE_MOBILE
                shouldRun = true;
#endif
                    if (shouldRun)
                    {
                        MoveCameraFree();
                        RotateCameraFree();
                    }
                    break;
            }

            base.Update();
        }
    }
}