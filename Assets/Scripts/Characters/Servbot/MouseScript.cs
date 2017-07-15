using UnityEngine;
using System.Collections;

public class MouseScript : MonoBehaviour {

    const float LIFETIME = 1f;

    float timer = 0;
	
	// Update is called once per frame
	void Update ()
    {
        timer += Time.deltaTime;

        if (timer >= LIFETIME)
            Destroy(gameObject);
	}
}
