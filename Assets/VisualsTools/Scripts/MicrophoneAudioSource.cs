using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DateTime = System.DateTime;
using TimeSpan = System.TimeSpan;

namespace CobVisuals {

/// <summary>
/// Attaches a Microphone to the AudioSource on this GameObject
/// </summary>
/// <remarks>
/// Enabling and disabling will teardown and setup any active microphone
/// Unity is really weird with Microphone input so you might experience some weird garbage.
/// - Latency seems to directly correlate to the length of the Microphone.Start() clip. Because it's an
/// int, the lowest you can go is 1, and it will be 1 second of latency... :/
/// - Garbage crackling and shit... Make sure that your sound is first of all hooked up to a Mixer. Second
/// if it's still crackling, edit the mixer in edit mode and change the pitch. It should fix it for the rest of the time
/// that unity is open
/// </remarks>
[RequireComponent(typeof(AudioSource))]
public class MicrophoneAudioSource : MonoBehaviour {
    [SerializeField]
    private string _deviceName;
    public string deviceName {
        get {
            return _deviceName;
        }
        set {
            if(Application.isPlaying && this.enabled) { //If playing, actually open the mic source
                TrySetupMicrophoneSource(value, (success)=>{
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
    public int sampleFrequency {
        get; protected set;
    }

    public event System.Action DeviceStateChanged;

    ///<summary> Starts a microphone</summary>
    ///<remarks> If Microphone.Start fails or hangs, you're SOL (see TrySetupMicrophoneSource).
    /// Caller is responsible for source.Play(); after setup and proper while(!(Microphone.GetPosition(deviceName) > 0)) {}
    ///</remarks>
    private void SetupMicrophoneSource(string deviceName) {
        AudioSource source = GetComponent<AudioSource>();
        int minFreq, maxFreq;
        Microphone.GetDeviceCaps(deviceName, out minFreq, out maxFreq);
        sampleFrequency = maxFreq == 0 ? 44100 : maxFreq;
        Debug.Log("Starting Microphone " + (deviceName ?? "Default Device") + " @ " + sampleFrequency + "Hz.");
        source.clip = Microphone.Start(deviceName, true, 1, sampleFrequency);
        source.loop = true;
    }

    ///<summary>Coroutine that supports TrySetupMicrophoneSource, can't yield in a lambda :( </summary>
    private IEnumerator TrySetupMicrophoneSourceCoroutine(string deviceName, System.Action<bool> cb) {
        SetupMicrophoneSource(deviceName);
        double startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        while(!(Microphone.GetPosition(deviceName) > 0)) {
            double waitingTime = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - startTime;
            if(waitingTime > 1000) { //Waiting more than 1 second
                cb(false);
                DeviceStateChanged();
                yield break;
            }
            yield return null; //Let the coroutine continue
        }
        GetComponent<AudioSource>().Play();
        cb(true);
        DeviceStateChanged();
    }

    ///<summary> Tries to start a MicrophoneSource but times out after 5 seconds, calling a callback </summary>
    ///<remarks> cb will be called when the microphone is successfully opened or times out after 1 second </remarks>
    public void TrySetupMicrophoneSource(string deviceName, System.Action<bool> cb = null) {
        TeardownMicrophoneSource();
        StartCoroutine(TrySetupMicrophoneSourceCoroutine(deviceName, (success)=>{
            Debug.Log($"Microphone {this._deviceName ?? "Default Device"} {(success ? "successfully listening" : "failed to listen")}");
            if(cb != null) {
                cb(success);
            }
        }));
    }

    ///<summary>Detaches the microphone from the current audio source</summary>
    public void TeardownMicrophoneSource() {
        if(!Microphone.IsRecording(this._deviceName)) {
            return;
        }
        AudioSource source = GetComponent<AudioSource>();
        source.Stop();
        source.clip = null;
        Microphone.End(this.deviceName);
        DeviceStateChanged();
    }

    public void OnEnable() {
        TrySetupMicrophoneSource(this._deviceName);
    }
    public void OnDisable() {
        TeardownMicrophoneSource();
    }
}

}