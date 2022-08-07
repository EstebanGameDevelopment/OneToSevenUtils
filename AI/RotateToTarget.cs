using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YourVRExperience.Utils
{
    public class RotateToTarget : MonoBehaviour
    {
        public const string EVENT_ROTATETOTARGET_HAS_STARTED = "EVENT_ROTATETOTARGET_HAS_STARTED";
        public const string EVENT_ROTATETOTARGET_HAS_BEEN_DESTROYED = "EVENT_ROTATETOTARGET_HAS_BEEN_DESTROYED";

        public Vector3 Target;
        public float RotationSpeed = 1;
        private bool m_activated = false;

        void Start()
        {
            SystemEventController.Instance.DispatchSystemEvent(EVENT_ROTATETOTARGET_HAS_STARTED, this);
        }

        void OnDestroy()
        {
            if (SystemEventController.Instance != null) SystemEventController.Instance.DispatchSystemEvent(EVENT_ROTATETOTARGET_HAS_BEEN_DESTROYED, this);
        }

        public void ActivateRotation(Vector3 _target)
        {
            Target = _target;
            m_activated = true;
        }

        public void DeactivateRotation()
        {
            m_activated = false;
        }

        public void UpdateLogic()
        {
            if (m_activated == false) return;

            Vector3 lookPos = Target - transform.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * RotationSpeed);
        }
    }
}