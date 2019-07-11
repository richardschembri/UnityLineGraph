using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using RSToolkit.Controls;

public class LineGraphController : MonoBehaviour
{
    [SerializeField]
    private Sprite dotSprite;
    [SerializeField]
    private Font font;


    // グラフを表示する範囲
    private RectTransform viewport;
    // グラフの要素を配置するContent
    // グラフの要素はグラフの点、ライン
    private RectTransform content;
    public Spawner YmarkerContent;
    public Spawner XmarkerContent;
    // 軸のGameObject
    private GameObject xUnitLabel;
    private GameObject yUnitLabel;

    private LineGraphSettings settings;
    private GameObject previousDot;

    private List<KeyValuePair<string, float>> valueList;

    public struct LineGraphSettings{
        public float xSize;
        public float ySize;
        public float yAxisSeparatorSpan;
        public int valueSpan;
        public Color dotColor;
        public Color connectionColor;
        public bool autoScroll;

        public float seperatorThickness;

        public static LineGraphSettings Default
        {
            get
            {
                return new LineGraphSettings()
                {
                    xSize = 50,
                    ySize = 5,
                    yAxisSeparatorSpan = 10,
                    valueSpan = 1,
                    dotColor = Color.white,
                    connectionColor = Color.white,
                    autoScroll = true,
                    seperatorThickness = 2f,
                };
            }
        }
    }

    private enum ZOrder
    {
        AXIS_SEPARATOR,
        CONNECTION,
        DOT,
        LABEL
    };

    private void Awake()
    {
        viewport = this.transform.Find("Viewport") as RectTransform;
        content = viewport.Find("Content") as RectTransform;
        xUnitLabel = this.transform.Find("X Unit Label").gameObject;
        yUnitLabel = this.transform.Find("Y Unit Label").gameObject;
        valueList = new List<KeyValuePair<string, float>>();
        settings = LineGraphSettings.Default;
    }

    private void Start()
    {
        CreateYAxisSeparatorFitGraph();
        FixLabelAndAxisSeparatorPosition();
    }

    /// <summary>
    /// 値を追加する
    /// </summary>
    /// <param name="label">ラベルの文字列</param>
    /// <param name="value">値</param>
    public void AddValue(string label, float value)
    {
        valueList.Add(new KeyValuePair<string, float>(label, value));

        // 点を追加する
        if (settings.valueSpan == 1 || valueList.Count % settings.valueSpan == 1)
        {
            int index = valueList.Count - 1;
            GameObject dot = CreateNewDot(index, value);

            if(previousDot != null)
            {
                RectTransform rectTransform1 =
                    previousDot.GetComponent<RectTransform>();
                RectTransform rectTransform2 =
                    dot.GetComponent<RectTransform>();

                CreateConnection(
                        rectTransform1.anchoredPosition,
                        rectTransform2.anchoredPosition);
            }

            CreateValueLabelByDot(index, value);
            CreateXMarker(index, label);

            previousDot = dot;

            FixContentSize();

            // Yセパレータの更新
            CreateYAxisSeparatorFitGraph();
            FixLabelAndAxisSeparatorPosition();

            if (settings.autoScroll)
            {
                RectTransform rect = dot.GetComponent<RectTransform>();

                CapturePoint(rect.localPosition);
            }
        }
    }

    /// <summary>
    /// X軸方向の単位を設定
    /// </summary>
    /// <param name="text">Text.</param>
    public void SetXUnitText(string text)
    {
        xUnitLabel.GetComponent<Text>().text = text;
    }

    /// <summary>
    /// Y軸方向の単位を設定
    /// </summary>
    /// <param name="text">Text.</param>
    public void SetYUnitText(string text)
    {
        yUnitLabel.GetComponent<Text>().text = text;
    }

    /// <summary>
    /// グラフに関するパラメータを全て変更する
    /// </summary>
    /// <param name="xSize">X軸方向の幅</param>
    /// <param name="ySize">Y軸方向の幅s</param>
    /// <param name="yAxisSeparatorSpan">Y軸セパレータの間隔</param>
    /// <param name="valueSpan">X軸方向の点を表示する間隔</param>
    /// <param name="autoScroll">自動的にスクロールするか</param>
    public void ChangeParam(float xSize, float ySize, int yAxisSeparatorSpan, int valueSpan, bool autoScroll)
    {
        ChangeSettings(new LineGraphSettings()
        {
            xSize = xSize,
            ySize = ySize,
            yAxisSeparatorSpan = yAxisSeparatorSpan,
            valueSpan = valueSpan,
            autoScroll = autoScroll
        });
    }

    /// <summary>
    /// LineGraphParameterでパラメータを変更する
    /// </summary>
    /// <param name="param">Parameter.</param>
    public void ChangeSettings(LineGraphSettings param)
    {
        this.settings = param;

        RefreshGraph();
    }

    public LineGraphSettings GetParameter()
    {
        return settings;
    }

    /// <summary>
    /// グラフがスクロールされた時の処理
    /// </summary>
    /// <param name="scrollPosition">スクロールの位置</param>
    public void OnGraphScroll(Vector2 scrollPosition)
    {
        FixLabelAndAxisSeparatorPosition();
    }

    /// <summary>
    /// 新しい点を作成する
    /// </summary>
    /// <returns>The new dot.</returns>
    /// <param name="index">X軸方向で何個目か</param>
    /// <param name="value">Y軸方向の値</param>
    private GameObject CreateNewDot(int index, float value)
    {
        GameObject dot = new GameObject("dot", typeof(Image));
        Image image = dot.GetComponent<Image>();
        image.useSpriteMesh = true;
        image.sprite = dotSprite;
        image.color = settings.dotColor;
        RectTransform rectTransform = dot.GetComponent<RectTransform>();
        rectTransform.SetParent(content);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        rectTransform.localScale = Vector2.one;
        rectTransform.sizeDelta = new Vector2(5, 5);
        rectTransform.anchoredPosition =
            new Vector2((index / settings.valueSpan + 1) * settings.xSize,
                    value * settings.ySize);
        rectTransform.SetSiblingIndex((int)ZOrder.DOT);

        return dot;
    }

    /// <summary>
    /// 点と点をつなぐ線を作成する
    /// </summary>
    /// <param name="pos1">点1の位置</param>
    /// <param name="pos2">点2の位置</param>
    private void CreateConnection(Vector2 pos1, Vector2 pos2)
    {
        GameObject connection = new GameObject("connection", typeof(Image));
        connection.GetComponent<Image>().color = settings.connectionColor;
        RectTransform rectTransform = connection.GetComponent<RectTransform>();
        rectTransform.SetParent(content);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        rectTransform.localScale = Vector2.one;
        Vector2 dir = (pos2 - pos1).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float distance = Vector2.Distance(pos1, pos2);
        rectTransform.sizeDelta = new Vector2(distance, 2);
        rectTransform.localEulerAngles = new Vector3(0, 0, angle);
        rectTransform.anchoredPosition = pos1 + dir * distance * 0.5f;
        rectTransform.SetSiblingIndex((int)ZOrder.CONNECTION);
    }

    /// <summary>
    /// 点の近くに値のラベルを表示する
    /// </summary>
    /// <param name="x">X軸方向で何個目か</param>
    /// <param name="y">Y軸方向の値</param>
    private void CreateValueLabelByDot(int x, float y)
    {
        GameObject label = new GameObject("label", typeof(Text));
        Text text = label.GetComponent<Text>();
        text.text = y.ToString();
        text.alignment = TextAnchor.MiddleCenter;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.fontSize = 10;
        text.font = font;
        text.color = Color.black;
        Vector2 offset = new Vector2(0, 8);
        RectTransform rectTransform = label.GetComponent<RectTransform>();
        rectTransform.SetParent(content);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        rectTransform.localScale = Vector2.one;
        rectTransform.anchoredPosition =
            new Vector2((x / settings.valueSpan + 1) * settings.xSize,
                    y * settings.ySize) + offset;
        rectTransform.SetSiblingIndex((int)ZOrder.LABEL);
    }

    /// <summary>
    /// Contentのサイズを調整する
    /// </summary>
    private void FixContentSize()
    {
        Vector2 buffer = new Vector2(10, 10);
        float width = (valueList.Count / settings.valueSpan + 1) * settings.xSize;
        //float height = ((GetMaxY() + (settings.yAxisSeparatorSpan * 1.75f)) * settings.ySize) + settings.seperatorThickness;
        int sepCount = Mathf.CeilToInt(GetMaxY() / settings.yAxisSeparatorSpan) + 1;
        Debug.Log(sepCount);
        float height = (settings.yAxisSeparatorSpan * sepCount * settings.ySize)
                        - ((settings.yAxisSeparatorSpan / 4) * settings.ySize)
                        + settings.seperatorThickness;

        content.sizeDelta = new Vector2(width, height) + buffer;
        YmarkerContent.GetComponent<RectTransform>().sizeDelta = new Vector2(YmarkerContent.GetComponent<RectTransform>().sizeDelta.x, content.sizeDelta.y);
    }

    /// <summary>
    /// 現在の最大値を取得する
    /// </summary>
    /// <returns>最大値</returns>
    private float GetMaxY()
    {
        //return valueList.Max(kv => kv.Value);

        float max = float.MinValue;

        if(valueList.Count == 0)
        {
            return settings.yAxisSeparatorSpan;//0;
        }

        for(int i = 0;i < valueList.Count; i += settings.valueSpan)
        {
            max = Mathf.Max(max, valueList[i].Value);
        }

        return max;
    }

    private void CreateXMarker(int index, string labelText){
        var xMarker = XmarkerContent.SpawnAndGetGameObject().GetComponent<XMarker>();
        xMarker.SetLabelText(labelText);
        XmarkerContent.GetComponent<RectTransform>().sizeDelta = new Vector2((index + 1) * settings.xSize, 0);
    }

    private void CreateYMarker(float y)
    {
        var marker = YmarkerContent.SpawnAndGetGameObject().GetComponent<YMarker>();
        marker.Init(y.ToString(), new Color(0, 0, 0, 0.5f));
        marker.transform.SetAsFirstSibling();
        marker.name = "YMarker(" + y + ")";
    }

    /// <summary>
    /// Y軸のセパレータを今のグラフに合わせて表示する
    /// </summary>
    private void CreateYAxisSeparatorFitGraph()
    {
        //RectTransform yAxisRect = yAxis.GetComponent<RectTransform>();
        float height = YmarkerContent.GetComponent<RectTransform>().sizeDelta.y; //yAxisRect.sizeDelta.x;
        //float height = content.sizeDelta.y; //yAxisRect.sizeDelta.x;
        // スクロールしていない時に表示できるY軸方向の最大値
        float maxValueNotScroll = (height / settings.ySize);

        float maxValue = GetMaxY();
        int separatorMax = (int)Mathf.Max(maxValue, maxValueNotScroll);// + (int)settings.yAxisSeparatorSpan;


        for(float y = 0; y <= separatorMax; y += settings.yAxisSeparatorSpan)
        {
            string markerName = "YMarker(" + y + ")";

            // 存在したら追加しない
            if (YmarkerContent.transform.Find(markerName) == null)
            {
                CreateYMarker(y);
            }
        }
    }

    /// <summary>
    /// グラフ外のラベルと軸セパレータの位置を更新
    /// </summary>
    private void FixLabelAndAxisSeparatorPosition()
    {
        Vector2 contentPosition = content.anchoredPosition;
        YmarkerContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(YmarkerContent.GetComponent<RectTransform>().anchoredPosition.x, content.anchoredPosition.y + settings.seperatorThickness);
        XmarkerContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(content.anchoredPosition.x,  XmarkerContent.GetComponent<RectTransform>().anchoredPosition.y);

    }

    /// <summary>
    /// グラフの表示を更新する
    /// </summary>
    private void RefreshGraph()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        previousDot = null;
        YmarkerContent.DestroyAllSpawns();
        XmarkerContent.DestroyAllSpawns();

        for (int x = 0; x < valueList.Count; x += settings.valueSpan)
        {
            string label = valueList[x].Key;
            float y = valueList[x].Value;
            GameObject dot = CreateNewDot(x, y);

            if (previousDot != null)
            {
                RectTransform rectTransform1 =
                    previousDot.GetComponent<RectTransform>();
                RectTransform rectTransform2 =
                    dot.GetComponent<RectTransform>();

                CreateConnection(
                        rectTransform1.anchoredPosition,
                        rectTransform2.anchoredPosition);
            }

            CreateValueLabelByDot(x, y);
            CreateXMarker(x, label);

            previousDot = dot;
        }
        FixContentSize();

        // Yセパレータの更新
        CreateYAxisSeparatorFitGraph();
        FixLabelAndAxisSeparatorPosition();
    }

    /// <summary>
    /// ある点をグラフの中央になるようにスクロールする
    /// </summary>
    private void CapturePoint(Vector2 position)
    {
        Vector2 viewportSize =
            new Vector2(viewport.rect.width, viewport.rect.height);
        Vector2 contentSize =
            new Vector2(content.rect.width, content.rect.height);

        Vector2 contentPosition = -position + 0.5f * viewportSize;

        if(contentSize.x < viewportSize.x)
        {
            contentPosition.x = 0.0f;
        }
        else
        {
            contentPosition.x = Mathf.Clamp(contentPosition.x, -contentSize.x + viewportSize.x, 0);
        }

        if(contentSize.y < viewportSize.y)
        {
            contentPosition.y = 0.0f;
        }
        else
        {
            contentPosition.y = Mathf.Clamp(contentPosition.y, -contentSize.y + viewportSize.y, 0);
        }

        content.localPosition = contentPosition;
        YmarkerContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(YmarkerContent.GetComponent<RectTransform>().anchoredPosition.x, content.anchoredPosition.y + settings.seperatorThickness);
    }
}
