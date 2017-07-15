using UnityEngine;
using System.Collections;

public class HadoukenScript : MonoBehaviour {

    public float lifetime = 3f;
    public bool isShakunetsu = false;

    float timer = 0f;
    float facing;
    float shakunetsuTimer;
    string owner;

    Animator animator;
    BoxCollider2D boxCollider2D;
    DamageInfo hadoukenInfo;
    DamageInfo shakunetsuInfo;

	// Use this for initialization
	void Start ()
    {
        SetUpInfo();
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();

        if (0 > GetComponent<Rigidbody2D>().velocity.x)
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > lifetime)
        {
            if (isShakunetsu)
                Explode();
            else
                DestroyThis();
        }
    }

    //Need to refigure this one out
    void OnTriggerEnter2D(Collider2D other)
    {
        if (Tags.X_BOUNDARY == other.tag || (!isShakunetsu && other.tag != owner && other.tag != Tags.StagePieces.BASE && other.tag != Tags.Y_BOUDNARY && other.tag != Tags.FRIENDLY && "Untagged" != other.tag))
        {
            if (other.transform.position.z <= (transform.position.z + hadoukenInfo.range) && other.transform.position.z >= (transform.position.z - hadoukenInfo.range))
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                animator.SetTrigger("Hit");
                boxCollider2D.enabled = false;

                if (Tags.X_BOUNDARY != other.tag)
                    other.gameObject.SendMessage("TakeDamage", hadoukenInfo);
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        shakunetsuTimer += Time.deltaTime;

        if (isShakunetsu && shakunetsuTimer >= .1 && Tags.ENEMY == other.tag) // Do damage after every .1 seconds 
        {
            if (other.transform.position.z <= (transform.position.z + shakunetsuInfo.range) && other.transform.position.z >= (transform.position.z - shakunetsuInfo.range))
            {
                // Maybe chnage this to do damage after a certain amount of time, instead of every frame (like every 6th frame maybe)
                other.gameObject.SendMessage("TakeDamage", shakunetsuInfo);
                shakunetsuTimer = 0;
            }
        }
    }

    public void DestroyThis()
    {
        Destroy(this.gameObject);
    }

    public void Explode()
    {
        animator.SetTrigger("Hit");
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
    }

    public void SetOwner(string owner)
    {
        this.owner = owner;
    }

    public void SetUpInfo()
    {
        if(!isShakunetsu)
            hadoukenInfo = new DamageInfo(1, 1f * transform.localScale.x, 7.5f);
        else
            shakunetsuInfo = new DamageInfo(1, 3f * transform.localScale.x, 2f);
    }

    public void isEX()
    {
        if(!isShakunetsu)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x * 1.5f, GetComponent<Rigidbody2D>().velocity.y);
            //hadoukenInfo.damage *= 2;
        }

        if (isShakunetsu) // Make this a large explosion in front of Ryu instead of what it is now.
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x * 2, GetComponent<Rigidbody2D>().velocity.y);
            //shakunetsuInfo.damage *= 2;
        }
    }
}
