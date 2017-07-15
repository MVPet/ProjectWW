using UnityEngine;
using System.Collections;

public class BaseColorScript : MonoBehaviour {

    public float pulsateSpeed;

    bool isFriendly;

    Color baseColor;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Pulsate()
    {
        baseColor.a -= pulsateSpeed;

        if (0 >= baseColor.a || 0.31 <= baseColor.a)
            pulsateSpeed *= -1;
    }

    void Update()
    {
        Pulsate();

        spriteRenderer.color = baseColor;
    }

    public void SetIsFriendly(bool value)
    {
        isFriendly = value;

        if (!isFriendly)
            baseColor = Color.red;
        else
            baseColor = Color.blue;

        baseColor.a = 0.31f;
    }
}
