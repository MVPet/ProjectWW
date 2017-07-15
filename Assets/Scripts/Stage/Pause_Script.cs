using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Pause_Script : MonoBehaviour {

    const int MAX_MENU_OPTIONS = 2;

    public Texture2D backgroundTexture;
    public Texture2D roomTexture;
    public Texture2D transportTexture;
    public Texture2D cursorTexture;
    public Texture2D resumeTexture;
    public Texture2D quitTexture;
    public Texture2D hqTexture;
    public Texture2D missionRoomTexture;
    public Texture2D playerTexture;
    public Texture2D missionEnemyTexture;

    bool pauseHeld = true;
    bool horizontalHeld = false;
    //bool verticalHeld = false;
    int menuOption = 0;
    float mapScale = 1.5f;
    float optionXGap;

    GameObject[,] stage;
    char[,] transportMap;

    Vector2 mapEdges;
    float mapWidth;
    //float mapHeight;
    Vector2 optionsAnchor;

    void OnGUI()
    {
        int guiMenuOption = 0;

        GUI.depth = 0;

        // Tinted Background
        GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), backgroundTexture);
        GUI.color = Color.white;

        // Resume
        GUI.DrawTexture(new Rect(optionsAnchor.x + (optionXGap * guiMenuOption++), optionsAnchor.y, resumeTexture.width, resumeTexture.height), resumeTexture, ScaleMode.StretchToFill);

        // Exit
        GUI.DrawTexture(new Rect(optionsAnchor.x + (optionXGap * guiMenuOption++), optionsAnchor.y, quitTexture.width, quitTexture.height), quitTexture, ScaleMode.StretchToFill);

        // Cursor
        GUI.DrawTexture(new Rect(optionsAnchor.x + (optionXGap * menuOption) - (cursorTexture.width * 0.75f), optionsAnchor.y, cursorTexture.width * 0.5f, cursorTexture.height * 0.5f), cursorTexture, ScaleMode.StretchToFill);

        DrawStageMap();
        DrawTransportMap();
        DrawPlayer();
        DrawDefeatTargets();
    }

    void DrawStageMap()
    {
        int row = 0;

        for(int i = 0; i < stage.GetLength(0); i++, row++)
        {
            for(int j = 0; j < stage.GetLength(1);j++)
            {
                if (!stage[i, j].name.Contains("Empty"))
                {
                    BaseScript baseScript = stage[i, j].GetComponent<BaseScript>();

                    if (baseScript)
                    {
                        if (baseScript.isFriendly)
                            GUI.color = Color.blue;
                        else
                            GUI.color = Color.red;
                    }

                    GUI.DrawTexture(new Rect(mapEdges.x + (j * 32.0f * mapScale), mapEdges.y + ((i + row) * 32.0f * mapScale), roomTexture.width * mapScale, roomTexture.height * mapScale), roomTexture);

                    GUI.color = Color.white;

                    if (baseScript)
                    {
                        if (baseScript.isHQ)
                        {
                            GUI.DrawTexture(new Rect(mapEdges.x + (j * 32.0f * mapScale) + (8f * mapScale), mapEdges.y + ((i + row) * 32.0f * mapScale) + (9f * mapScale), hqTexture.width * mapScale, hqTexture.height * mapScale), hqTexture);
                        }

                        if(baseScript.isMission)
                        {
                            GUI.DrawTexture(new Rect(mapEdges.x + (j * 32.0f * mapScale), mapEdges.y + ((i + row) * 32.0f * mapScale), missionRoomTexture.width * mapScale, missionRoomTexture.height * mapScale), missionRoomTexture);
                        }
                    }
                }
            }
        }

        mapWidth = mapEdges.x + (stage.GetLength(1)) * 32.0f * mapScale;
        //mapHeight = mapEdges.y + (stage.GetLength(0) + (stage.GetLength(0) - 1)) * 32.0f * mapScale;
    }

    void DrawTransportMap()
    {
        int row = -1;

        for (int i = 0; i < stage.GetLength(0); i++, row++)
        {
            for (int j = 0; j < stage.GetLength(1); j++)
            {
                if('|' == transportMap[i,j])
                    GUI.DrawTexture(new Rect(mapEdges.x + (j * 32.0f * mapScale), mapEdges.y + ((i + row) * 32.0f * mapScale), transportTexture.width * mapScale, transportTexture.height * mapScale), transportTexture);
            }
        }
    }

    void DrawPlayer()
    {
        Vector2 playerPos = GameObject.Find(Tags.LEVEL_MANAGER).GetComponent<LevelScript>().GetPlayerLocationInStage();

        float xPos = (mapWidth - mapEdges.x) * playerPos.x;
        //float yPos = (mapHeight - mapEdges.y) * (1.0f - playerPos.y);

        //Debug.Log("H: " + mapHeight);
        //Debug.Log("mEy: " + mapEdges.y);
        //Debug.Log(1.0f - playerPos.y);

        //Debug.Log("yP: " + yPos);

        // MIGHT NEED TO FIX LATER
        GUI.DrawTexture(new Rect(mapEdges.x + (xPos) + (8.0f * mapScale), mapEdges.y + ((stage.GetLength(0) + 1.0f) - (playerPos.y * 2.0f)) * 32.0f * mapScale + (8.0f * mapScale), playerTexture.width * mapScale, playerTexture.height * mapScale), playerTexture);
        //GUI.DrawTexture(new Rect(mapEdges.x + (xPos) + (playerTexture.width * 0.5f * mapScale), mapEdges.y + (yPos) - (playerTexture.height * 0.5f * mapScale), playerTexture.width * mapScale, playerTexture.height * mapScale), playerTexture);
    }

    void DrawDefeatTargets()
    {
        Vector2 targetPos = GameObject.Find(Tags.LEVEL_MANAGER).GetComponent<LevelScript>().GetMisisonTargetLocationsInStage();

        if (targetPos.x != 555)
        {
            float xPos = (mapWidth - mapEdges.x) * targetPos.x;
            //float yPos = (mapHeight - mapEdges.y) * (1.0f - targetPos.y);

            // MIGHT NEED TO FIX LATER
            //GUI.DrawTexture(new Rect(mapEdges.x + (xPos) + (missionEnemyTexture.width * 0.5f * mapScale), mapEdges.y + (yPos) - (missionEnemyTexture.height * 0.5f * mapScale), missionEnemyTexture.width * mapScale, missionEnemyTexture.height * mapScale), missionEnemyTexture);
            GUI.DrawTexture(new Rect(mapEdges.x + (xPos) + (missionEnemyTexture.width * 0.5f * mapScale), mapEdges.y + ((stage.GetLength(0) + 1.0f) - (targetPos.y * 2.0f)) * 32.0f * mapScale + (8.0f * mapScale), missionEnemyTexture.width * mapScale, missionEnemyTexture.height * mapScale), missionEnemyTexture);
        }
    }

    // Use this for initialization
    void Start ()
    {
        Time.timeScale = 0;
        stage = GameObject.Find(Tags.LEVEL_MANAGER).GetComponent<LevelScript>().GetStage();
        mapEdges = new Vector2((Screen.width * 0.5f) - (32 * mapScale * (stage.GetLength(1) / 2f)), (Screen.height * 0.4f) - (32 * mapScale * (stage.GetLength(0) / 2f)));
        optionsAnchor = new Vector2(Screen.width * 0.20f, Screen.height * 0.80f);
        optionXGap = Screen.width * 0.45f;
        GenerateTransportMap();
    }

    private void GenerateTransportMap()
    {
        List<string> transportLayout = GameObject.Find(Tags.LEVEL_MANAGER).GetComponent<LevelScript>().GetTransportLayout();

        transportMap = new char[transportLayout.Count, transportLayout[0].Length];
        // "Empty" the map
        for (int i = 0; i < transportLayout.Count; i++)
        {
            for (int j = 0; j < transportLayout[i].Length; j++)
            {
                transportMap[i,j] = '-';
            }
        }

        // Fill the map
        for (int i = 0; i < transportLayout.Count; i++)
        {
            for(int j = 0; j < transportLayout[i].Length; j++)
            {
                switch (transportLayout[i][j])
                {
                    case 'U':
                        for (int x = i; x > 0; x--)
                            if (transportLayout[x][j] != 'U' || transportLayout[x][j] != 'D' || transportLayout[x][j] != 'V')
                                transportMap[x,j] = '|';
                        break;
                    case 'D':
                        for (int x = i; x > transportLayout.Count; x++)
                            if (transportLayout[x][j] != 'U' || transportLayout[x][j] != 'D' || transportLayout[x][j] != 'V')
                                transportMap[x, j] = '|';
                        break;
                    case 'V':
                        for (int x = i; x > 0; x--)
                            if (transportLayout[x][j] != 'U' || transportLayout[x][j] != 'D' || transportLayout[x][j] != 'V')
                                transportMap[x, j] = '|';

                        for (int x = i; x > transportLayout.Count; x++)
                            if (transportLayout[x][j] != 'U' || transportLayout[x][j] != 'D' || transportLayout[x][j] != 'V')
                                transportMap[x, j] = '|';
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void CheckHorizontalInput()
    {
        int horizontal = (int)Input.GetAxisRaw(Tags.Axes.HORIZONTAL);

        if (0 == horizontal)
            horizontalHeld = false;

        if (!horizontalHeld && 0 != horizontal)
        {
            horizontalHeld = true;

            menuOption += horizontal;

            if (0 > menuOption)
                menuOption = MAX_MENU_OPTIONS - 1;
            else if (MAX_MENU_OPTIONS - 1 < menuOption)
                menuOption = 0;
        }
    }

    /*void CheckVerticalInput()
    {
        int vertical = (int)Input.GetAxisRaw(Tags.Axes.VERTICAL);

        if (0 == vertical)
            verticalHeld = false;

        if (!verticalHeld && 0 != vertical)
        {
            verticalHeld = true;

            menuOption += -vertical;

            if (0 > menuOption)
                menuOption = MAX_MENU_OPTIONS - 1;
            else if (MAX_MENU_OPTIONS - 1 < menuOption)
                menuOption = 0;
        }
    }*/

    void CheckSubmitInput()
    {
        int submit = (int)Input.GetAxisRaw(Tags.Axes.SUBMIT);

        if (0 < submit)
        {
            switch (menuOption)
            {
                case 0:
                    UnPause();
                    break;
                case MAX_MENU_OPTIONS - 1:
                    SceneManager.LoadScene(Tags.Scenes.TITLE);
                    break;
            }
        }
    }

    void Update()
    {
        CheckHorizontalInput();
        //CheckVerticalInput();
        CheckSubmitInput();
        CheckPauseInput();
    }

    private void CheckPauseInput()
    {
        float pause = Input.GetAxisRaw("Pause");

        if (0 >= pause)
            pauseHeld = false;

        if (!pauseHeld && 0 < pause)
        {
            pauseHeld = true;
            UnPause();
        }
    }

    private void UnPause()
    {
        //Time.timeScale = GameObject.Find(Tags.CROSS_SCENE_DATA).GetComponent<CrossSceneDataScript>().GetGameSpeed();
        Time.timeScale = 1.2f;
        GameObject.Find(Tags.LEVEL_MANAGER).GetComponent<LevelScript>().UnPause();
        Destroy(gameObject);
    }
}
