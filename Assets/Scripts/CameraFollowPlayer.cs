using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField]
    private Transform playerPos;
    private Vector3 offset = new Vector3(0, 2, -10);

    // Update is called once per frame
    void LateUpdate()
    {
        this.transform.position = playerPos.position + offset;
    }
}
