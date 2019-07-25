namespace UnityLineGraph
{
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

        private GameObject previousDot;

        private List<KeyValuePair<string, float>> valueList;

        public List<GraphLine> GraphLines{
            get{
                return m_GraphLineSpawner.SpawnedGameObjects.Select(gl => gl.GetComponent<GraphLine>()).ToList();
            }
        }

        public float xSize = 50f;
        public float ySize = 5f;
        public float yAxisValueSpan = 10f;
        public bool AutoScroll = true;
        public float seperatorThickness = 2f;
        public List<string> xAxisLabels;

        private float m_OffsetY = 0f;

        public float OffsetY{
            get{
                return m_OffsetY;
            }
            private set{
                m_OffsetY = value;
            }
        }

        public void ResetSettings(){
            xSize = 50f;
            ySize = 5f;
            yAxisValueSpan = 10f;
            AutoScroll = true;
            seperatorThickness = 2f;
        }

        private void Awake()
        {
            viewport = this.transform.Find("Viewport") as RectTransform;
            content = viewport.Find("Content") as RectTransform;
            xUnitLabel = this.transform.Find("X Unit Label").gameObject;
            yUnitLabel = this.transform.Find("Y Unit Label").gameObject;
            valueList = new List<KeyValuePair<string, float>>();
        }

        private void Start()
        {
        }

        /// <summary>
        /// X軸方向の単位を設定
        /// </summary>
        /// <param name="text">Text.</param>
        public void SetXUnitText(string text)
        {
            if(xUnitLabel != null){
                xUnitLabel.GetComponent<Text>().text = text;
            }
        }

        /// <summary>
        /// Y軸方向の単位を設定
        /// </summary>
        /// <param name="text">Text.</param>
        public void SetYUnitText(string text)
        {
            if(yUnitLabel != null){
                yUnitLabel.GetComponent<Text>().text = text;
            }
        }

        private Spawner m_graphLineSpawner;
        public Spawner m_GraphLineSpawner{
            get{
                if(m_graphLineSpawner == null){
                    m_graphLineSpawner = content.GetComponent<Spawner>();
                }
                return m_graphLineSpawner;
            }
        }

        public GraphLine AddGraphLine(Color color){
            return AddGraphLine(color, color);
        }
        public GraphLine AddGraphLine(Color lineColor, Color pointColor){
            var graphLine = m_GraphLineSpawner.SpawnAndGetGameObject().GetComponent<GraphLine>();
            //graphLine.settings = Settings;
            graphLine.parentController = this;
            graphLine.SetColors(lineColor, pointColor);
            return graphLine;
        }

        /// <summary>
        /// グラフがスクロールされた時の処理
        /// </summary>
        /// <param name="scrollPosition">スクロールの位置</param>
        public void OnGraphScroll(Vector2 scrollPosition)
        {
            UpdateMakersPosition();
        }

        /// <summary>
        /// Contentのサイズを調整する
        /// </summary>
        private void ResizeContentContainer()
        {
            Vector2 buffer = new Vector2(10, 10);
            ///float width = (Settings.xAxisLabels.Count / 2) * Settings.xSize;
            float width = (xAxisLabels.Count + 1) * xSize;
            int sepCount = YmarkerContent.SpawnedGameObjects.Count;
            float height = (yAxisValueSpan * sepCount * ySize)
                            - ((yAxisValueSpan / 4) * ySize)
                            + seperatorThickness;

            content.sizeDelta = new Vector2(width, height) + buffer;
            YmarkerContent.GetComponent<RectTransform>().sizeDelta = new Vector2(YmarkerContent.GetComponent<RectTransform>().sizeDelta.x, content.sizeDelta.y);
        }

        /// <summary>
        /// 現在の最大値を取得する
        /// </summary>
        /// <returns>最大値</returns>
        private float GetMaxY()
        {
            float max = float.MinValue;

            for (int i = 0; i < GraphLines.Count; i++){
                max = Mathf.Max(max, GraphLines[i].GetMaxY());
            }

            return max;
        }
        private float GetSepMaxY(){
            float sepMax = float.MinValue;

            for (int i = 0; i < GraphLines.Count; i++){
                sepMax = Mathf.Max(sepMax, GraphLines[i].GetSepMaxY());
            }

            return sepMax;
        }

        private float GetMinY(){
            float min = float.MaxValue;

            for (int i = 0; i < GraphLines.Count; i++){
                min = Mathf.Min(min, GraphLines[i].GetMinY());
            }

            return min;
        }
        private float GetSepMinY(){
            float sepMin = float.MaxValue;

            for (int i = 0; i < GraphLines.Count; i++){
                sepMin = Mathf.Min(sepMin, GraphLines[i].GetSepMinY());
            }

            return sepMin;
        }



        /// <summary>
        /// グラフ外のラベルと軸セパレータの位置を更新
        /// </summary>
        private void UpdateMakersPosition()
        {
            Vector2 contentPosition = content.anchoredPosition;
            YmarkerContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(YmarkerContent.GetComponent<RectTransform>().anchoredPosition.x, content.anchoredPosition.y + seperatorThickness);
            XmarkerContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(content.anchoredPosition.x,  XmarkerContent.GetComponent<RectTransform>().anchoredPosition.y);

        }

    public void ClearGraph(){
        valueList.Clear();
        RefreshGraphUI();
    } 

        /// <summary>
        /// グラフの表示を更新する
        /// </summary>
        public void RefreshGraphUI()
        {

            // Xセパレータの更新
            CreateXAxisMarkers();
            // Yセパレータの更新
            CreateYAxisMarkers();

            ResizeContentContainer();

            UpdateMakersPosition();

            m_OffsetY = GetSepMinY();

            for(int i = 0; i < GraphLines.Count; i++){
                GraphLines[i].Generate();
            }
            if(GraphLines.Any()){
                ScrollToPoint(GraphLines[0].EndPoint);
            }
        }

        /// <summary>
        /// ある点をグラフの中央になるようにスクロールする
        /// </summary>
        public void ScrollToPoint(GraphPoint point)
        {
            Vector2 viewportSize =
                new Vector2(viewport.rect.width, viewport.rect.height);
            Vector2 contentSize =
                new Vector2(content.rect.width, content.rect.height);

            Vector2 contentPosition = - point.AnchoredPosition + 0.5f * viewportSize;

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
            //YmarkerContent.GetComponent<RectTransform>().anchoredPosition = new Vector2(YmarkerContent.GetComponent<RectTransform>().anchoredPosition.x, content.anchoredPosition.y + Settings.seperatorThickness);
            UpdateMakersPosition();
        }


        #region Markers
        private void CreateXMarker(int index, string labelText){
            var markerName = "XMarker(" + index + ")";
            if(XmarkerContent.SpawnedGameObjects.Any(xmc => xmc.name == markerName)){
                return;
            }

            var marker = XmarkerContent.SpawnAndGetGameObject().GetComponent<XMarker>();
            marker.SetLabelText(labelText);
            XmarkerContent.GetComponent<RectTransform>().sizeDelta = new Vector2((index + 1) * xSize, 0);

            marker.name = markerName;
        }

        public void CreateXAxisMarkers(){
            XmarkerContent.DestroyAllSpawns();
            for (int x = 0; x < xAxisLabels.Count; x++) // += settings.valueSpan)
            {
                CreateXMarker(x, xAxisLabels[x]);
            }
        }
        private void CreateYMarker(float y)
        {
            var markerName = "YMarker(" + y + ")";
            if(YmarkerContent.SpawnedGameObjects.Any(ymc => ymc.name == markerName)){
                return;
            }

            var marker = YmarkerContent.SpawnAndGetGameObject().GetComponent<YMarker>();
            marker.Init(y.ToString(), new Color(0, 0, 0, 0.5f));
            marker.transform.SetAsFirstSibling();
            marker.name = markerName;
            marker.SetLabelText(y.ToString()); 
        }

        /// <summary>
        /// Y軸のセパレータを今のグラフに合わせて表示する
        /// </summary>
        private void CreateYAxisMarkers()
        {
            YmarkerContent.DestroyAllSpawns();

            float sepMaxValue = GetSepMaxY(); //GetMaxY();
            float sepMinValue = GetSepMinY(); //GetMinY();

            //int seperatorCount = Mathf.CeilToInt((sepMaxValue - sepMinValue) / settings.yAxisSeparatorSpan);

            for(float y = sepMinValue; y <= sepMaxValue; y += yAxisValueSpan)
            {
                string markerName = "YMarker(" + y + ")";
                var yMarker = YmarkerContent.transform.Find(markerName);

                // 存在したら追加しない
                if (yMarker == null)
                {
                    CreateYMarker(y);
                }else{
                    yMarker.GetComponent<YMarker>().SetLabelText(y.ToString()); 
                }
            }
        }

        #endregion
    }
}
