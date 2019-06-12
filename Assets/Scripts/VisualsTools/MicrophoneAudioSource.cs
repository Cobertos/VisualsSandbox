using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DateTime = System.DateTime;
using TimeSpan = System.TimeSpan;

namespace CobVisuals {

/// <summary>
/// Attaches the microphone to the current audio source
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class MicrophoneAudioSource : MonoBehaviour {
    private string _deviceName;
    public string deviceName {
        get {
            return _deviceName;
        }
        set {
            if(Application.isPlaying) { //If playing, actually open the mic source
                TeardownMicrophoneSource();
                TrySetupMicrophoneSource(value, (success)=>{
                    Debug.Log("Microphone " + (value ?? "Default Device") + " " + (success ? "successfully opened" : "failed to open"));
                    if(success) {
                        _deviceName = value;
                    }
                });
                
            }
            else { //Otherwise, just change the property of the microphoneSource
                _deviceName = value;
            }
        }
    }

    ///<summary> Starts a microphone, no guarentee that it actually starts (see TrySetupMicrophoneSource) </summary>
    private void SetupMicrophoneSource(string deviceName) {
        AudioSource source = GetComponent<AudioSource>();
        source.clip = Microphone.Start(deviceName, true, 10, 44100);
        source.loop = true;
        source.Play();
    }

    private IEnumerator TrySetupMicrophoneSourceCoroutine(string deviceName, System.Action<bool> cb = null) {
        SetupMicrophoneSource(deviceName);
        double startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        while(!(Microphone.GetPosition(deviceName) > 0)) {
            yield return null; //Let the coroutine continue
            double waitingTime = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - startTime;
            if(waitingTime > 1000) { //Waiting more than 1 second
                if(cb != null) {
                    cb(false);
                }
                yield break;
            }
        }
        if(cb != null) {
            cb(true);
        }
    }

    ///<summary> Tries to start a MicrophoneSource but times out after 5 seconds, calling a callback </summary>
    ///<remarks> cb will be called when the microphone is successfully opened or times out after 1 second </remarks>
    public void TrySetupMicrophoneSource(string deviceName, System.Action<bool> cb = null) {
        StartCoroutine(TrySetupMicrophoneSourceCoroutine(deviceName, cb));
    }

    public void TeardownMicrophoneSource() {
        AudioSource source = GetComponent<AudioSource>();
        source.Stop();
        source.clip = null;
        Microphone.End(this.deviceName);
    }

    void Start() {
        this.deviceName = null; //Trigger the microphone to setup for the first time
    }
}

}