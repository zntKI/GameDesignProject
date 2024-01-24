using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField]
    private Transform playerPos;
    private Vector3 offset = new Vector3(5, 2, -10);

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        this.transform.position = playerPos.position + offset;
        //this.transform.Translate(playerPos.position.x + offset.x, 0f, playerPos.position.x + offset.z);
    }

    public void ChangeOffset(int sceneId)
    {
        if (sceneId == 1)
        {
            offset = new Vector3(0, 2, -10);
        }
        else if (sceneId == 2)
        {
            this.gameObject.GetComponent<Camera>().orthographicSize *= 1.3f;
            offset = new Vector3(5, 0, -10);
        }
    }
}
