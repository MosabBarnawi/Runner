using System.Collections;
using UnityEngine;
using TMPro;
using System;


public class HUDFPS : MonoBehaviour
{

    // Attach this to a GUIText to make a frames/second indicator.
    //
    // It calculates frames/second over each updateInterval,
    // so the display does not keep changing wildly.
    //
    // It is also fairly accurate at very low FPS counts (<10).
    // We do this not by simply counting frames per interval, but
    // by accumulating FPS for each frame. This way we end up with
    // correct overall FPS even if the interval renders something like
    // 5.5 frames.
    test myTest;

    public float updateInterval = 0.5F;

    private float accum = 0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval
    [SerializeField] private TextMeshProUGUI guiText;
    void Start()
    {
        if (!guiText)
        {
            Debug.Log("UtilityFramesPerSecond needs a GUIText component!");
            enabled = false;
            return;
        }
        timeleft = updateInterval;

        myTest.updateInterval = updateInterval;

        myTest.timeleft = myTest.updateInterval;
    }

    void Update()
    {
        NormalCall();

        //STRUCT_CALL();
    }

    private void STRUCT_CALL()
    {
        myTest.timeleft -= Time.deltaTime;
        myTest.accum += Time.timeScale / Time.deltaTime;
        ++frames;

        // Interval ended - update GUI text and start new interval
        if (myTest.timeleft <= 0.0)
        {
            // display two fractional digits (f2 format)
            float fps = myTest.accum / myTest.frames;
            string format = System.String.Format("{0:F2} FPS", fps);
            guiText.text = format;

            if (fps < 30)
                guiText.color = Color.yellow;
            else
                if (fps < 10)
                guiText.color = Color.red;
            else
                guiText.color = Color.green;
            //	DebugConsole.Log(format,level);
            myTest.timeleft = myTest.updateInterval;
            myTest.accum = 0.0F;
            myTest.frames = 0;
        }
    }

    private void NormalCall()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        // Interval ended - update GUI text and start new interval
        if (timeleft <= 0.0)
        {
            // display two fractional digits (f2 format)
            float fps = accum / frames;
            string format = System.String.Format("{0:F2} FPS", fps);
            guiText.text = format;

            if (fps < 30)
                guiText.color = Color.yellow;
            else
                if (fps < 10)
                guiText.color = Color.red;
            else
                guiText.color = Color.green;
            //	DebugConsole.Log(format,level);
            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;
        }
    }

    public struct test
    {

        public float updateInterval;

        public float accum ; // FPS accumulated over the interval
        public int frames; // Frames drawn over the interval
        public float timeleft; // Left time for current interval
    }
}

