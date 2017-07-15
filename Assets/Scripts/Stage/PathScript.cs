using UnityEngine;
using System.Collections;

public class PathScript : MonoBehaviour
{
    const int START_SPAWN_FACTOR = 3;
    protected const float SPAWN_DISTANCE = 6.0f;

    public bool isFriendly = false; // HARD CODED VALUE

    public GameObject mook;
    public GameObject upTransport;
    public GameObject downTransport;

    protected float mookFootOffset;
    protected float minYSpawn;
    protected float maxYSpawn;
    protected float minXSpawn;
    protected float maxXSpawn;

    protected virtual GameObject SpawnEnemy(GameObject enemyObject)
    {
        GameObject instance;
        float xSpawn = Random.Range(minXSpawn, maxXSpawn);
        float ySpawn = Random.Range(minYSpawn, maxYSpawn) - mookFootOffset; // Get rid of this later

        instance = Instantiate(enemyObject, new Vector3(xSpawn, ySpawn + SPAWN_DISTANCE, -1f), Quaternion.identity) as GameObject;
        instance.GetComponent<AI_Base_Script>().SetInAir(true, ySpawn);
        instance.tag = (isFriendly) ? Tags.FRIENDLY : Tags.ENEMY;

        return instance;
    }

    private void SetStartValues()
    {
        minYSpawn = transform.position.y - 2.75f;   // DYBoundary, might want to un-hard code this later
        maxYSpawn = transform.position.y - 2.0f;   // UYBoundary, might want to un-hard code this later

        minXSpawn = transform.position.x - 3.2f;
        maxXSpawn = transform.position.x + 3.2f;

        //mookFootOffset = mook.GetComponent<MookScript>().footOffset;
    }

    public virtual void SetEnemiesStart(int value)
    {
        int amntEnemeies = value * START_SPAWN_FACTOR;

        SetStartValues();

        for (int i = 0; i < amntEnemeies; i++)
            SpawnEnemy(mook);
    }

    public void SpawnUpTransfer(Vector2 endPoint)
    {
        GameObject transport = Instantiate(upTransport, new Vector3(transform.position.x, transform.position.y - 1.37f, transform.position.z - 0.25f), Quaternion.identity) as GameObject;
        transport.GetComponent<TransportScript>().SetTransportPoint(endPoint);

        transport.transform.parent = transform;
    }

    public void SpawnDownTransfer(Vector2 endPoint)
    {
        GameObject transport = Instantiate(downTransport, new Vector3(transform.position.x, transform.position.y - 2.5f, transform.position.z - 0.25f), Quaternion.identity) as GameObject;
        transport.GetComponent<TransportScript>().SetTransportPoint(endPoint);

        transport.transform.parent = transform;
    }
}
