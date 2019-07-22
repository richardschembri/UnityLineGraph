namespace UnityLineGraph
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class XMarker : MonoBehaviour
    {
        Text m_Label;
        void Awake(){
            m_Label = GetComponentInChildren<Text>();
        }
        public void SetLabelText(string labelText){

            m_Label.text = labelText;
        }
        public string GetLabelText(){ 
            return m_Label.text;
        }
    }
}
