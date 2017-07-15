using UnityEngine;
using System.Collections;

public class MissionTemplate
{
    public enum Type
    {
        None, Capture, Defeat, KO,
    };

    public Type type;
    public int numTarget;
    public GameObject defeatTarget;
    public string targetName;
    public Vector2 spawnLocation;
    public int layer;

    public MissionTemplate()
    {
        type = Type.None;
        numTarget = 0;
    }
}
