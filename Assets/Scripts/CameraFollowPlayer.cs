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
    }

    public void ChangeOffset(int sceneId)
    {
        if (sceneId == 1)
        {
            offset = new Vector3(5, 2, -10);
        }
    }
}
