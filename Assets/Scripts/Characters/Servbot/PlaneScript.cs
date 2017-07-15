using UnityEngine;
using System.Collections;

public class PlaneScript : MonoBehaviour {

    public GameObject hitSpark;

    float floatLifetime = 1f;
    float timer = 0f;
    float endYPos;
    bool  falling = false;
    string owner;

    Vector2 velocity;
    DamageInfo damageInfo;


    // Use this for initialization
    void Start ()
    {
        if (0 > GetComponent<Rigidbody2D>().velocity.x)
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);

        endYPos = transform.localPosition.y - 0.5f;

        damageInfo = new DamageInfo(4, 2f * transform.localScale.x, 5f);
    }
	
	// Update is called once per frame
	void Update ()
    {
        timer += Time.deltaTime;
        velocity = GetComponent<Rigidbody2D>().velocity;

        if (falling)
        {
            if (velocity.y > -10)
                velocity.y -= 15f * Time.deltaTime;

            if (velocity.y < 0 && transform.position.y > (endYPos - 0.1) && transform.position.y < (endYPos + 0.1))
            {
                transform.position = new Vector3(transform.position.x, endYPos, transform.position.z);
                velocity.y = 0;
                velocity.x = 0;
                Destroy(gameObject);
            }
        }
        else if (timer >= floatLifetime)
        {
            falling = true;
            timer = 0;
            velocity.x /= 2;
        }

        GetComponent<Rigidbody2D>().velocity = velocity;
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != owner && other.tag != Tags.StagePieces.BASE && other.tag != Tags.Y_BOUDNARY && other.tag != Tags.X_BOUNDARY && other.tag != Tags.FRIENDLY && "Untagged" != other.tag)
        {
            if (other.transform.position.z <= (transform.position.z + damageInfo.range) && other.transform.position.z >= (transform.position.z - damageInfo.range))
            {
                other.gameObject.SendMessage("TakeDamage", damageInfo);

                SpawnHitSpark(other);
            }
        }
    }

    public void DestroyThis()
    {
        Destroy(this.gameObject);
    }

    public void SetOwner(string owner)
    {
        this.owner = owner;
    }
}
