using UnityEngine;
using System.Collections;

public class VisionScript : MonoBehaviour
{
    /*
    AIBase parentAi;
    Big_Mook_Script parentAi2;
    RyuAI ryuScript;
    */

    // Use this for initialization
    void Start()
    {
        /*
        parentAi = transform.parent.GetComponent<AIBase>();

        if (!parentAi) parentAi2 = transform.parent.GetComponent<Big_Mook_Script>();

        if (!parentAi2) ryuScript = transform.parent.GetComponent<RyuAI>();
        */
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (Tags.PLAYER == col.tag)
        {
            /*
            if (parentAi)
                parentAi.FoundPlayer(true);
            else if (parentAi2)
                parentAi2.FoundPlayer(true);
            else
                ryuScript.FoundPlayer(true);
                */

            transform.parent.SendMessage("FoundPlayer", true);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (Tags.PLAYER == col.tag)
        {
            /*
            if (parentAi)
                parentAi.FoundPlayer(false);
            else if (parentAi2)
                parentAi2.FoundPlayer(false);
            else
                ryuScript.FoundPlayer(true);
                */

            transform.parent.SendMessage("FoundPlayer", false);
        }
    }
}
