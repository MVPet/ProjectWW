using UnityEngine;
using System.Collections;

public class Tags : MonoBehaviour {

    // A list of tag strings
    public const string PLAYER      = "Player";
    public const string STAGE       = "Stage";
    public const string CHAR_BOUND  = "CharBound";
    public const string X_BOUNDARY  = "XBoundary";
    public const string Y_BOUDNARY  = "YBoundary";
    public const string ENEMY       = "Enemy";
    public const string MAIN_CAMERA = "MainCamera";
    public const string LEVEL_MANAGER = "LevelManager";
    public const string CROSS_SCENE_DATA = "CrossSceneData";
    public const string FRIENDLY    = "Friendly";

    public struct StagePieces
    {
        public const string EMPTY= "Empty";
        public const string PATH = "Path";
        public const string BASE = "Base";
        public const string HQ = "HQ";
    };

    public struct Characters
    {
        public const string RYU = "Ryu";
        public const string SERVBOT = "Servbot";
        public const string CHUNLI = "Chun-Li";
    };

    public struct Axes
    {
        public const string HORIZONTAL      = "Horizontal";
        public const string VERTICAL        = "Vertical";
        public const string LIGHT_ATTACK    = "LightAttack";
        public const string HEAVY_ATTACK    = "HeavyAttack";
        public const string DODGE           = "Dodge";
        public const string GUARD           = "Guard";
        public const string SUBMIT          = "Submit";
        public const string CANCEL          = "Cancel";
        public const string PAUSE           = "PAUSE";
    };

    public struct Scenes
    {
        public const string TITLE = "TITLE";
        public const string MAIN_MENU = "MainMenu";
        public const string TEST = "Test";
    };

    public struct Managers
    {
        public const string LEVEL_MANAGER = "LevelManager";
    }
}
