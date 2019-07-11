using System.Collections.Generic;
using Math = System.Math;
using DateTime = System.DateTime;
using TimeSpan = System.TimeSpan;
using UnityEngine;

namespace CobVisuals {

/// <summary>
/// BPM source requiring tapping on a key
/// </remarks>
public class TapBPMSource : BPMSource {
    public double bpmAverage;
    public int bpmSamples = 0;
    public double bpmsLast;

    void Update(){
        if(Input.GetKeyDown(KeyCode.B)) {
            double now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            //Set epoch to the last key press time
            beatEpoch = now;

            //Get the first sample or last keypress > 1.5 sec ago so reset
            if(bpmSamples <= 0 || now - bpmLast > 1500) {
                bpmsLast = now;
                bpmSamples = 1;
                return;
            }

            //Get a new sample delta, and add to cummulative average
            double msDelta = now - bpmsLast;
            double bpmFromDelta = (1/msDelta) * (60.0 * 1000.0);
            bpmAverage = (bpmAverage * bpmSamples + bpmFromDelta) / (bpmSamples + 1);
            bpmSamples += 1;
            bpmsLast = now;

            bpm = bpmAverage; //Set the internal bpm using the average calculated
        }
    }
}

}