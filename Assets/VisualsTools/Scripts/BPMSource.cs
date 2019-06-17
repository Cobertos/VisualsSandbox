using System.Collections.Generic;
using Math = System.Math;
using DateTime = System.DateTime;
using TimeSpan = System.TimeSpan;
using UnityEngine;

namespace CobVisuals {

/// <summary>
/// Base class and functionality for getting BPM.
/// </summary>
/// <remarks>
/// BPM must be manually entered. Derive for dynamic BPM updating, like from MIDI fed BPM or inferred BPM
/// </remarks>
[ExecuteInEditMode] //TODO: Do we still want this?
public class BPMSource : MonoBehaviour {
    //TODO: Getter for bpm

    /// <summary> The start of the song in MS/the time where `.beat` would be 0.0</summary>
    public double beatEpoch;
    /// <summary> Beats per minute </summary>
    public double bpm = 120.0;

    void Awake(){
        //Set the beatEpoch to Now, let the user change later
        beatEpoch = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }


    /// <summary> Beats per millisecond </summary>
    public double bpms {
        get { return this.bpm / (60.0 * 1000.0); }
    }

    /// <summary> Millisecond per beat </summary>
    public double mspb {
        get { return 1.0 / this.bpms; }
    }

    /// <summary> 
    /// The current beat, 0.0 being the start of the song, 1.0 being one beat
    /// into the song
    /// </summary>
    public double beat {
        get {
            double now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond; //Ticks to ms
            double delta = now - this.beatEpoch;
            double beats = delta * this.bpms;
            return beats;
        }
    }

    /// <returns>
    /// The current fractional beat/scales the BPM by 1/frac
    /// </returns>
    /// <remarks>
    /// Example:
    /// frac = 0.25 will get 4*this.beat, so every 16th note from the original BPM
    /// </remarks>
    public double getFractionalBeat(double frac = 1) {
        return this.beat*1.0/frac;
    }

    /// <returns>
    /// Time of the last fractional beat in MS
    /// </returns>
    public double getLastBeatTime(double frac = 1, double offset = 0) {
        return this.beatEpoch + Math.Floor(this.getFractionalBeat(frac)) * frac * this.mspb;
    }

    //TODO
    //getNextBeatTime(frac=1, offset=0) {
    //    return this.getLastBeatTime(frac) + (this.mspb * frac);
    //}

    /// <summary>
    /// Wrap a passed delegate to only run a maximum every fractional beat
    /// </summary>
    /// <remarks>
    /// Call this every Update() and it will always trigger just after the last beat dependant on
    /// Unity's frame rate
    /// </remarks>
    public System.Action onceEvery(double frac, double offset, System.Action func) {
        double lastFuncTime = 0;
        double lbt; //last beat time
        return ()=>{
            lbt = this.getLastBeatTime(frac, offset);
            if(lbt > lastFuncTime) {
                lastFuncTime = lbt;
                func();
            }
        };
    }
}

}