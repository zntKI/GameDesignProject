using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public bool shouldStop = false;

    [SerializeField]
    private float slowMotionTimeScale;

    private float startTimeScale;
    private float startFixedDeltaTime;

    // Start is called before the first frame update
    void Start()
    {
        startTimeScale = Time.timeScale;
        startFixedDeltaTime = Time.fixedDeltaTime;
    }

    void Update()
    {
        if (shouldStop)
        {
            StopSlowMotion();
        }
    }

    public void StartSlowMotion()
    {
        Time.timeScale = slowMotionTimeScale;
        Time.fixedDeltaTime = startFixedDeltaTime * slowMotionTimeScale;
    }

    public void StopSlowMotion()
    {
        //TODO: Make it slowly go back to normal speed

        Time.timeScale += slowMotionTimeScale/* * .5f*/;
        if (Time.timeScale > startTimeScale)
        {
            Time.timeScale = startTimeScale;
        }
        Time.fixedDeltaTime += startFixedDeltaTime * slowMotionTimeScale/* * .5f*/;
        if (Time.fixedDeltaTime > startFixedDeltaTime)
        {
            Time.fixedDeltaTime = startFixedDeltaTime;
        }
        //Time.timeScale = Mathf.Lerp(Time.timeScale, startTimeScale, slowMotionTimeScale);
        //Time.fixedDeltaTime = Mathf.Lerp(Time.fixedDeltaTime, startFixedDeltaTime, startFixedDeltaTime * slowMotionTimeScale);

        //Debug.Log($"TimeScale: {Time.timeScale}; Fixed: {Time.fixedDeltaTime}");

        if (Time.timeScale == startTimeScale && Time.fixedDeltaTime == startFixedDeltaTime)
        {
            shouldStop = false;
        }

        //Time.timeScale = startTimeScale;
        //Time.fixedDeltaTime = startFixedDeltaTime;
    }
}
