using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace YourVRExperience.Utils
{
    public class LanguageController : MonoBehaviour
    {
        private static LanguageController _instance;
        public static LanguageController Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = GameObject.FindObjectOfType<LanguageController>();
                }
                return _instance;
            }
        }

        public TextAsset GameTexts;
        public string CodeLanguage = "es";

        private Hashtable m_texts = new Hashtable();

        private void LoadGameTexts()
        {
            if (m_texts.Count != 0) return;
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.LoadXml(GameTexts.text);
            XmlNodeList textsList = xmlDoc.GetElementsByTagName("text");
            foreach (XmlNode textEntry in textsList)
            {
                XmlNodeList textNodes = textEntry.ChildNodes;
                string idText = textEntry.Attributes["id"].Value;
                m_texts.Add(idText, new TextEntry(idText, textNodes));
            }
        }

        public string GetText(string _id)
        {
            LoadGameTexts();
            if (m_texts[_id] != null)
            {
                return ((TextEntry)m_texts[_id]).GetText(CodeLanguage);
            }
            else
            {
                return _id;
            }
        }
    }
}