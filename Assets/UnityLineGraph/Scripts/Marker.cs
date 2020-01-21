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

    public void SetLabelFontSize(int fontSize){
        m_Label.fontSize = fontSize;
    }

    public void SetLineColor(Color lineColor){
        m_LineMarker.color = lineColor;
    }
    public void SecondValueLabel(string labelText){
        if(m_SecondValueLabel != null){
            m_SecondValueLabel.text = labelText;
        }
    }
    public void SetSecondValueLabelFontSize(int fontSize){
        if(m_SecondValueLabel != null){
            m_SecondValueLabel.fontSize = fontSize;
        }
    }

    public void HideMarkerContents(){
        ShowMarkerContents(false);
    }
    public void ShowMarkerContents(){
        ShowMarkerContents(true);
    }
    public void ShowMarkerContents(bool show){
        m_Label.gameObject.SetActive(show);
        m_LineMarker.gameObject.SetActive(show);
        if(m_SecondValueLabel != null){
            m_SecondValueLabel.gameObject.SetActive(show);
        }
    }
}
