using UnityEngine;
using System.Collections;

public class FireScript : MonoBehaviour {

    public GameObject hitSpark;

    DamageInfo damageInfo;
    string owner;

    // Use this for initialization
    void Start ()
    {
        damageInfo = new DamageInfo(4, 0, 1f);
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
