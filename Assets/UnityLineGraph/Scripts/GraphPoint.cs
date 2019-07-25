using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class GraphPoint : MonoBehaviour
{
    private RectTransform m_rectTransform;

    private RectTransform m_RectTransform{
        get{
            if (m_rectTransform == null){
                m_rectTransform = this.GetComponent<RectTransform>();
            }
            return m_rectTransform;
        }
    }

    private Text m_label;
    public Text Label{
        get{
            if (m_label == null){
                m_label = this.transform.GetComponentInChildren<Text>();
            }
            return m_label;
        }
    }
    private Image m_imageComponent;
    private Image m_ImageComponent{
        get{
            if(m_imageComponent == null){
                m_imageComponent = this.GetComponent<Image>();
            }
            return m_imageComponent;
        }
    }

    public Vector2 AnchoredPosition{
        get{
            return m_RectTransform.anchoredPosition;
        }
        private set{
            m_RectTransform.anchoredPosition = value;
        }
    }

    private float m_value;
    public float Value{
        get{
            return m_value;
        }
        private set{
            m_value = value;
            Label.text = m_value.ToString();
        }
    }

    private string m_key;
    public string Key{
        get{
            return m_key;
        }
        private set{
            m_key = value;
        }
    }

    public void Set(string key, float value, Vector2 position, Color color){
        SetColor(color);
        m_RectTransform.anchorMin = Vector2.zero;
        m_RectTransform.anchorMax = Vector2.zero;
        m_RectTransform.localScale = Vector2.one;
        AnchoredPosition = position;
        m_RectTransform.SetAsLastSibling();
        name = string.Format("GraphPoint({0})", value);
        Key = key;
        Value = value;
    }

    public void SetColor(Color color){
        m_ImageComponent.color = color;
    }

    public void ShowLabel(bool show){
        Label.gameObject.SetActive(show);
    }
}
