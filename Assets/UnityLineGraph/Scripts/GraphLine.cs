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
        [SerializeField]
        private Color m_pointColor = Color.red;
        private List<KeyValuePair<string, float>> valueList = new List<KeyValuePair<string, float>>();
        [SerializeField]
        private Color m_lineColor = Color.red;
        //public LineGraphController.LineGraphSettings settings;

        public LineGraphController parentController;

        public Spawner PointSpawner;
        public Spawner SubLineSpawner;

        public GraphPoint EndPoint{get; private set;}
        public class OnValueAddedEvent : UnityEvent<float, float> {}
        public OnValueAddedEvent OnValueAdded = new OnValueAddedEvent();        

        private float m_LineThickness = 2f;
        public float LineThickness{
            get{
                return m_LineThickness;
            } set{
                m_LineThickness = value;
                var sublines = SubLineSpawner.SpawnedGameObjects;
                for (int i = 0; i < sublines.Count; i++){
                    var rt = sublines[i].GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(rt.sizeDelta.x, m_LineThickness);
                }
            }
        }

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

        public int ValueCount{
            get{
                return valueList.Count;
            }
        }

        private bool m_ShowLabels = true;
        public bool ShowLabels{
            get{
                return m_ShowLabels;
            }
            set{
                m_ShowLabels = value;
                var points = PointSpawner.SpawnedGameObjects;
                for (int i = 0; i < points.Count; i++){
                    var gp = points[i].GetComponent<GraphPoint>();
                    gp.ShowLabel(m_ShowLabels);
                }
            }
        }

        public void SetColors(Color color){
            SetColors(color, color);
        }
        public void SetColors(Color lineColor, Color pointColor){
            m_lineColor = lineColor;
            m_pointColor = pointColor;

            var points = PointSpawner.SpawnedGameObjects;
            for (int i = 0; i < points.Count; i++){
                var gp = points[i].GetComponent<GraphPoint>();
                gp.SetColor(pointColor);
            }

            var sublines = SubLineSpawner.SpawnedGameObjects;
            for (int i = 0; i < sublines.Count; i++){
                sublines[i].GetComponent<Image>().color = lineColor;
            }
        }

        /// <summary>
        /// 新しい点を作成する
        /// </summary>
        /// <returns>The new dot.</returns>
        /// <param name="index">X軸方向で何個目か</param>
        /// <param name="value">Y軸方向の値</param>
        private GraphPoint GenerateNewPoint(int index = -1)
        {
            int plv_index = m_EndPointIndex;
            if(index > -1){
                plv_index = index;
            }
            var plv = valueList[plv_index];
            var lbl_index = parentController.xAxisLabels.IndexOf(plv.Key);
            var point = PointSpawner.SpawnAndGetGameObject().GetComponent<GraphPoint>();
            //var pointX = ((plv_index + 1 / 2) * parentController.Settings.xSize) + parentController.Settings.xSize;
            var pointX = ((lbl_index + 1 / 2) * parentController.xPixelsPerUnit) + parentController.xPixelsPerUnit;
            var pointY = (plv.Value - parentController.OffsetY)  * parentController.yPixelsPerUnit;
            var pointPosition = new Vector2(pointX, pointY);

            point.Set(plv.Key, plv.Value, pointPosition, m_pointColor);
            point.ShowLabel(ShowLabels);

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
            lineRectTransform.sizeDelta = new Vector2(distance, LineThickness);
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
            EndPoint = null;
        }

        public void Clear(){
            ClearUI();
            ClearData();
        }

        public void Generate(){
            ClearUI();

            for (int x = 0; x < valueList.Count; x++)//+= settings.valueSpan)
            {
                GenerateConnectedPoint(x);
            }
        }

        public void GenerateConnectedPoint(int index = -1){
                var newPoint = GenerateNewPoint(index);

                // If not first point
                if (EndPoint != null)
                {
                    GenerateSubLine(EndPoint, newPoint);
                }

                EndPoint = newPoint;
        }

        public void AddValue(string label, float value){
            valueList.Add(new KeyValuePair<string, float>(label, value));
        }

        public void AddAndGenerateValue(string label, float value){
            AddValue(label, value);
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
                return parentController.yAxisUnitSpan;//0;
            }

            for(int i = 0;i < valueList.Count; i++) //+= settings.valueSpan)
            {
                max = Mathf.Max(max, valueList[i].Value);
            }

            return max;
        }
        public float GetSepMaxY(){
            float max = GetMaxY();
            float sepMax = float.MinValue;
            int sepCount = (int)(max / parentController.yAxisUnitSpan);
            while (sepMax < max){
               sepMax = Mathf.Max(max, sepCount * parentController.yAxisUnitSpan);
               sepCount++; 
            }

            return sepMax + parentController.yAxisUnitSpan;
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

        public float GetSepMinY(){
            float min = GetMinY();
            float sepMin = float.MaxValue;
            int sepCount = (int)(min / parentController.yAxisUnitSpan);
            while (sepMin > min || sepMin % parentController.yAxisUnitSpan != 0){
               sepMin = Mathf.Min(min, sepCount * parentController.yAxisUnitSpan);
               sepCount--; 
            }

            return sepMin - parentController.yAxisUnitSpan;
        }

    }
}