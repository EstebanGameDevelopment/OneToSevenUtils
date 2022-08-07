using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace YourVRExperience.Utils
{
    public class TouchJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public enum ConfigurationDirections { ALL, HORIZONTAL, VERTICAL }

        [SerializeField] private float dotRange = 1;
        [SerializeField] private float zonaMuerta = 0;
        [SerializeField] private ConfigurationDirections directionOptions = ConfigurationDirections.ALL;

        [SerializeField] protected RectTransform bg = null;
        [SerializeField] private RectTransform dot = null;
        [SerializeField] private float limitMovement = 1;

        private Canvas m_joystickCanvas;
        private Camera m_joystickCamera;

        private Vector2 m_joystickInput = Vector2.zero;
        private Vector2 m_staticPosition = Vector2.zero;

        public float DotRange
        {
            get { return dotRange; }
            set { dotRange = Mathf.Abs(value); }
        }

        public float ZonaMuerta
        {
            get { return zonaMuerta; }
            set { zonaMuerta = Mathf.Abs(value); }
        }

        public float Horizontal
        {
            get { return m_joystickInput.x; }
        }
        public float Vertical
        {
            get { return m_joystickInput.y; }
        }
        public Vector2 Direction
        {
            get { return new Vector2(Horizontal, Vertical); }
        }

        public ConfigurationDirections DirectionConfig
        {
            get { return DirectionConfig; }
            set { directionOptions = value; }
        }
        void Start()
        {
            DotRange = dotRange;
            ZonaMuerta = zonaMuerta;
            m_joystickCanvas = GetComponentInParent<Canvas>();

            Vector2 center = new Vector2(0.5f, 0.5f);
            bg.pivot = center;
            dot.anchorMin = center;
            dot.anchorMax = center;
            dot.pivot = center;
            dot.anchoredPosition = Vector2.zero;

            m_staticPosition = bg.anchoredPosition;
            bg.anchoredPosition = m_staticPosition;
            bg.gameObject.SetActive(true);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            m_joystickInput = Vector2.zero;
            dot.anchoredPosition = Vector2.zero;
        }

        public void OnDrag(PointerEventData eventData)
        {
            m_joystickCamera = null;
            if (m_joystickCanvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                m_joystickCamera = m_joystickCanvas.worldCamera;
            }

            Vector2 position = RectTransformUtility.WorldToScreenPoint(m_joystickCamera, bg.position);
            Vector2 radius = bg.sizeDelta / 2;
            m_joystickInput = (eventData.position - position) / (radius * m_joystickCanvas.scaleFactor);
            if (directionOptions == ConfigurationDirections.HORIZONTAL)
            {
                m_joystickInput = new Vector2(m_joystickInput.x, 0f);
            }
            else
            {
                if (directionOptions == ConfigurationDirections.VERTICAL)
                {
                    m_joystickInput = new Vector2(0f, m_joystickInput.y);
                }
            }
            if (m_joystickInput.magnitude > zonaMuerta)
            {
                if (m_joystickInput.magnitude > 1)
                {
                    m_joystickInput = m_joystickInput.normalized;
                }
            }
            else
            {
                m_joystickInput = Vector2.zero;
            }
            dot.anchoredPosition = m_joystickInput * radius * dotRange;
        }
    }
}