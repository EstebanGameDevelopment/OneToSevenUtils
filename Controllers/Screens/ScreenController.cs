using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YourVRExperience.Utils
{
    public class ScreenController : MonoBehaviour
    {
        public const float SIZE_VR_SCREEN = 0.002f;

        public const string EVENT_SCREENCONTROLLER_REQUEST_CAMERA_DATA = "EVENT_SCREENCONTROLLER_REQUEST_CAMERA_DATA";
        public const string EVENT_SCREENCONTROLLER_RESPONSE_CAMERA_DATA = "EVENT_SCREENCONTROLLER_RESPONSE_CAMERA_DATA";

        private static ScreenController _instance;
        public static ScreenController Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = GameObject.FindObjectOfType<ScreenController>();
                }
                return _instance;
            }
        }

        public GameObject[] Screens;

        private List<GameObject> m_screensCreated = new List<GameObject>();

        void Awake()
        {
            SystemEventController.Instance.Event += OnSystemEvent;
        }

        void OnDestroy()
        {
            if (SystemEventController.Instance != null) SystemEventController.Instance.Event -= OnSystemEvent;
        }

        private void OnSystemEvent(string _nameEvent, object[] _parameters)
        {
            if (_nameEvent == EVENT_SCREENCONTROLLER_RESPONSE_CAMERA_DATA)
            {
                GameObject targetScreen = (GameObject)_parameters[0];
                if (m_screensCreated.Contains(targetScreen))
                {
                    Vector3 positionCamera = (Vector3)_parameters[1];
                    Vector3 forwardCamera = (Vector3)_parameters[2];
                    targetScreen.transform.position = positionCamera + 1.2F * forwardCamera;
                    targetScreen.transform.forward = forwardCamera;
                    targetScreen.transform.localScale = new Vector3(SIZE_VR_SCREEN, SIZE_VR_SCREEN, SIZE_VR_SCREEN);
                }
            }
        }

        public void CreateScreen(string _nameScreen, bool _destroyPreviousScreen)
        {
            if (_destroyPreviousScreen)
            {
                DestroyScreens();
            }

            for (int i = 0; i < Screens.Length; i++)
            {
                if (Screens[i].name == _nameScreen)
                {
                    GameObject newScreen = Instantiate(Screens[i]);
                    m_screensCreated.Add(newScreen);
#if ENABLE_OCULUS || ENABLE_PICONEO || ENABLE_HTCVIVE
                    newScreen.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
#if ENABLE_OCULUS
                    newScreen.AddComponent<OVRRaycaster>();
#endif
                    SystemEventController.Instance.DispatchSystemEvent(EVENT_SCREENCONTROLLER_REQUEST_CAMERA_DATA, newScreen);
#endif
                }
            }
        }

        public void DestroyScreens()
        {
            for (int i = 0; i < m_screensCreated.Count; i++)
            {
                GameObject.Destroy(m_screensCreated[i]);
            }
            m_screensCreated.Clear();
        }
    }
}