using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using Math = System.Math;

namespace CobVisuals {

/// <summary>
/// Shows where the microphone and AudioSource are writing/reading respectively in the audio clip and has adjustments
/// </summary>
/// <remarks>
/// Allows you to debug latency, crackling, and other real time audio stabability problems more easily
/// </remarks>
[CustomEditor(typeof(AudioSourcePlaybackDebugger))]
[CanEditMultipleObjects]
public class AudioSourcePlaybackDebuggerEditor : Editor {
    Label readHeadLabel;
    Label writeHeadLabel;
    VisualElement readHead; //The AudioSource reading position in the audio clip
    VisualElement writeHead; //The microphone writing position in the audio clip
    VisualElement root;

    public static int Clamp(int a, int min, int max) {
        return Math.Max(Math.Min(a, max), min);
    }

    public override VisualElement CreateInspectorGUI()
    {
        root = new VisualElement();
        //Playback
        VisualElement playbackContainer = new VisualElement(){
            style = {
                flexDirection = FlexDirection.Row,
                alignItems = Align.FlexEnd,
                justifyContent = Justify.SpaceBetween,

                backgroundColor = Color.black,
                color = Color.white,
                fontSize = 16,
                height = 50,
                //width = ..., defaults to auto, full width in flexbox
                marginBottom = 10,
                unityFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/VisualsTools/Fonts/RobotoMono/RobotoMono-Regular.ttf")
            }
        };
        readHeadLabel = new Label(){
            text = "???",
            style = {
                color = Color.green
            }
        };
        playbackContainer.Add(readHeadLabel);
        writeHeadLabel = new Label(){
            text = "???",
            style = {
                color = Color.red
            }
        };
        playbackContainer.Add(writeHeadLabel);
        readHead = new VisualElement(){
            style = {
                height = 50,
                width = 1,
                backgroundColor = Color.green,
                position = Position.Absolute,
                left = 0
            }
        };
        playbackContainer.Add(readHead);
        writeHead = new VisualElement(){
            style = {
                height = 50,
                width = 1,
                backgroundColor = Color.red,
                color = Color.red,
                position = Position.Absolute,
                left = 0
            }
        };
        playbackContainer.Add(writeHead);
        root.Add(playbackContainer);

        //AudioSource control
        root.Add(new PropertyField(serializedObject.FindProperty("source")));

        root.Add(new Button(()=>{
            var m = (target as AudioSourcePlaybackDebugger).source;
            var s = m.GetComponent<AudioSource>();
            s.timeSamples = AudioSourcePlaybackDebuggerEditor.Clamp(Microphone.GetPosition(m.deviceName) - 1000,0,s.clip.samples);
        }){
            text = "Calibrate (-1000 samples behind WRITE)"
        });

        EditorApplication.update += Refresh;
        return root;
    }

    public double GetVisualizerWidth() {
        return System.Single.IsNaN(root.resolvedStyle.width) ? 300.0 : (double)root.resolvedStyle.width;
    }

    public void Refresh() {
        AudioSourcePlaybackDebugger editorTarget;
        try {
            editorTarget = target as AudioSourcePlaybackDebugger;
        }
        catch(System.Exception e) {
            Debug.Log("Not ready");
            return;
        }
        if(editorTarget == null) {
            return;
        }
        MicrophoneAudioSource source = editorTarget.source;
        if(source == null) {
            return;
        }
        AudioSource audioSource = source.GetComponent<AudioSource>();
        readHeadLabel.text = $"READ: {audioSource.timeSamples}";
        writeHeadLabel.text = $"WRITE: {Microphone.GetPosition(source.deviceName)}";
        if(audioSource.clip == null) { //.
            return;
        }

        readHead.style.left = (int)((double)audioSource.timeSamples / audioSource.clip.samples * GetVisualizerWidth());
        writeHead.style.left = (int)((double)Microphone.GetPosition(source.deviceName) / audioSource.clip.samples * GetVisualizerWidth());
    }
}

}