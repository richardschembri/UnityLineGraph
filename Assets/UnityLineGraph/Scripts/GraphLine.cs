namespace UnityLineGraph
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using RSToolkit.Controls;

    public class GraphLine : MonoBehaviour
    {
        [SerializeField]
        private Sprite m_pointSprite;
        private RectTransform content;
        private Color m_pointColor;
        private List<KeyValuePair<string, float>> valueList;
        private Color m_lineColor;
        public LineGraphController.LineGraphSettings settings;

        public Spawner PointLabelSpawner;
        public Spawner PointSpawner;
        public Spawner LineSpawner;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        /// <summary>
        /// 新しい点を作成する
        /// </summary>
        /// <returns>The new dot.</returns>
        /// <param name="index">X軸方向で何個目か</param>
        /// <param name="value">Y軸方向の値</param>
        private GameObject GenerateNewPoint(int index, float value)
        {
            var point = PointSpawner.SpawnAndGetGameObject();
            var pointImage = point.GetComponent<Image>();
            pointImage.color = m_pointColor;
            var pointRectTransform = point.GetComponent<RectTransform>();
            pointRectTransform.anchorMin = Vector2.zero;
            pointRectTransform.anchorMax = Vector2.zero;
            pointRectTransform.localScale = Vector2.one;
            pointRectTransform.anchoredPosition =
                new Vector2((index / settings.valueSpan + 1) * settings.xSize,
                        value * settings.ySize);
            pointRectTransform.SetSiblingIndex((int)SortOrder.POINT);

            return point;
        }
        /// <summary>
        /// 点と点をつなぐ線を作成する
        /// </summary>
        /// <param name="from">点1の位置</param>
        /// <param name="to">点2の位置</param>
        private void GenerateLine(Vector2 from, Vector2 to)
        {
            var line = LineSpawner.SpawnAndGetGameObject();
            line.GetComponent<Image>().color = m_lineColor;
            var lineRectTransform = line.GetComponent<RectTransform>();
            lineRectTransform.anchorMin = Vector2.zero;
            lineRectTransform.anchorMax = Vector2.zero;
            lineRectTransform.localScale = Vector2.one;
            Vector2 dir = (to - from).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float distance = Vector2.Distance(from, to);
            lineRectTransform.sizeDelta = new Vector2(distance, 2);
            lineRectTransform.localEulerAngles = new Vector3(0, 0, angle);
            lineRectTransform.anchoredPosition = from + dir * distance * 0.5f;
            lineRectTransform.SetSiblingIndex((int)SortOrder.LINE);
        }

        /// <summary>
        /// 点の近くに値のラベルを表示する
        /// </summary>
        /// <param name="x">X軸方向で何個目か</param>
        /// <param name="y">Y軸方向の値</param>
        private void GeneratePointLabel(int x, float y)
        {
            var pointLabel = PointLabelSpawner.SpawnAndGetGameObject();
            var pointLabelText = pointLabel.GetComponent<Text>();
            pointLabelText.text = y.ToString(); 
            var offset = new Vector2(0, 8);
            var pointLabelRectTransform = pointLabel.GetComponent<RectTransform>();
            pointLabelRectTransform.SetParent(content);
            pointLabelRectTransform.anchorMin = Vector2.zero;
            pointLabelRectTransform.anchorMax = Vector2.zero;
            pointLabelRectTransform.localScale = Vector2.one;
            pointLabelRectTransform.anchoredPosition =
                new Vector2((x / settings.valueSpan + 1) * settings.xSize,
                        y * settings.ySize) + offset;
            pointLabelRectTransform.SetSiblingIndex((int)SortOrder.LABEL);

        }

        private enum SortOrder
        {
            LINE = 0,
            POINT = 1,
            LABEL = 2
        };
    }
}