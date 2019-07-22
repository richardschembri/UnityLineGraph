namespace UnityLineGraph
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;
    using RSToolkit.Controls;

    public class GraphLine : MonoBehaviour
    {
        private RectTransform content;
        private Color m_pointColor;
        private List<KeyValuePair<string, float>> valueList = new List<KeyValuePair<string, float>>();
        private Color m_lineColor;
        public LineGraphController.LineGraphSettings settings;

        public Spawner PointSpawner;
        public Spawner SubLineSpawner;

        private GraphPoint endPoint;
        public class OnValueAddedEvent : UnityEvent<float, float> {}
        public OnValueAddedEvent OnValueAdded = new OnValueAddedEvent();        
/* 
        private LineGraphController m_parentController;
        public LineGraphController ParentController{
            get{
                return m_parentController;
            }
            set{
                m_parentController = value;
            }
        }
*/
        private int m_EndPointIndex{
            get{
                return valueList.Count - 1;
            }
        }

        private  KeyValuePair<string, float> m_EndPointLabelValue{
            get{
                return valueList[m_EndPointIndex];
            }
        }

        public void SetColors(Color lineColor, Color pointColor){
            m_lineColor = lineColor;
            m_pointColor = pointColor;
        }

        /// <summary>
        /// 新しい点を作成する
        /// </summary>
        /// <returns>The new dot.</returns>
        /// <param name="index">X軸方向で何個目か</param>
        /// <param name="value">Y軸方向の値</param>
        private GraphPoint GenerateNewPoint()
        {
            
            var point = PointSpawner.SpawnAndGetGameObject().GetComponent<GraphPoint>();
            /*
            var pointPosition = 
                new Vector2((m_EndPointIndex / settings.valueSpan + 1) * settings.xSize,
                        m_EndPointLabelValue.Value * settings.ySize);
            */
            var pointX = ((m_EndPointIndex + 1 / 2) * settings.xSize) + settings.xSize;
            var pointPosition = 
                new Vector2(pointX,
                        m_EndPointLabelValue.Value * settings.ySize);

            point.Set(m_EndPointLabelValue.Key, m_EndPointLabelValue.Value, pointPosition, m_pointColor);

            return point;
        }

        private void GenerateSubLine(GraphPoint from, GraphPoint to){
            GenerateSubLine(from.AnchoredPosition, to.AnchoredPosition);
        }

        /// <summary>
        /// 点と点をつなぐ線を作成する
        /// </summary>
        /// <param name="from">点1の位置</param>
        /// <param name="to">点2の位置</param>
        private void GenerateSubLine(Vector2 from, Vector2 to)
        {
            var line = SubLineSpawner.SpawnAndGetGameObject();
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
            //lineRectTransform.SetSiblingIndex((int)SortOrder.LINE);
            lineRectTransform.SetAsFirstSibling();
        }

        public void ClearUI(){
            SubLineSpawner.DestroyAllSpawns();
            PointSpawner.DestroyAllSpawns();
        }

        public void ClearData(){
            valueList.Clear();
        }

        public void Clear(){
            ClearUI();
            ClearData();
        }

        public void Generate(){
            ClearUI();

            for (int x = 0; x < valueList.Count; x++)//+= settings.valueSpan)
            {
                GenerateConnectedPoint();
            }
        }

        public void GenerateConnectedPoint(){
                var newPoint = GenerateNewPoint();

                // If not first point
                if (endPoint != null)
                {
                    GenerateSubLine(endPoint, newPoint);
                }

                endPoint = newPoint;
        }

        public void AddValue(string label, float value){
            valueList.Add(new KeyValuePair<string, float>(label, value));
            GenerateConnectedPoint();
        }

    /// <summary>
    /// 現在の最大値を取得する
    /// </summary>
    /// <returns>最大値</returns>
        public float GetMaxY()
        {
            
            //return valueList.Max(kv => kv.Value);

            float max = float.MinValue;

            if(valueList.Count == 0)
            {
                return settings.yAxisSeparatorSpan;//0;
            }

            for(int i = 0;i < valueList.Count; i++) //+= settings.valueSpan)
            {
                max = Mathf.Max(max, valueList[i].Value);
            }

            return max;
        }

        public float GetMinY(){
            float min = float.MaxValue;
            if(valueList.Count == 0)
            {
                min = 0;
            }

            for(int i = 0;i < valueList.Count; i++) //+= settings.valueSpan)
            {
                min = Mathf.Min(min, valueList[i].Value);
            }

            return min;
        }

    }
}