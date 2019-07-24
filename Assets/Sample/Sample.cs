using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityLineGraph;

public class Sample : MonoBehaviour
{
    LineGraphController lineGraph;
    List<float> valueList;
    LineGraphController.LineGraphSettings settings;

    void Start()
    {
        lineGraph = GameObject.Find("LineGraph").GetComponent<LineGraphController>();

        settings = LineGraphController.LineGraphSettings.Default;
        //settings.xSize = 95;

        var graphLine = lineGraph.AddGraphLine(Color.green, Color.green);
        var graphLine2 = lineGraph.AddGraphLine(Color.red, Color.red);
        var graphLine3 = lineGraph.AddGraphLine(Color.magenta, Color.magenta);

        for(int i = 0; i < 13; i++){
            settings.xAxisLabels.Add(string.Format("{0}lbl", i + 1));
        }
        lineGraph.ChangeSettings(settings);

        valueList = new List<float>()
        {
            //5.25f, 10f, 7f, 1f, 20f, 100.5f, 50.75f
            55.25f, 60f, 57f, 51f, 70f, 150.5f, 100.75f
        };
        for (int i = 0; i < valueList.Count; i++)
        {
            var lbl = string.Format("{0}lbl", i);
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int value = Random.Range(0, 300);

            valueList.Add(value);
            lineGraph.AddValue(valueList.Count.ToString(), value);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            settings.xSize = 10;
            lineGraph.ChangeSettings(settings);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            settings.ySize = 1;
            lineGraph.ChangeSettings(settings);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            settings.yAxisSeparatorSpan = 50;
            lineGraph.ChangeSettings(settings);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            //settings.valueSpan = 5;
            //lineGraph.ChangeSettings(settings);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Color blue = Color.blue;
            settings.dotColor = blue;
            blue.a = 0.5f;
            settings.connectionColor = blue;
            lineGraph.ChangeSettings(settings);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            settings = LineGraphController.LineGraphSettings.Default;
            lineGraph.ChangeSettings(settings);
        }
        if(Input.GetKeyUp(KeyCode.R)){
            valueList.Clear();
            lineGraph.ClearGraph();
        }
    }
}
