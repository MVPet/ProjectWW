using UnityEngine;
using System.Collections;

public class CrossSceneDataScript : MonoBehaviour {

    private CharList charList;
    public const float GAME_SPEED = 1.2f;

    public int player; // char value of the player's character
    private bool chunUnlocked = false;

    void Start()
    {
        Screen.SetResolution(960, 600, false, 60); // Force these for now
        DontDestroyOnLoad(this);
        charList = GetComponentInChildren<CharList>();
    }

    public void SetPlayer(int charNum)
    {
        player = charNum;
    }

    public GameObject GetPlayer()
    {
        return charList.GetCharAtIndex(player);
    }

    public bool CheckCharUnlocked(int charIndex)
    {
        switch(charIndex)
        {
            case 0:
                return true;
            case 1:
                return chunUnlocked;
            default:
                return true;
        }
    }

    public int GetTotalPlayableCharacters()
    {
        if (!chunUnlocked)
            return 1;
        else
            return 2;
    }

    public void UnlockChun()
    {
        chunUnlocked = true;
    }

    public float GetGameSpeed()
    {
        return GAME_SPEED;
    }
}
