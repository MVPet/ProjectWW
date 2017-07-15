using UnityEngine;
using System.Collections;

public class HitSparkScript : MonoBehaviour
{

    float lifetime = 0;
    float lifeTimer = 0;

    // Use this for initialization
    void Start()
    {
        lifetime = GetComponent<ParticleSystem>().duration;
    }

    // Update is called once per frame
    void Update()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer >= lifetime)
            Destroy(gameObject);
    }
}
