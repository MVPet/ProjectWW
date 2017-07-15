using UnityEngine;
using System.Collections;

public class ServbotScript : CharacterBaseScript
{

    public GameObject table;
    public GameObject mouse;
    public GameObject pPlane;
    public GameObject fire;
    public GameObject servSummon;
    public Texture2D uniqueBarBackground;
    public Texture2D uniqueBarForeground;

    int maxFood = 100;
    public int curFood = 0;

    void OnGUI()
    {
        BaseGUI();
        float percentUnique = uniqueBarForeground.width * ((float)curFood / (float)maxFood);

        GUI.DrawTexture(new Rect(58, 26, uniqueBarBackground.width, uniqueBarBackground.height), uniqueBarBackground, ScaleMode.ScaleToFit);

        // Misaligned for some reason, 
        GUI.DrawTexture(new Rect(61 - (3 * (int)(percentUnique * 0.01f)), 26, percentUnique, uniqueBarForeground.height), uniqueBarForeground, ScaleMode.ScaleAndCrop);
    }

    void Start()
    {
        name = Tags.Characters.SERVBOT;
        //footOffset = -0.15f;
        SetUpAttacks();
        BaseStart("Serv");
    }

    void Update()
    {
        BaseUpdate();
    }

    public void LightAttack1()
    {
        if (!is_dodging)
            velocity.x = 2f * facing.x;
    }

    public void LightAttack5()
    {
        inAir = true;
        velocity.y = 8;
        startPosition.y = transform.position.y;
    }

    public void HeavyAttack1()
    {
        //Spawn ServSummons here
        float summonXOffset = 96f; // The width in pixel of each frame
        Camera cam = GameObject.FindGameObjectWithTag(Tags.MAIN_CAMERA).GetComponent<Camera>();

        for (int i = 0; i < curFood; i++)
        {
            GameObject instance;

            float randY = Random.Range(transform.position.y - 0.5f, transform.position.y + 0.5f);

            Vector3 worldPoint;

            if (1 == transform.localScale.x)
                worldPoint = cam.ScreenToWorldPoint(new Vector3(0 - summonXOffset, 0));
            else
                worldPoint = cam.ScreenToWorldPoint(new Vector3(Screen.width + summonXOffset, 0));

            worldPoint.y = randY;

            instance = Instantiate(servSummon, worldPoint, Quaternion.identity) as GameObject;
            instance.GetComponent<Rigidbody2D>().velocity = new Vector2(5 * transform.localScale.x, 0f);
            instance.GetComponent<ServSummonScript>().SetOwner(tag);
            instance.GetComponent<ServSummonScript>().AddLifetime(i * 0.01f);

            summonXOffset += 4f;
        }

        curFood = 0;
    }


    public void SpawnTable()
    {
        Vector3 tableSpawn = new Vector3(transform.position.x + (0.1f * transform.localScale.x), transform.position.y, transform.position.z);
        GameObject instance = Instantiate(table, tableSpawn, transform.rotation) as GameObject;
        instance.transform.localScale = transform.localScale;
        instance.GetComponent<TableScript>().SetOwner(tag);
    }

    public void SpawnMouse()
    {
        Vector3 mouseSpawn = new Vector3(transform.position.x - (0.25f * transform.localScale.x), transform.position.y, transform.position.z);
        GameObject instance = Instantiate(mouse, mouseSpawn, transform.rotation) as GameObject;
        instance.transform.localScale = transform.localScale;
    }

    public void HeavyAttack3()
    {
        inAir = true;
        velocity.y = 8;
        velocity.x = 5 * transform.localScale.x;
        startPosition.y = transform.position.y;
    }

    public void SpawnPPlane()
    {
        Vector3 pPlaneSpawn = new Vector3(transform.position.x + (0.1f * transform.localScale.x), transform.position.y, transform.position.z);
        GameObject instance = Instantiate(pPlane, pPlaneSpawn, transform.rotation) as GameObject;
        instance.GetComponent<PlaneScript>().SetOwner(tag);
        instance.GetComponent<Rigidbody2D>().velocity = new Vector2(3 * transform.localScale.x, 0);
    }

    public void HeavyAttack4()
    {
        if (!is_dodging)
            velocity.x = 3 * transform.localScale.x;
        horizDampEnable = false;
    }

    public void SpawnFire()
    {
        Vector3 fireSpawn = new Vector3(transform.position.x + (0.2f * transform.localScale.x), transform.position.y + 0.35f, transform.position.z);
        GameObject instance = Instantiate(fire, fireSpawn, transform.rotation) as GameObject;
        spawnedItem = instance;
        instance.transform.localScale = transform.localScale;
        instance.GetComponent<FireScript>().SetOwner(tag);
    }

    // Fill Damage Dictionary here
    protected override void SetUpAttacks()
    {
        DamageInfo info = new DamageInfo();

        //L
        info = new DamageInfo();
        info.damage = 1;
        info.knockback = new Vector2(0, 0);
        attackInfo.Add("L", info);


        //LL
        info = new DamageInfo();
        info.damage = 1;
        info.knockback = new Vector2(0, 0);
        attackInfo.Add("LL", info);

        //LLH
        info = new DamageInfo();
        info.damage = 4;
        info.knockback = new Vector2(5f, 7f);
        attackInfo.Add("LLH", info);

        //LLL
        info = new DamageInfo();
        info.damage = 1;
        //info.knockback = new Vector2(0, 8.5f);
        info.knockback = new Vector2(0, 0f);
        attackInfo.Add("LLL", info);

        //LLLL
        info = new DamageInfo();
        info.damage = 1;
        //info.knockback = new Vector2(0, 10f);
        info.knockback = new Vector2(0, 0f);
        attackInfo.Add("LLLL", info);

        //LLLLL
        info = new DamageInfo();
        info.damage = 4;
        info.knockback = new Vector2(3f, 10f);
        attackInfo.Add("LLLLL", info);

        //S
        info = new DamageInfo();
        info.damage = 2;
        info.range = 10f;
        info.knockback = new Vector2(0f, 3f);
        attackInfo.Add("S", info);

        //SS
        info = new DamageInfo();
        info.damage = 2;
        info.range = 10f;
        info.knockback = new Vector2(0f, 10f);
        attackInfo.Add("SS", info);
    }

    protected override void BuildMeter()
    {
        base.BuildMeter();

        if ('L' == comboString[comboString.Length - 1])
            curFood++;

        if (curFood >= maxFood)
            curFood = maxFood;
    }
}
