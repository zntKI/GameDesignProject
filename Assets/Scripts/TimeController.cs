using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    [SerializeField]
    private float slowMotionTimeScale;

    private float startTimeScale;
    private float startFixedDeltaTime;

    private bool isInSlowMotion = false;

    // Start is called before the first frame update
    void Start()
    {
        startTimeScale = Time.timeScale;
        startFixedDeltaTime = Time.fixedDeltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isInSlowMotion = isInSlowMotion ? StopSlowMotion() : StartSlowMotion();
        }
    }

    private bool StartSlowMotion()
    {
        Time.timeScale = slowMotionTimeScale;
        Time.fixedDeltaTime = startFixedDeltaTime * slowMotionTimeScale;

        return true;
    }

    private bool StopSlowMotion() 
    {
        Time.timeScale = startTimeScale;
        Time.fixedDeltaTime = startFixedDeltaTime;

        return false;
    }
}
