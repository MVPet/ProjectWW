using UnityEngine;
using System.Collections;

public class BaseScript : PathScript
{
    protected const int SPAWN_ENEMY_THRESHOLD = 10;
    private const int GROUP_SIZE = 25;

    public Texture2D textBase;
    public Texture2D textHQ;
    public Texture2D healthbarFrame;
    public Texture2D healthbarBackground;
    public Texture2D healthbarForeground;

    public GameObject bigMookObject;

    public bool isHQ = false;
    public bool isMission = false;
    bool spawnEnemies = false;
    int numOfEnemiesSpawned = 0;
    int maxHealth = 50;
    int curHealth;
    bool showStatus = false;
    bool leaderSpawned = false;

    BaseColorScript colorScript;
    MissionScript mission;
    int baseNumber;

    void OnGUI()
    {
        if (showStatus)
        {
            float percentHealthLeft = healthbarForeground.width * (((float)maxHealth - (float)curHealth) / (float)maxHealth);
            int friendlyMultiplyer = (isFriendly) ? 0 : 1;

            if (!isHQ)
                GUI.DrawTexture(new Rect((Screen.width * .5f) - (textBase.width * 0.75f * .5f), Screen.height * .15f, textBase.width * 0.75f, textBase.height * 0.75f), textBase, ScaleMode.ScaleToFit); // 10
                                                                                                                                                                                                         //GUI.DrawTexture(new Rect((Screen.width - healthbarBackground.width) - 15, Screen.height * .4f, textBase.width * 0.75f, textBase.height * 0.75f), textBase, ScaleMode.ScaleToFit); // 10
            else
                GUI.DrawTexture(new Rect((Screen.width * .5f) - (textHQ.width * 0.75f * .5f), Screen.height * .15f, textHQ.width * 0.75f, textHQ.height * 0.75f), textHQ, ScaleMode.ScaleToFit); // 10

            GUI.DrawTexture(new Rect((Screen.width * .5f) - (healthbarBackground.width * .5f), Screen.height * .15f + 20, healthbarBackground.width, healthbarBackground.height), healthbarBackground, ScaleMode.ScaleToFit); // 30
            GUI.DrawTexture(new Rect((Screen.width * .5f) - ((healthbarForeground.width * .5f) - percentHealthLeft), Screen.height * .15f + 20, (healthbarForeground.width - percentHealthLeft) * friendlyMultiplyer, healthbarForeground.height), healthbarForeground, ScaleMode.StretchToFill); // 30
            GUI.DrawTexture(new Rect((Screen.width * .5f) - (healthbarFrame.width * .5f), Screen.height * .15f + 20, healthbarFrame.width, healthbarFrame.height), healthbarFrame, ScaleMode.ScaleToFit); // 30
        }
    }

    void Start()
    {
        curHealth = maxHealth;
        minYSpawn = transform.position.y - 2.3f;   // DYBoundary, might want to un-hard code this later
        maxYSpawn = transform.position.y - 1.75f;   // UYBoundary, might want to un-hard code this later
        minXSpawn = transform.position.x - 4.5f;
        maxXSpawn = transform.position.x + 4.5f;

        //mookFootOffset = mook.GetComponent<MookScript>().footOffset;

        colorScript = transform.FindChild("BaseColor").GetComponent<BaseColorScript>();
        colorScript.SetIsFriendly(isFriendly);
    }

    void SpawnGroup(bool onlyLeader = false)
    {
        GameObject instance;
        if (!onlyLeader)
        {
            for (int i = 0; i < GROUP_SIZE; i++)
            {
                instance = base.SpawnEnemy(mook);
                instance.tag = (isFriendly) ? Tags.FRIENDLY : Tags.ENEMY;
                numOfEnemiesSpawned++;
            }
        }

        instance = base.SpawnEnemy(bigMookObject);
        instance.tag = (isFriendly) ? Tags.FRIENDLY : Tags.ENEMY;
        //instance.GetComponent<AI_Base_Script>().SetInAir(true, SPAWN_DISTANCE);
        //if (0 >= curHealth) instance.GetComponent<Big_Mook_Script>().SetAsLeader(this);
        if (0 >= curHealth && !leaderSpawned)
        {
            leaderSpawned = true;
            instance.SendMessage("SetAsLeader", this);
        }
        numOfEnemiesSpawned++;
    }


    void Update()
    {
        if (!isHQ && !isFriendly && spawnEnemies && numOfEnemiesSpawned < SPAWN_ENEMY_THRESHOLD)
            SpawnGroup();
    }

    //void OnTriggerEnter2D(Collider2D other)
    void OnTriggerStay2D(Collider2D other)
    {
        if (!isHQ)
        {
            if (Tags.PLAYER == other.tag)
            {
                showStatus = true;
                if (!isFriendly)
                {
                    spawnEnemies = true;
                    GameObject.FindGameObjectWithTag(Tags.MAIN_CAMERA).GetComponent<CameraScript>().SetFocus(transform);
                }
            }
            else if (Tags.ENEMY == other.tag)
                other.SendMessage("SetInBase", this);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (Tags.PLAYER == other.tag)
        {
            showStatus = false;
            if (!isFriendly)
            {
                spawnEnemies = false;
                GameObject.FindGameObjectWithTag(Tags.MAIN_CAMERA).GetComponent<CameraScript>().SetFocus(null);
            }
        }
        else if (Tags.ENEMY == other.tag)
            other.SendMessage("SetNotInBase");
    }

    public void TakeDamage(string damageTag)
    {
        if ((0 < curHealth) && (!isFriendly && damageTag == Tags.ENEMY) || (isFriendly && damageTag == Tags.FRIENDLY))
        {
            curHealth--;
            numOfEnemiesSpawned--;

            if (0 >= numOfEnemiesSpawned)
                numOfEnemiesSpawned = 0;

            if (0 >= curHealth) SpawnGroup(true);
        }
    }

    public void SetAsMissionBase(MissionScript missionScript, int baseNumber)
    {
        isFriendly = false;
        isMission = true;
        this.baseNumber = baseNumber;
        mission = missionScript;
    }

    public void SetAsHQ(bool friendly)
    {
        isFriendly = friendly;
        isHQ = true;
        maxHealth *= 2;
        curHealth = maxHealth;

        SpriteRenderer background = transform.FindChild("Background").GetComponent<SpriteRenderer>();
        background.color = (isFriendly) ? Color.blue : Color.red;
    }

    public override void SetEnemiesStart(int value)
    {
        base.SetEnemiesStart(value);
    }

    public void LeaderDied()
    {
        isFriendly = !isFriendly;
        colorScript.SetIsFriendly(isFriendly);
        spawnEnemies = false;
        curHealth = 100;

        if (isMission)
            mission.BaseCaptureStatus(baseNumber, isFriendly);

        GameObject.FindGameObjectWithTag(Tags.MAIN_CAMERA).GetComponent<CameraScript>().SetFocus(null);
    }
}
