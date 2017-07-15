using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// REWORK ALL OF OBJECTIVES
public class LevelScript : MonoBehaviour
{
    public Texture2D stageStartTex;
    public Texture2D stageEndTex;
    public Texture2D[] numbers;
    public Texture2D textKO;
    public Texture2D textLoading;

    //string levelString = "p0E0"; // Maybe chnage to lowecase = friendly, uppercase = enemy
    //string levelString = ""; // Maybe change to lowecase = friendly, uppercase = enemy
    //string layer1String = "";
    //string layer2String = "";

    const float BASE_DISTANCE = 9.31f;
    const float BASE_HEIGHT = 10;
    const float BASE_RADIUS = 5.17f;

    public static int enemiesKOd = 0;
    public bool isPaused = false;
    //public List<Objective> curObjectives = new List<Objective>();
    public GameObject levelBase;
    public GameObject levelPath;
    public GameObject levelEmpty;
    public GameObject xBoundary;
    public GameObject missionObject;
    public GameObject mook;
    public GameObject pauseState;

    float stageEndTimer = 9;
    float musicVolumeLevel = .4f;
    //int curObjGuiIndex = 0;
    bool stageStart = false;
    bool stageClear = false;
    bool loading = false;    
    bool pauseHeld = false;
    //int playerSpawnIndex = 0;
    Vector2 playerSpawnIndex;
    //int enemyHQIndex = 0;
    Vector2 enemyHQIndex;
    int missionIndex = 0;
    public float startTimer = 0;

    List<MissionTemplate> missionList;
    List<BaseScript> baseList;
    GameObject[,] stage;
    List<string> transportLayout;

    public GameObject bossEnemy;
    public GameObject player;
    GameObject currentMission;

    float stageWidth;
    //float stageHeight;
    //GameObject player;

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
        float numXPos = 10; // 10

        numXPos = NumbersGUI(numXPos, 68, enemiesKOd, 0.75f); // 88

        GUI.DrawTexture(new Rect(numXPos, 68, textKO.width * 0.75f, textKO.height * 0.75f), textKO, ScaleMode.ScaleToFit);

        if (!stageStart)
            GUI.DrawTexture(new Rect((Screen.width * 0.5f) - (stageStartTex.width * 0.5f), Screen.height * 0.3f, stageStartTex.width, stageStartTex.height), stageStartTex);

        if (stageClear)
        {
            if (loading)
            {
                GUI.DrawTexture(new Rect((Screen.width * 0.5f) - (textLoading.width * 0.5f), (Screen.height * 0.5f) - (textLoading.height * 0.5f), textLoading.width * 0.75f, textLoading.height * 0.75f), textLoading, ScaleMode.ScaleToFit);
                GameObject.Find(Tags.CROSS_SCENE_DATA).GetComponent<CrossSceneDataScript>().UnlockChun();
                SceneManager.LoadScene(0);
            }
            else
                GUI.DrawTexture(new Rect((Screen.width * 0.5f) - (stageEndTex.width * 0.5f), Screen.height * 0.3f, stageEndTex.width, stageEndTex.height), stageEndTex);
        }
    }

    void SetUpPlayer()
    {
        float playerXSpawn = (playerSpawnIndex.x * BASE_DISTANCE) - 0.27f;
        float playerYSpawn = playerSpawnIndex.y - 2.0f;

        GameObject instance;

        if (null == player)
            player = GameObject.Find(Tags.CROSS_SCENE_DATA).GetComponent<CrossSceneDataScript>().GetPlayer();

        instance = Instantiate(player, new Vector3(playerXSpawn, playerYSpawn, 0), Quaternion.identity) as GameObject;
        instance.GetComponent<CharacterBaseScript>().SetSpawnOnGround(new Vector2(playerXSpawn, playerYSpawn));
        instance.tag = Tags.PLAYER;

        //GameObject.FindGameObjectWithTag(Tags.MAIN_CAMERA).GetComponent<CameraScript>().SetPlayer(instance.transform);

        // Might need to set up music the same as the character list
        //GameObject.Find(Tags.MAIN_CAMERA).GetComponent<AudioSource>().clip = instance.GetComponent<CharacterBase>().themeMusic;
    }

    GameObject SpawnRoom(string stagePiece, float xLoc, float yLoc, bool isLeftEdge, bool isRightEdge, bool friendly = false)
    {
        GameObject instance;

        switch (stagePiece)
        {
            case Tags.StagePieces.HQ:
                instance = Instantiate(levelBase, new Vector3(xLoc, yLoc, 1), Quaternion.identity) as GameObject;
                instance.GetComponent<BaseScript>().SetAsHQ(friendly);
                instance.GetComponent<PathScript>().isFriendly = friendly;
                break;
            case Tags.StagePieces.BASE:
                instance = Instantiate(levelBase, new Vector3(xLoc, yLoc, 1), Quaternion.identity) as GameObject;
                baseList.Add(instance.GetComponent<BaseScript>());
                instance.GetComponent<PathScript>().isFriendly = friendly;
                break;
            case Tags.StagePieces.PATH:
                instance = Instantiate(levelPath, new Vector3(xLoc, yLoc, 1), Quaternion.identity) as GameObject;
                instance.GetComponent<PathScript>().isFriendly = friendly;
                break;
            default:
                instance = Instantiate(levelEmpty, new Vector3(xLoc, yLoc, 1), Quaternion.identity) as GameObject;
                break;
        }

        if (isLeftEdge)
        {
            GameObject leftEdge = Instantiate(xBoundary, new Vector3(xLoc - BASE_RADIUS + 0.5f, yLoc, 1), Quaternion.Euler(0, 0, 90)) as GameObject;
            leftEdge.transform.SetParent(instance.transform);
            leftEdge.layer = 8;
        }

        if (isRightEdge)
        {
            GameObject rightEdge = Instantiate(xBoundary, new Vector3(xLoc + BASE_RADIUS - 0.5f, yLoc, 1), Quaternion.Euler(0, 0, 90)) as GameObject;
            rightEdge.transform.SetParent(instance.transform);
            rightEdge.layer = 8;
        }

        return instance;
    }

    void SetCameraMinMax(int max)
    {
        CameraScript camScript = GameObject.FindGameObjectWithTag(Tags.MAIN_CAMERA).GetComponent<CameraScript>();

        camScript.SetMinXPos(0);
        //camScript.SetMaxXPos(((levelString.Length * 0.5f) - 1) * BASE_DISTANCE);
        //camScript.SetMaxXPos(((stageLayerStrings[1].Length * 0.5f) - 1) * BASE_DISTANCE);
        camScript.SetMaxXPos(((max * 0.5f) - 1) * BASE_DISTANCE);
    }

    void GenerateStage(List<string> stageLayout)
    {
        // 9.32 is how far away each should be
        stageWidth = stageLayout[0].Length * BASE_DISTANCE;
        //stageHeight = (stageLayout.Count - 1) * BASE_HEIGHT;

        int roomCount;

        for (int i = 0; i < stageLayout.Count; i++)
        {
            roomCount = 0;
            SpawnRoom(Tags.StagePieces.EMPTY, BASE_DISTANCE * -1, (stageLayout.Count - i - 1) * 10f, false, false);

            for (int j = 0; j < stageLayout[i].Length; j++)
            {
                bool isLeftEdge = (0 == j || '-' == stageLayout[i][j - 1]);
                bool isRightEdge = ((stageLayout[i].Length - 1 == j) || '-' == stageLayout[i][j + 1]);

                switch (stageLayout[i][j])
                {
                    case 'e':
                        stage[i, j] = SpawnRoom(Tags.StagePieces.BASE, BASE_DISTANCE * roomCount++, (stageLayout.Count - i - 1) * 10f, isLeftEdge, isRightEdge, false);
                        break;
                    case 'E':
                        stage[i, j] = SpawnRoom(Tags.StagePieces.HQ, BASE_DISTANCE * roomCount++, (stageLayout.Count - i - 1) * 10f, isLeftEdge, isRightEdge, false);
                        enemyHQIndex.x = j;
                        enemyHQIndex.y = ((stageLayout.Count - i - 1) * 10f);
                        break;
                    case 'f':
                        stage[i, j] = SpawnRoom(Tags.StagePieces.BASE, BASE_DISTANCE * roomCount++, (stageLayout.Count - i - 1) * 10f, isLeftEdge, isRightEdge, true);
                        break;
                    case 'F':
                        stage[i, j] = SpawnRoom(Tags.StagePieces.HQ, BASE_DISTANCE * roomCount++, (stageLayout.Count - i - 1) * 10f, isLeftEdge, isRightEdge, true);
                        playerSpawnIndex.x = j;
                        playerSpawnIndex.y = ((stageLayout.Count - i - 1) * 10f);
                        break;
                    case 'p':
                        stage[i, j] = SpawnRoom(Tags.StagePieces.PATH, BASE_DISTANCE * roomCount++, (stageLayout.Count - i - 1) * 10f, isLeftEdge, isRightEdge);
                        break;
                    default:
                        stage[i, j] = SpawnRoom(Tags.StagePieces.EMPTY, BASE_DISTANCE * roomCount++, (stageLayout.Count - i - 1) * 10f, false, false);
                        break;
                }
            }

            SpawnRoom(Tags.StagePieces.EMPTY, BASE_DISTANCE * roomCount, (stageLayout.Count - i - 1) * 10f, false, false);
        }
    }

    void GenerateStartingEnemies(List<string> enemySpawns)
    {
        for (int i = 0; i < enemySpawns.Count; i++)
        {
            for (int j = 0; j < enemySpawns[i].Length; j++)
            {
                if ('-' != enemySpawns[i][j])
                    stage[i, j].GetComponent<PathScript>().SetEnemiesStart(enemySpawns[i][j] - '0');
            }
        }
    }

    Vector2 FindUpEndPoint(int startVertIndex, int endVertIndex, int horizIndex)
    {
        Vector2 endPoint = new Vector2(0, 0);

        /*for (int x = startVertIndex; x < endVertIndex; x++)
        {
            if (null != stage[x, horizIndex])
            {
                endPoint = new Vector2(-0.27f + (horizIndex * BASE_DISTANCE), -2.0f + (x * 10));
                break;
            }
        }*/

        endPoint = new Vector2(-0.27f + (horizIndex * BASE_DISTANCE), ((startVertIndex - 1) * 10));

        return endPoint;
    }

    Vector2 FindDownEndPoint(int startVertIndex, int endVertIndex, int horizIndex)
    {
        Vector2 endPoint = new Vector2(0, 0);

        /*for (int x = startVertIndex; x > endVertIndex; x--)
        {
            if (null != stage[x, horizIndex])
            {
                endPoint = new Vector2(-0.27f + (horizIndex * BASE_DISTANCE), -2.0f + (x * 10));
                break;
            }
        }*/

        endPoint = new Vector2(-0.27f + (horizIndex * BASE_DISTANCE), ((startVertIndex + 1) * 10));

        return endPoint;
    }

    void GenerateTransportPoints(List<string> transferLayout)
    {
        for (int i = 0; i < transferLayout.Count; i++)
        {
            for (int j = 0; j < transferLayout[i].Length; j++)
            {
                switch (transferLayout[i][j])
                {
                    case 'U':
                        stage[i, j].GetComponent<PathScript>().SpawnUpTransfer(FindDownEndPoint((transferLayout.Count - i - 1), transferLayout.Count, j));
                        break;
                    case 'D':
                        stage[i, j].GetComponent<PathScript>().SpawnDownTransfer(FindUpEndPoint((transferLayout.Count - i - 1), 0, j));
                        break;
                    case 'V':
                        stage[i, j].GetComponent<PathScript>().SpawnUpTransfer(FindDownEndPoint((transferLayout.Count - i - 1), transferLayout.Count, j));
                        stage[i, j].GetComponent<PathScript>().SpawnDownTransfer(FindUpEndPoint((transferLayout.Count - i - 1), 0, j));
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void InitializeLevel()
    {
        StageLayouts stageLayouts = new StageLayouts();

        StageData stageData = stageLayouts.GetRandomStage();

        stage = new GameObject[stageData.height, stageData.width];

        GenerateStage(stageData.stageLayout);
        GenerateStartingEnemies(stageData.enemyLayout);
        GenerateTransportPoints(stageData.transportLayout);
        transportLayout = stageData.transportLayout;

        SetCameraMinMax(stageData.stageLayout[0].Length);
    }

    void CreateNewMission()
    {
        GameObject initialize;
        initialize = Instantiate(missionObject);
        initialize.GetComponent<MissionScript>().SetUpMissionParameters(missionList[missionIndex]);
        currentMission = initialize;
    }

    void SetUpjective()
    {
        MissionTemplate missionTemplate;

        missionIndex = 0;

        
        // First mission
        missionTemplate = new MissionTemplate();
        missionTemplate.type = MissionTemplate.Type.KO;
        missionTemplate.numTarget = 15;
        missionList.Add(missionTemplate);

        // Second mission
        missionTemplate = new MissionTemplate();
        missionTemplate.type = MissionTemplate.Type.Capture;
        missionTemplate.numTarget = 1;
        missionList.Add(missionTemplate);

        // Third mission
        missionTemplate = new MissionTemplate();
        missionTemplate.type = MissionTemplate.Type.Defeat;
        missionTemplate.numTarget = 1;
        missionTemplate.defeatTarget = bossEnemy;
        missionTemplate.targetName = "Ryu";
        missionTemplate.spawnLocation = new Vector2((enemyHQIndex.x * BASE_DISTANCE) - 0.27f, enemyHQIndex.y - 2.0f);
        missionTemplate.layer = (int)(enemyHQIndex.y / 10.0f);
        missionList.Add(missionTemplate);

        CreateNewMission();
    }

    public void MissionComplete(GameObject mission)
    {
        Destroy(mission);
        missionIndex++;

        if (missionList.Count <= missionIndex)
            StageEnd();
        else
            CreateNewMission();
    }

    void Start()
    {
        baseList = new List<BaseScript>();
        missionList = new List<MissionTemplate>();
        //GetLevelLayout();
        //GenerateLeveLStrings();
        InitializeLevel();
        SetUpPlayer();
        enemiesKOd = 0;
        Time.timeScale = 1.3f;
        //Time.timeScale = GameObject.Find(Tags.CROSS_SCENE_DATA).GetComponent<CrossSceneDataScript>().GetGameSpeed();
    }

    void StageEnd()
    {
        stageClear = true;
        GameObject.FindGameObjectWithTag(Tags.PLAYER).SendMessage("DisableInput", stageClear);
        musicVolumeLevel = .1f;
        stageEndTimer = 0;
    }

    void LerpMusicVolume()
    {
        float volume = GameObject.FindGameObjectWithTag(Tags.MAIN_CAMERA).GetComponent<AudioSource>().volume;
        volume = Mathf.Lerp(volume, musicVolumeLevel, 0.01f);
        GameObject.FindGameObjectWithTag(Tags.MAIN_CAMERA).GetComponent<AudioSource>().volume = volume;
    }

    protected void CheckPauseInput()
    {
        float pause = Input.GetAxisRaw("Pause");

        if (!isPaused && 0 >= pause)
            pauseHeld = false;

        if (!pauseHeld && 0 < pause)
        {
            pauseHeld = true;
            Pause();
        }
    }

    void Update()
    {
        LerpMusicVolume();

        CheckPauseInput();

        if (!stageStart && 2 <= startTimer)
        {
            stageStart = true;
            SetUpjective();
        }
        else
            startTimer += Time.deltaTime;

        if (stageClear)
        {
            if (2 <= stageEndTimer)
                loading = true;
            else
                stageEndTimer += Time.deltaTime;
        }
    }

    // Returns xPos as percents 
    public Vector2 GetPlayerLocationInStage()
    {
        float xPos = GameObject.FindGameObjectWithTag(Tags.PLAYER).transform.position.x / stageWidth;
        float yPos = GameObject.FindGameObjectWithTag(Tags.MAIN_CAMERA).GetComponent<CameraScript>().GetStageLayer();
        //float yPos = GameObject.FindGameObjectWithTag(Tags.PLAYER).transform.position.y / stageHeight;

        return new Vector2(xPos, yPos);
    }

    // Returns xPos and yPos as percents 
    public Vector2 GetMisisonTargetLocationsInStage()
    {
        float xPos = 555;
        float yPos = 555;

        if (currentMission && currentMission.GetComponent<MissionScript>().GetTargets())
        {
            xPos = currentMission.GetComponent<MissionScript>().GetTargets().transform.position.x / stageWidth;
            //yPos = currentMission.GetComponent<MissionScript>().GetTargets().transform.position.y / stageHeight;
            yPos = currentMission.GetComponent<MissionScript>().GetTargetLayer();
        }

        return new Vector2(xPos, yPos);
    }

    public void Pause()
    {
        isPaused = true;
        GameObject.FindGameObjectWithTag(Tags.PLAYER).GetComponent<CharacterBaseScript>().isPaused = true;
        GameObject instance = Instantiate(pauseState);
    }

    public void UnPause()
    {
        isPaused = false;
        GameObject.FindGameObjectWithTag(Tags.PLAYER).GetComponent<CharacterBaseScript>().isPaused = false;
    }

    public int GetTotalKOs()
    {
        return enemiesKOd;
    }

    public List<BaseScript> GetBasesList()
    {
        return baseList;
    }

    public GameObject[,] GetStage()
    {
        return stage;
    }

    public List<string> GetTransportLayout()
    {
        return transportLayout;
    }

    public Transform WhatRoomAmIIn(Transform obj)
    {
        int row = (int)Mathf.Floor((obj.position.x + BASE_RADIUS) / BASE_DISTANCE);
        int col = (int)Mathf.Floor((obj.position.y + BASE_RADIUS) / 10.0f); // 2.5f is the height radius, I think

        return stage[stage.GetLength(0) - col, row].transform; // Since the stage is drawn "Upside Down"
    }
}