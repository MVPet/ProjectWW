using UnityEngine;
using System.Collections.Generic;

public class ProjectileInfo
{
    public string name = "";
    public float lifetime = 99f;
    public bool singleHit = true;
    public Vector2 velocity;
    public Dictionary<string, DamageInfo> attackInfo;

    public ProjectileInfo()
    {
        attackInfo = new Dictionary<string, DamageInfo>();
    }
}
