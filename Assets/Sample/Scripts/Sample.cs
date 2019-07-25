using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityLineGraph;

public class Sample : MonoBehaviour
{
    LineGraphController lineGraph;
    public Color Graph1Color = Color.green;
    public Color Graph2Color = Color.red;
    public Color Graph3Color = Color.magenta;

    public bool ShowLabels = false;

    private GraphLine m_GraphLine1; 
    private GraphLine m_GraphLine2; 
    private GraphLine m_GraphLine3; 
    public string xAxisLabelFormat = "{0}s";

    void Start()
    {
        lineGraph = GameObject.Find("LineGraph").GetComponent<LineGraphController>();

        m_GraphLine1 = lineGraph.AddGraphLine(Graph1Color);
        m_GraphLine1.ShowLabels = ShowLabels;

        m_GraphLine2 = lineGraph.AddGraphLine(Graph2Color);
        m_GraphLine2.ShowLabels = ShowLabels;

        m_GraphLine3 = lineGraph.AddGraphLine(Graph3Color);
        m_GraphLine3.ShowLabels = ShowLabels;

        for(int i = 0; i < 13; i++){
            lineGraph.xAxisLabels.Add(string.Format(xAxisLabelFormat, i + 1));
        }


        lineGraph.SetXUnitText("Time(s)");
        lineGraph.SetYUnitText("Energy");

        RefreshGraph();
    }

    public void RefreshGraph(){
        
        var valueList = new List<float>()
        {
            55.25f, 60f, 57f, 51f, 70f, 150.5f, 100.75f
            //5.25f, 6f, 7f, 5f, 7f, 15.5f, 10.75f
        };
        m_GraphLine1.ClearData();
        m_GraphLine2.ClearData();
        m_GraphLine3.ClearData();
        for (int i = 0; i < valueList.Count; i++)
        {
            var lbl = string.Format(xAxisLabelFormat, i + 1);
            m_GraphLine1.AddValue(lbl.ToString(), valueList[i]); 

            var g2Val = Random.Range(valueList[i] - 15f, valueList[i] + 15f); 
            m_GraphLine2.AddValue(lbl.ToString(), (float)System.Math.Round(g2Val, 2));
            if(i > 0 && i % 2 == 0){
                var g3Val = Random.Range(valueList[i] - 10f, valueList[i] + 10f); 
                m_GraphLine3.AddValue(lbl.ToString(), (float)System.Math.Round(g3Val, 2));
            }
        }

        lineGraph.RefreshGraphUI();
    }

    public void ClearGraph(){
        lineGraph.ClearGraph();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int value = Random.Range(0, 300);
            lineGraph.GraphLines[0].AddAndGenerateValue(string.Format(xAxisLabelFormat, lineGraph.xAxisLabels.Count + 1), value);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            lineGraph.xPixelsPerUnit = 10;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            lineGraph.yPixelsPerUnit = 1;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            lineGraph.yAxisUnitSpan = 50;
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            lineGraph.yAxisUnitSpan = 5;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Color lineColor = new Color(Random.Range(0f,1f), Random.Range(0f,1f), Random.Range(0f,1f));
            var randomIndex = Random.Range(0,lineGraph.GraphLines.Count);

            lineGraph.GraphLines[randomIndex].SetColors(lineColor);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            var randomThickness = Random.Range(1f,6f);
            var randomIndex = Random.Range(0,lineGraph.GraphLines.Count);

            lineGraph.GraphLines[randomIndex].LineThickness = randomThickness;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            lineGraph.ResetSettings();
        }
        if(Input.GetKeyUp(KeyCode.D)){
            ClearGraph();
        }
        if(Input.GetKeyUp(KeyCode.R)){
            RefreshGraph();
        }

        if(Input.GetKeyUp(KeyCode.L)){
            ShowLabels = !ShowLabels;
            for(int i = 0; i < lineGraph.GraphLines.Count; i++){
                lineGraph.GraphLines[i].ShowLabels = ShowLabels;
            }
        }
    }
}
