using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Template = MissionTemplate;

public class MissionScript : MonoBehaviour
{
    public Texture2D missionStartTexture;
    public Texture2D missionCompleteTexture;
    public Texture2D missionBox;
    public Texture2D[] numbers;
    public Texture2D textObjective;
    public Texture2D textKoObjective1;
    public Texture2D textKoObjective2;
    public Texture2D textCaptureObjective1;
    public Texture2D textCaptureObjective2;

    public Template template;

    int doOnce = 0;
    bool targetsSpawned = false;
    string guiString;
    bool completed = false;
    bool slowDown = false;
    float guiTimer = 0;
    float slowDownTimer = 0;

    Dictionary<int, bool> missionBases = new Dictionary<int, bool>();
    GameObject targets; // Will Need to make one to support multiple

    float NumbersGUI(float xPos, float yPos, int numberToDraw, float scale)
    {
        int numTexture;
        int guiNumber = numberToDraw; //Number to draw is most likely passed by value, so this isn't needed
        float numXPos = xPos;
        bool continueDraw = false;

        // Full-Ass this later
        if (guiNumber >= 1000)
        {
            numTexture = guiNumber / 1000;
            GUI.DrawTexture(new Rect(numXPos, yPos, numbers[numTexture].width * scale, numbers[numTexture].height * scale), numbers[numTexture], ScaleMode.ScaleToFit);
            guiNumber = guiNumber % 1000;
            numXPos += 33 * scale;
            continueDraw = true;
        }
        if (guiNumber >= 100 || (continueDraw && guiNumber == 0))
        {
            numTexture = guiNumber / 100;
            GUI.DrawTexture(new Rect(numXPos, yPos, numbers[numTexture].width * scale, numbers[numTexture].height * scale), numbers[numTexture], ScaleMode.ScaleToFit);
            guiNumber = guiNumber % 100;
            numXPos += 33 * scale;
            continueDraw = true;
        }
        if (guiNumber >= 10 || (continueDraw && guiNumber == 0))
        {
            numTexture = guiNumber / 10;
            GUI.DrawTexture(new Rect(numXPos, yPos, numbers[numTexture].width * scale, numbers[numTexture].height * scale), numbers[numTexture], ScaleMode.ScaleToFit);
            guiNumber = guiNumber % 10;
            numXPos += 33 * scale;
            continueDraw = true;
        }
        if (guiNumber > -1)
        {
            numTexture = guiNumber / 1;
            GUI.DrawTexture(new Rect(numXPos, yPos, numbers[numTexture].width * scale, numbers[numTexture].height * scale), numbers[numTexture], ScaleMode.ScaleToFit);
            guiNumber = guiNumber % 1;
            numXPos += 40 * scale;
        }

        return numXPos;
    }

    void OnGUI()
    {
        if (0.5 <= guiTimer)
        {
            if (3 >= guiTimer)
            {
                if (completed)
                    GUI.DrawTexture(new Rect((Screen.width * 0.5f) - (missionCompleteTexture.width * 0.5f), Screen.height * 0.3f, missionCompleteTexture.width, missionCompleteTexture.height), missionCompleteTexture);
                else
                    GUI.DrawTexture(new Rect((Screen.width * 0.5f) - (missionStartTexture.width * 0.5f), Screen.height * 0.3f, missionStartTexture.width, missionStartTexture.height), missionStartTexture);
            }
            else
            {
                if (!completed)
                {
                    GUI.depth = 0;
                    GUI.DrawTexture(new Rect((Screen.width * 0.5f) - 100, -5, 200, 60), missionBox);
                    GUI.DrawTexture(new Rect((Screen.width * 0.5f) - (textObjective.width * 0.5f * 0.5f), 10, textObjective.width * 0.5f, textObjective.height * 0.5f), textObjective, ScaleMode.ScaleToFit);

                    switch(template.type)
                    {
                        case Template.Type.KO:
                            GUI.Label(new Rect(Screen.width * 0.44f, 25, 200, 50), "Defeat " + template.numTarget + " enemies");
                            break;
                        case Template.Type.Capture:
                            GUI.Label(new Rect(Screen.width * 0.45f, 25, 200, 50), "Capture " + template.numTarget + " Base");
                            break;
                        case Template.Type.Defeat:
                            GUI.Label(new Rect(Screen.width * 0.46f, 25, 200, 50), "Defeat " + template.targetName);
                            break;
                    }
                }
                else if (0 == doOnce)
                {
                    doOnce = 1;
                    GameObject.Find(Tags.LEVEL_MANAGER).GetComponent<LevelScript>().MissionComplete(gameObject);
                }
            }
        }
    }

    void StartCompletionProcess()
    {
        slowDown = true;
        Time.timeScale = 0.5f;
        slowDownTimer = 0;
    }

    public void TargetDefeated()
    {
        template.numTarget--;

        if (0 == template.numTarget)
            StartCompletionProcess();
    }

    void CheckKOCompletion()
    {
        int enemiesKOs = GameObject.Find(Tags.LEVEL_MANAGER).GetComponent<LevelScript>().GetTotalKOs();

        if (template.numTarget <= enemiesKOs)
            StartCompletionProcess();
    }

    void CheckCaptureCompletion()
    {
        if (0 != missionBases.Count)
        {
            bool allBasesCaptured = true;

            foreach (KeyValuePair<int, bool> index in missionBases)
            {
                if (!index.Value)
                    allBasesCaptured = false;
            }

            if (allBasesCaptured)
                StartCompletionProcess();
        }
    }

    // Need to set where to spawn target
    void SpawnTargets()
    {
        if (!targetsSpawned)
        {
            targetsSpawned = true;
            GameObject instantiate;

            instantiate = Instantiate(template.defeatTarget, template.spawnLocation, Quaternion.identity) as GameObject;
            //instantiate.GetComponent<AIBase>().SetAsMission(this);
            instantiate.GetComponent<RyuAI>().SetAsMission(this);
            //instantiate.GetComponent<RyuAI>().SetInAir(true, 6.0f); // Fix this once spawns are fixed
            instantiate.SendMessage("SetSpawnOnGround", template.spawnLocation);
            instantiate.transform.GetChild(1).tag = Tags.ENEMY;
            targets = instantiate;
        }
    }

    // Update is called once per frame
    void Update()
    {
        guiTimer += Time.deltaTime;
        slowDownTimer += Time.deltaTime;

        if (slowDown && !completed && 1 <= slowDownTimer)
        {
            completed = true;
            //Time.timeScale = GameObject.Find(Tags.CROSS_SCENE_DATA).GetComponent<CrossSceneDataScript>().GetGameSpeed();
            Time.timeScale = 1.2f;

            guiTimer = 0;
        }

        //if (!completed && Template.Type.KO == template.type)
        if (!slowDown && 3 <= guiTimer)
        {
            switch (template.type)
            {
                case Template.Type.KO:
                    CheckKOCompletion();
                    break;
                case Template.Type.Capture:
                    CheckCaptureCompletion();
                    break;
                case Template.Type.Defeat:
                    if (!targetsSpawned) SpawnTargets();
                    break;
            };
        }
    }

    public void SetUpCaptureMission()
    {
        int randomNumber = 0;
        List<BaseScript> baseList = GameObject.Find(Tags.LEVEL_MANAGER).GetComponent<LevelScript>().GetBasesList();

        if (baseList.Count < template.numTarget)
            template.numTarget = baseList.Count;

        for (int i = 0; i < template.numTarget; i++)
        {
            randomNumber = Random.Range(0, baseList.Count);

            if (!missionBases.ContainsKey(randomNumber))
            {
                baseList[randomNumber].SetAsMissionBase(this, randomNumber);
                missionBases.Add(randomNumber, false);
            }
            else
                i--;
        }
    }

    public void SetUpMissionParameters(Template misisonTemplate)
    {
        template = new Template();
        template.type = misisonTemplate.type;
        template.numTarget = misisonTemplate.numTarget;
        template.defeatTarget = misisonTemplate.defeatTarget;
        template.targetName = misisonTemplate.targetName;
        template.spawnLocation = misisonTemplate.spawnLocation;
        template.layer = misisonTemplate.layer;

        if (Template.Type.Capture == template.type)
            SetUpCaptureMission();
    }

    public void BaseCaptureStatus(int baseNumber, bool status)
    {
        missionBases[baseNumber] = status;
    }

    public GameObject GetTargets()
    {
        if (targets)
            return targets;
        else
            return null;
    }

    public Template.Type GetMissionType()
    {
        return template.type;
    }

    public int GetTargetLayer()
    {
        return template.layer;
    }
}
