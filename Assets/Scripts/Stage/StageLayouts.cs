using UnityEngine;
using System.Collections.Generic;

public class StageData
{
    public List<string> stageLayout;
    public List<string> enemyLayout;
    public List<string> transportLayout;
    public int width;
    public int height;

    public StageData()
    {
        stageLayout = new List<string>();
        enemyLayout = new List<string>();
        transportLayout = new List<string>();
    }
}

public class StageLayouts
{
    List<StageData> stages;

    public StageLayouts()
    {
        StageData tempStage;
        stages = new List<StageData>();
        string stageLayer;

        ////////////////////////////////////////////////////////////////////////////// Code assumes stages are updside down right now
        // Stage Layouts
        // Stage 0
        tempStage = new StageData();
        stageLayer = "";

        // Stage Strings
        stageLayer = "--ppepp--";
        tempStage.stageLayout.Add(stageLayer);
        stageLayer = "Fpp-p-ppE";
        tempStage.stageLayout.Add(stageLayer);
        stageLayer = "--ppppp--";
        tempStage.stageLayout.Add(stageLayer);

        // Enemy Spawn Strings
        stageLayer = "--11011--";
        tempStage.enemyLayout.Add(stageLayer);
        stageLayer = "011-3-110";
        tempStage.enemyLayout.Add(stageLayer);
        stageLayer = "--11111--";
        tempStage.enemyLayout.Add(stageLayer);

        // Transition Strings
        stageLayer = "--D-D-D--";
        tempStage.transportLayout.Add(stageLayer);
        stageLayer = "--V-U-V--";
        tempStage.transportLayout.Add(stageLayer);
        stageLayer = "--U---U--";
        tempStage.transportLayout.Add(stageLayer);

        tempStage.width = tempStage.stageLayout[0].Length;
        tempStage.height = tempStage.stageLayout.Count;
        stages.Add(tempStage);
    }

    public StageData GetRandomStage()
    {
        int rand = Random.Range(0, stages.Count);

        return stages[rand];
    }
}
