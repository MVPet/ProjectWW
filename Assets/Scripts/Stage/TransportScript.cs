using UnityEngine;
using System.Collections;

public class TransportScript : MonoBehaviour {

    public Vector2 endPoint;

    bool PlayerInRange(float playerZPos)
    {
        if (playerZPos <= (transform.position.y + 0.8f) && playerZPos >= (transform.position.y - 0.8f))
            return true;
        else
            return false;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (Tags.PLAYER == other.tag && PlayerInRange(other.transform.position.z))
            GameObject.FindGameObjectWithTag(Tags.PLAYER).SendMessage("CanTransport", endPoint);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (Tags.PLAYER == other.tag)
            GameObject.FindGameObjectWithTag(Tags.PLAYER).SendMessage("CanTransport", new Vector2(555, 555));
    }

    public void SetTransportPoint(Vector2 endPoint)
    {
        this.endPoint = endPoint;
    }
}
