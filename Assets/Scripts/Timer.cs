using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {
    private float time = 0f;
    private bool paused = false;
    
    void Update() {
        if (!paused) {
            // counts up until it reaches 0
            time += Time.deltaTime;
        }
    }

    public float getTime() {
        return time;
    }
    
    public void timerReset() {
        paused = true;
        time = 0;
    }

    public void timerResume() {
        paused = false;
    }
    
    public void timerPause() {
        paused = true;
    }
}