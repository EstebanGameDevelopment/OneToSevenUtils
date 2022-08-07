
using UnityEngine;

namespace YourVRExperience.Utils
{
    public class LookAtTarget : MonoBehaviour
    {
        private Transform m_target;

        public void SetTarget(Transform _target)
        {
            m_target = _target;
        }

        void Update()
        {
            if (m_target != null)
            {
                transform.rotation = Quaternion.LookRotation(m_target.forward);
            }
        }
    }
}