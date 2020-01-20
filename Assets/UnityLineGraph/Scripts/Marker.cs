using UnityEngine;
using UnityEngine.UI;

public class Marker : MonoBehaviour
{
    [SerializeField]
    Text m_Label;
    [SerializeField]
    Image m_LineMarker;
    [SerializeField]
    Text m_SecondValueLabel;

    // Start is called before the first frame update
    void Awake(){
        /*
        m_Label = GetComponentInChildren<Text>();
        m_LineMarker = GetComponentInChildren<Image>();
        m_SecondValueLabel = GetComponentInChildren<Text>();
        */
    }

    public void Init(string labelText, Color lineColor){
        Init(labelText, lineColor, string.Empty);
    }

    public void Init(string labelText, Color lineColor, string secondValueLabelText){
        SetLabelText(labelText);
        SetLineColor(lineColor);
        SecondValueLabel(secondValueLabelText);
    }

    public void SetLabelText(string labelText){
        m_Label.text = labelText;
    }

    public void SetLineColor(Color lineColor){
        m_LineMarker.color = lineColor;
    }
    public void SecondValueLabel(string labelText){
        if(m_SecondValueLabel != null){
            m_SecondValueLabel.text = labelText;
        }
    }
}
