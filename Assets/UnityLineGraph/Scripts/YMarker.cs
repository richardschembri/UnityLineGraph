using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YMarker : MonoBehaviour
{
    Text m_Label;
    Image m_LineMarker;

    void Awake(){
        m_Label = GetComponentInChildren<Text>();
        m_LineMarker = GetComponentInChildren<Image>();
    }

    public void Init(string labelText, Color lineColor){
        SetLabelText(labelText);
        SetLineColor(lineColor);
    }

    public void SetLabelText(string labelText){

        m_Label.text = labelText;
    }

    public void SetLineColor(Color lineColor){
        m_LineMarker.color = lineColor;
    }

}
