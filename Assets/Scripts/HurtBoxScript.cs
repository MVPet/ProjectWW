using UnityEngine;
using System.Collections;

public class HurtBoxScript : MonoBehaviour {

    public void TakeDamage(DamageInfo info)
    {
        transform.parent.SendMessage("TakeDamage", info);
    }

    // Leave there here to appease Unity
    public void SetInBase(BaseScript baseScript)
    {
        transform.parent.SendMessage("SetInBase", baseScript);
    }

    // Leave there here to appease Unity
    public void SetNotInBase()
    {
        transform.parent.SendMessage("SetNotInBase");
    }
}
