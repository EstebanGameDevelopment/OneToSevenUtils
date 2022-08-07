using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YourVRExperience.Utils
{
    public class SystemEventController : ScriptableObject
    {
        private static SystemEventController _instance;
        public static SystemEventController Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = ScriptableObject.CreateInstance<SystemEventController>();
                }
                return _instance;
            }
        }

        public delegate void SystemEvent(string _nameEvent, params object[] _parameters);

        public event SystemEvent Event;

        public void DispatchSystemEvent(string _nameEvent, params object[] _parameters)
        {
            if (Event != null) Event(_nameEvent, _parameters);
        }

    }
}