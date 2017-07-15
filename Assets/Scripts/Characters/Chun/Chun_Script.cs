using UnityEngine;
using System.Collections.Generic;

public class Chun_Script : CharacterBaseScript
{
    public GameObject kikoken;
    public GameObject kikosho;

    void OnGUI()
    {
        BaseGUI();
    }


    // Use this for initialization
    void Start()
    {
        BaseStart("Chun");
    }

    // Update is called once per frame
    void Update()
    {
        BaseUpdate();
    }

    public override void StartDOOODGE()
    {
        base.StartDOOODGE();

        //inAir = true;
        //velocity.y = 3;

        startPosition.y = transform.position.y;
    }

    public void Normal1()
    {
        velocity.x = 1 * transform.localScale.x;
    }

    public void Normal2()
    {
        velocity.x = 1 * transform.localScale.x;
    }

    public void Normal5()
    {
        velocity.x = 3 * transform.localScale.x;
    }

    public void Unique()
    {
        xMove = Input.GetAxisRaw("Horizontal");

        inAir = true;
        velocity.y = 8;
        startPosition.y = transform.position.y;

        velocity.x = 3 * xMove;
    }

    public void Special1()
    {
        Vector3 projectileOffset = new Vector3(transform.position.x + 1.25f * transform.localScale.x, transform.position.y - 0.85f, transform.position.z);
        GameObject instance = Instantiate(kikoken, projectileOffset, Quaternion.identity) as GameObject;
        instance.GetComponent<ProjectileBaseScript>().SetProjectileInfo(projectiles["Kikoken"], this.tag, transform.localScale.x);
    }

    public void Special3()
    {
        horizDampEnable = false;
        velocity.x = 3.0f * transform.localScale.x;
    }

    public void Special4()
    {
        Vector3 projectileOffset = new Vector3(transform.position.x + 0.5f * transform.localScale.x, transform.position.y - 0.2f, transform.position.z);
        GameObject instance;
        instance = Instantiate(kikosho, projectileOffset, Quaternion.identity) as GameObject;
        instance.GetComponent<ProjectileBaseScript>().SetProjectileInfo(projectiles["Kikosho"], this.tag, transform.localScale.x);
    }

    public void Super0()
    {
        velocity.x = 2 * transform.localScale.x;
    }

    public void Super1()
    {
        velocity.x = 1 * transform.localScale.x;
    }
}
