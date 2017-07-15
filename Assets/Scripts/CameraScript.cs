using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{
    public Transform focus = null;

    private float minXPos = 999;
    private float maxXPos = -999;
    private int stageLayer = 1;
    GameObject player;

    private void SetCameraX(float x)
    {
        transform.position = new Vector3(x, stageLayer * 10f, transform.position.z);
    }

    private void GoToFocus()
    {
        float x = Mathf.Lerp(transform.position.x, focus.position.x, 5.0f * Time.deltaTime);

        transform.position = new Vector3(x, stageLayer * 10f, transform.position.z);
    }

    void Update()
    {
        player = GameObject.FindGameObjectWithTag(Tags.PLAYER);

        //Debug.Log(null != focus);

        if (null != focus)
            //SetCameraX(focus.position.x);
            GoToFocus();
        else
            SetCameraX(player.transform.position.x);

        //float playerXPos = player.transform.position.x;

        /*if (playerXPos >= minXPos)
            if (playerXPos <= maxXPos)
                SetCameraX(player.transform.position.x);
            else
                SetCameraX(maxXPos);
        else
            SetCameraX(minXPos);*/
    }

    public float GetMinXPos()
    {
        return minXPos;
    }

    public void SetMinXPos(float minX)
    {
        minXPos = minX;
    }

    public float GetMaxXPos()
    {
        return maxXPos;
    }

    public void SetMaxXPos(float maxX)
    {
        maxXPos = maxX;
    }

    public void SetStageLayer(int layer)
    {
        stageLayer = layer;
    }

    public int GetStageLayer()
    {
        return stageLayer;
    }

    public void SetFocus(Transform item)
    {
        focus = item;

        // Probably should just enable/disable these
        if (item)
        {
            transform.Find("RightXBoundary").gameObject.SetActive(true);
            transform.Find("LeftXBoundary").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("RightXBoundary").gameObject.SetActive(false);
            transform.Find("LeftXBoundary").gameObject.SetActive(false);
        }
    }
}
