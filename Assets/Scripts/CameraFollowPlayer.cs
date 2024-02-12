using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField]
    private Transform playerPos;

    private Vector3 offset = new Vector3(5, 2, -10);
    private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;

    private bool shouldDoAdvanced = false;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (playerPos == null)
        {
            playerPos = GameObject.Find("Player").transform;
        }
        if (!shouldDoAdvanced)
        {
            this.transform.position = playerPos.position + offset;
        }
        else
        {
            Vector3 targetPos = playerPos.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
        }
    }

    public void ChangeOffset(int sceneId)
    {
        if (sceneId == 1)
        {
            //Debug.Log("IN1");
            offset = new Vector3(0, 2, -10);

            shouldDoAdvanced = false;
        }
        else if (sceneId == 2)
        {
            //Debug.Log("IN2");
            this.gameObject.GetComponent<Camera>().orthographicSize *= 1.3f;
            offset = new Vector3(5, 0, -10);

            shouldDoAdvanced = false;
        }
        else
        {
            offset = new Vector3(0, 0, -10);

            shouldDoAdvanced = true;
        }
    }
}
