using UnityEngine;
using System.Collections;

public class ProjectileBaseScript : MonoBehaviour {

    public ProjectileInfo pInfo;

    float timer = 0f;
    float facing;
    string owner;
    string comboString = "P";

    public GameObject hitSpark;

    Animator animator;
    BoxCollider2D boxCollider2D;

    // Use this for initialization
    void Awake ()
    {
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        GetComponent<Rigidbody2D>().velocity = pInfo.velocity;
    }
	
	// Update is called once per frame
	void Update ()
    {
        timer += Time.deltaTime;

        if (timer > pInfo.lifetime)
        {
            if (pInfo.singleHit)
                DestroyThis();
            else
                Fizzle();

        }
    }

    void SpawnHitSpark(Collider2D col)
    {
        BoxCollider2D pHitBox = GetComponent<BoxCollider2D>();

        Vector3 hitSparkSpawn = pHitBox.bounds.center + col.bounds.center;

        hitSparkSpawn.x *= 0.5f;
        hitSparkSpawn.y *= 0.5f;
        hitSparkSpawn.z *= 0.5f;

        Instantiate(hitSpark, hitSparkSpawn, Quaternion.identity);
    }

    //Need to refigure this one out
    void OnTriggerEnter2D(Collider2D other)
    {
        //if (Tags.X_BOUNDARY == other.tag || (other.tag != owner && other.tag != Tags.StagePieces.BASE && other.tag != Tags.Y_BOUDNARY && other.tag != Tags.FRIENDLY && "Untagged" != other.tag))
        if (Tags.X_BOUNDARY == other.tag || (other.tag != owner && other.tag != Tags.StagePieces.BASE && other.tag != Tags.Y_BOUDNARY && "Untagged" != other.tag))
        {
            if ((this.tag == Tags.ENEMY && (other.tag == Tags.PLAYER || other.tag == Tags.FRIENDLY)) || ((this.tag == Tags.FRIENDLY || this.tag == Tags.PLAYER) && other.tag == Tags.ENEMY))
            {
                DamageInfo dInfo = new DamageInfo(pInfo.attackInfo[comboString]);

                if (other.transform.position.z <= (transform.position.z + dInfo.range) && other.transform.position.z >= (transform.position.z - dInfo.range))
                {
                    if (pInfo.singleHit)
                    {
                        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                        animator.SetTrigger("Hit");
                        boxCollider2D.enabled = false;
                    }

                    if (Tags.X_BOUNDARY != other.tag)
                    {
                        dInfo.knockback.x *= transform.localScale.x;
                        other.gameObject.SendMessage("TakeDamage", dInfo);

                        if (hitSpark)
                            SpawnHitSpark(other);
                    }
                }
            }
        }
    }

    public void Fizzle()
    {
        animator.SetTrigger("Hit");
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
    }

    public void DestroyThis()
    {
        Destroy(this.gameObject);
    }

    public void SetOwner(string owner)
    {
        this.owner = owner;
    }

    public void SetProjectileInfo(ProjectileInfo info, string owner, float scale)
    {
        transform.localScale = new Vector3(scale, transform.localScale.y, transform.localScale.z);
        this.owner = owner;
        this.tag = owner;
        pInfo = info;
        pInfo.velocity.x = Mathf.Abs(pInfo.velocity.x) * scale;

        gameObject.SetActive(true);
    }

    public void AddProjectileHit()
    {
        comboString += "P";
    }

    public void SetInBase(BaseScript baseScript)
    {}

    public void SetNotInBase()
    {}

}
