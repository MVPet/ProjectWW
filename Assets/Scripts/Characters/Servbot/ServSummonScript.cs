using UnityEngine;
using System.Collections;

public class ServSummonScript : MonoBehaviour
{
    public GameObject hitSpark;

    float lifetime = 3f;
    float timer = 0f;
    string owner;

    DamageInfo damageInfo;

    void Start()
    {
        // Does not spawn Data as there should only be one Data object
        int servNum = (int)Random.Range(0, 7);
        GetComponent<Animator>().SetInteger("ServNum", servNum + 1);

        damageInfo = new DamageInfo(1, 0f, 0f); // Does wayy too much damage, need to make healths and damage into floats
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

    void Update()
    {
        //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y + footOffset); //1.5 for offset
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);

        timer += Time.deltaTime;

        if (timer >= lifetime)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != owner && other.tag != Tags.StagePieces.BASE && other.tag != Tags.Y_BOUDNARY && other.tag != Tags.X_BOUNDARY && other.tag != Tags.FRIENDLY && "ServSummon(Clone)" != other.name && "Untagged" != other.tag)
        {
            if (other.transform.position.z <= (transform.position.z + damageInfo.range) && other.transform.position.z >= (transform.position.z - damageInfo.range))
            {
                other.gameObject.SendMessage("TakeDamage", damageInfo);

                SpawnHitSpark(other);
            }
        }
    }

    public void SetOwner(string owner)
    {
        this.owner = owner;
    }

    public void AddLifetime(float value)
    {
        lifetime += value;
    }
}
