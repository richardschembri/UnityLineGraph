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

        lineGraph.ChangeSettings(settings);

        valueList = new List<float>()
        {
            5.25f, 10f, 7f, 1f, 20f, 100.5f
        };

        for (int i = 0; i < valueList.Count; i++)
        {
            lineGraph.AddValue("woh" + (i + 1).ToString(), valueList[i]);
        }

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
            settings.valueSpan = 5;
            lineGraph.ChangeSettings(settings);
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
            lineGraph.ClearGraph();
        }
    }
}
