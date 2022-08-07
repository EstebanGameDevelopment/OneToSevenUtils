using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YourVRExperience.Utils
{
    public class StateMachine : MonoBehaviour
    {
        protected int m_state = 0;
        protected int m_previousState = 0;

        protected float m_timeCounter = 0;

        public int State
        {
            get { return m_state; }
            set { m_state = value; }
        }

        protected virtual void ChangeState(int newState)
        {
            m_previousState = m_state;
            m_state = newState;
            m_timeCounter = 0;
        }

        protected virtual void RestorePreviousState()
        {
            m_state = m_previousState;
            m_timeCounter = 0;
        }
    }
}