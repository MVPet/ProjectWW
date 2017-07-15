using UnityEngine;

public class Ryu : CharacterBaseScript {

    public GameObject hadouken;
    public GameObject shakunetsu;

    bool isFocus = false;

    Vector2 hadoukenOffset = new Vector2(1.3f, 1.05f);

    void OnGUI()
    {
        BaseGUI();
    }

    // Use this for initialization
    void Start ()
    {
        //name = Tags.Characters.RYU;
        //footOffset = -2;
        //SetUpAttacks();
        BaseStart("Ryu");
	}
	
	// Update is called once per frame
	void Update ()
    {
        BaseUpdate();
	}

    public void StartFocus()
    {
        isFocus = true;
    }

    public void Special1()
    {
        isFocus = false;

        //velocity.x = 0.5f * facing.x;

        GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x, velocity.y);
    }


    public void Special2()
    {
        Vector3 projectileOffset = new Vector3 (transform.position.x + hadoukenOffset.x * transform.localScale.x, transform.position.y + hadoukenOffset.y, transform.position.z);
        GameObject instance =  Instantiate(hadouken, projectileOffset, Quaternion.identity) as GameObject;
        instance.GetComponent<ProjectileBaseScript>().SetProjectileInfo(projectiles["Hadouken"], this.tag, transform.localScale.x);
    }

    public void Special3()
    {
        inAir = true;
        velocity.y = 8;
        startPosition.y = transform.position.y;
    }

    public void Special4()
    {
        horizDampEnable = false;
        velocity.x = 2.5f * transform.localScale.x;
    }

    public void Special5()
    {
        Vector3 projectileOffset = new Vector3(transform.position.x + hadoukenOffset.x * transform.localScale.x, transform.position.y + hadoukenOffset.y, transform.position.z);
        GameObject instance = Instantiate(shakunetsu, projectileOffset, Quaternion.identity) as GameObject;
        instance.GetComponent<ProjectileBaseScript>().SetProjectileInfo(projectiles["Shakunetsu"], this.tag, transform.localScale.x);
    }

    public void SuperAttackOnHit()
    {
        inAir = true;
        velocity.y = 10;
        startPosition.y = transform.position.y;
    }
}
