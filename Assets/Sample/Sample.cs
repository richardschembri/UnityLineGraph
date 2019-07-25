using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityLineGraph;

public class Sample : MonoBehaviour
{
    LineGraphController lineGraph;

    void Start()
    {
        lineGraph = GameObject.Find("LineGraph").GetComponent<LineGraphController>();

        var graphLine = lineGraph.AddGraphLine(Color.green, Color.green);
        var graphLine2 = lineGraph.AddGraphLine(Color.red, Color.red);
        var graphLine3 = lineGraph.AddGraphLine(Color.magenta, Color.magenta);

        for(int i = 0; i < 13; i++){
            lineGraph.xAxisLabels.Add(string.Format("{0}lbl", i + 1));
        }

        var valueList = new List<float>()
        {
            //5.25f, 10f, 7f, 1f, 20f, 100.5f, 50.75f
            55.25f, 60f, 57f, 51f, 70f, 150.5f, 100.75f
        };
        for (int i = 0; i < valueList.Count; i++)
        {
            var lbl = string.Format("{0}lbl", i + 1);
            graphLine.AddValue(lbl.ToString(), valueList[i]); 

            var g2Val = Random.Range(valueList[i] - 5f, valueList[i] + 5f); 
            graphLine2.AddValue(lbl.ToString(), (float)System.Math.Round(g2Val, 2));
            if(i > 0 && i % 2 == 0){
                var g3Val = Random.Range(valueList[i] - 10f, valueList[i] + 10f); 
                graphLine3.AddValue(lbl.ToString(), (float)System.Math.Round(g3Val, 2));
            }
        }

        lineGraph.RefreshGraphUI();

        lineGraph.SetXUnitText("時間(s)");
        lineGraph.SetYUnitText("個体数");
    }

    public void RefreshGraph(){
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int value = Random.Range(0, 300);
            lineGraph.GraphLines[0].AddAndGenerateValue(string.Format("{0}lbl", lineGraph.xAxisLabels.Count + 1), value);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            lineGraph.xSize = 10;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            lineGraph.ySize = 1;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            lineGraph.yAxisValueSpan = 50;
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            lineGraph.yAxisValueSpan = 5;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Color blue = Color.blue;
            blue.a = 0.5f;
            lineGraph.GraphLines[0].SetColors(blue);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            lineGraph.ResetSettings();
        }
        if(Input.GetKeyUp(KeyCode.R)){
            lineGraph.ClearGraph();
        }
    }
}
