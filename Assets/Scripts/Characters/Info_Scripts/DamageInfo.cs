using UnityEngine;
using System.Collections;

public class DamageInfo {

    public string combo;
    public float damage; // Need to change this into a float thanks to servSummon
    public Vector2 knockback;
    //public float hitStun;
    public float range; // 0.3 of 64 pixels "wide"

    public DamageInfo(float damage = 0, float xKnockback = 0, float yKnockback = 0, float hitStun = 0, float range = 0.3f)
    {
        this.damage = damage;
        knockback = new Vector2(xKnockback, yKnockback);
        this.range = range;
    }

    public DamageInfo(DamageInfo info)
    {
        combo = info.combo;
        damage = info.damage;
        knockback = info.knockback;
        range = info.range;
    }
}
