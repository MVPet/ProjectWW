using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{
    const int MAX_MENU_OPTION = 3;
    const int MAX_CHAR_OPTION = 2;

    public int menuOption = 0;
    public int charOption = 0;
    public int controlMenuOption = 0;
    public string version;
    public int charToSelect = 0;

    public Texture2D title;
    public Texture2D pressStart;
    public Texture2D loading;
    public Texture2D cursor;
    public Texture2D charText;
    public Texture2D[] charPortraits;
    public Texture2D charCursor;
    public Texture2D controlsTexture;
    public Texture2D controlsArrowTexture;
    public Texture2D[] controlsTextures;
    public Texture2D[] keyboardTextures;
    public Texture2D[] controllerTextures;

    bool startPressed;
    bool horizontalHeld;
    bool verticalHeld;
    bool controlMenu;

    Vector2 optionsAnchor;
    float optionYGap;

    void OnGUI()
    {
        if (!controlMenu)
        {
            //Draw TitleScreen
            int guiMenuOption = 0;

            // Disclaimer
            GUI.Label(new Rect(10, 10, Screen.width - 10, 30), "DISCLAIMER: THIS IS A NONPROFIT FAN GAME. THIS IS IN NO WAY ENDORESED BY CAPCOM");
            GUI.Label(new Rect(10, 30, Screen.width - 10, 30), "ALL CHARACTERS, GRAPHICS, AND SOUND ARE OWNED BY CAPCOM.");

            // Title
            GUI.DrawTexture(new Rect((Screen.width * 0.5f) - (title.width * 0.5f), (Screen.height * 0.25f) - (title.height * 0.5f), title.width, title.height), title, ScaleMode.StretchToFill);

            // Cursor
            GUI.DrawTexture(new Rect(optionsAnchor.x - cursor.width, optionsAnchor.y + (optionYGap * menuOption), cursor.width * 0.5f, cursor.height * 0.5f), cursor, ScaleMode.StretchToFill);

            // Character Option
            GUI.DrawTexture(new Rect(optionsAnchor.x, optionsAnchor.y + (optionYGap * guiMenuOption++), charText.width, charText.height), charText, ScaleMode.StretchToFill);

            // Character Portraits
            for (int i = 0; i < MAX_CHAR_OPTION; i++)
                if (GameObject.Find(Tags.CROSS_SCENE_DATA).GetComponent<CrossSceneDataScript>().CheckCharUnlocked(i))
                    GUI.DrawTexture(new Rect(optionsAnchor.x + charText.width + (charPortraits[i].width * 0.5f) + (charPortraits[i].width * 2 * i), optionsAnchor.y - (charPortraits[i].width * 0.5f), charCursor.width * 2, charCursor.height * 2), charPortraits[i], ScaleMode.StretchToFill);

            // Character Select Cursor
            GUI.DrawTexture(new Rect(optionsAnchor.x + charText.width + (charPortraits[charOption].width * 0.5f) + (charPortraits[charOption].width * charOption * 2.0f), optionsAnchor.y - (charPortraits[charOption].width * 0.5f), charCursor.width * 2, charCursor.height * 2), charCursor, ScaleMode.StretchToFill);

            // Controls Option
            GUI.DrawTexture(new Rect(optionsAnchor.x, optionsAnchor.y + (optionYGap * guiMenuOption++), controlsTexture.width, controlsTexture.height), controlsTexture, ScaleMode.StretchToFill);

            // Play Option
            GUI.DrawTexture(new Rect(optionsAnchor.x, optionsAnchor.y + (optionYGap * guiMenuOption++), pressStart.width, pressStart.height), pressStart, ScaleMode.StretchToFill);

            // Loading
            if (startPressed)
                GUI.DrawTexture(new Rect(Screen.width * 0.25f, Screen.height - loading.height - 20, loading.width, loading.height), loading, ScaleMode.StretchToFill);

            // Version
            GUI.Label(new Rect(10, Screen.height - 30, Screen.width - 10, 30), version);
        }
        else
        {
            // Draw Control Menu
            GUI.DrawTexture(new Rect(Screen.width * 0.5f - controlsTexture.width * 0.5f, Screen.height * 0.05f, controlsTexture.width, controlsTexture.height), controlsTexture, ScaleMode.StretchToFill);
            if (0 == controlMenuOption)
            {
                GUI.DrawTexture(new Rect(Screen.width * 0.5f - keyboardTextures[0].width * 0.75f * 0.5f, Screen.height * 0.15f, keyboardTextures[0].width * 0.75f, keyboardTextures[0].height * 0.75f), keyboardTextures[0], ScaleMode.StretchToFill);
                GUI.DrawTexture(new Rect(Screen.width * 0.5f - keyboardTextures[0].width * 0.75f * 0.5f - 10.0f, Screen.height * 0.15f + 4.0f, -controlsArrowTexture.width, controlsArrowTexture.height), controlsArrowTexture, ScaleMode.StretchToFill);
                GUI.DrawTexture(new Rect(Screen.width * 0.5f + keyboardTextures[0].width * 0.75f * 0.5f + 10.0f, Screen.height * 0.15f + 4.0f, controlsArrowTexture.width, controlsArrowTexture.height), controlsArrowTexture, ScaleMode.StretchToFill);
            }
            else if (1 == controlMenuOption)
            {
                GUI.DrawTexture(new Rect(Screen.width * 0.5f - controllerTextures[0].width * 0.75f * 0.5f, Screen.height * 0.15f, controllerTextures[0].width * 0.75f, controllerTextures[0].height * 0.75f), controllerTextures[0], ScaleMode.StretchToFill);
                GUI.DrawTexture(new Rect(Screen.width * 0.5f - controllerTextures[0].width * 0.75f * 0.5f - 10.0f, Screen.height * 0.15f + 4.0f, -controlsArrowTexture.width, controlsArrowTexture.height), controlsArrowTexture, ScaleMode.StretchToFill);
                GUI.DrawTexture(new Rect(Screen.width * 0.5f + controllerTextures[0].width * 0.75f * 0.5f + 10.0f, Screen.height * 0.15f + 4.0f, controlsArrowTexture.width, controlsArrowTexture.height), controlsArrowTexture, ScaleMode.StretchToFill);
            }
            ControlsGUI();
        }
    }

    void ControlsGUI()
    {
        for(int i = 1; i < controlsTextures.Length; i++)
        {
            GUI.DrawTexture(new Rect(Screen.width * 0.2f, Screen.height * 0.15f + (optionYGap * 0.75f * i), controlsTextures[i].width * 0.75f, controlsTextures[i].height * 0.75f), controlsTextures[i], ScaleMode.StretchToFill);

            if (0 == controlMenuOption)
                GUI.DrawTexture(new Rect(Screen.width * 0.5f, Screen.height * 0.15f + (optionYGap * 0.75f * i), keyboardTextures[i].width * 0.75f, keyboardTextures[i].height * 0.75f), keyboardTextures[i], ScaleMode.StretchToFill);
            else if (1 == controlMenuOption)
                GUI.DrawTexture(new Rect(Screen.width * 0.5f, Screen.height * 0.15f + (optionYGap * 0.75f * i), controllerTextures[i].width * 0.75f, controllerTextures[i].height * 0.75f), controllerTextures[i], ScaleMode.StretchToFill);
        }
    }

    void Start()
    {
        charToSelect = GameObject.Find(Tags.CROSS_SCENE_DATA).GetComponent<CrossSceneDataScript>().GetTotalPlayableCharacters();
        optionsAnchor = new Vector2(Screen.width * 0.3f, Screen.height * 0.45f);
        optionYGap = 32f + 32f; // 32 is the height of each text Texture
    }

    void CheckHorizontalInput()
    {
        int horizontal = (int)Input.GetAxisRaw(Tags.Axes.HORIZONTAL);

        if (0 == horizontal)
            horizontalHeld = false;

        if (!horizontalHeld && 0 != horizontal)
        {
            horizontalHeld = true;

            if (!controlMenu)
            {
                if (0 == menuOption)
                {
                    charOption += horizontal;

                    if (0 > charOption)
                        charOption = charToSelect - 1;
                    else if (charToSelect - 1 < charOption)
                        charOption = 0;
                }
            }
            else
            {
                controlMenuOption += horizontal;

                if (0 > controlMenuOption)
                    controlMenuOption = 1;
                else if (1 < controlMenuOption)
                    controlMenuOption = 0;
            }
        }
    }

    void CheckVerticalInput()
    {
        int vertical = (int)Input.GetAxisRaw(Tags.Axes.VERTICAL);

        if (0 == vertical)
            verticalHeld = false;

        if (!verticalHeld && 0 != vertical)
        {
            verticalHeld = true;

            menuOption += -vertical;

            if (0 > menuOption)
                menuOption = MAX_MENU_OPTION - 1;
            else if (MAX_MENU_OPTION - 1 < menuOption)
                menuOption = 0;
        }
    }

    void CheckInput()
    {
        int submit = (int)Input.GetAxisRaw(Tags.Axes.SUBMIT);
        int back = (int)Input.GetAxisRaw(Tags.Axes.CANCEL);

        if (0 < submit)
        {
            switch (menuOption)
            {
                case 1:
                    controlMenu = true;
                    break;
                case MAX_MENU_OPTION - 1:
                    startPressed = true;
                    GameObject.Find(Tags.CROSS_SCENE_DATA).GetComponent<CrossSceneDataScript>().SetPlayer(charOption);
                    SceneManager.LoadScene(Tags.Scenes.TEST);
                    break;
                default:
                    break;
            }
        }

        if (0 < back)
            if (controlMenu)
                controlMenu = false;
    }

    void Update()
    {
        CheckHorizontalInput();
        CheckVerticalInput();
        CheckInput();
    }
}
