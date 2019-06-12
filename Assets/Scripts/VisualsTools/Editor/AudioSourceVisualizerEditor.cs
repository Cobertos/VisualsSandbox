using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using Math = System.Math;

namespace CobVisuals {

/// <summary>
/// Editor class for the AudioSourceVisualizer
/// </summary>
[CustomEditor(typeof(AudioSourceVisualizer))]
[CanEditMultipleObjects]
public class AudioSourceVisualizerEditor : Editor {

    static int height = 50;
    static int samples = 128; //TODO: Configurable, must be power of 2 as well according to docs

    VisualElement[] bins = new VisualElement[AudioSourceVisualizerEditor.samples];

    //void OnEnable() {
    //}

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement customInspector = new VisualElement();
        //Visualizer
        VisualElement visualizerContainer = new VisualElement(){
            style = {
                flexDirection = FlexDirection.Row,
                alignItems = Align.FlexEnd,
                justifyContent = Justify.Center,

                backgroundColor = Color.black,
                color = Color.white,
                fontSize = 16,
                height = AudioSourceVisualizerEditor.height,
                //width = ..., defaults to auto, full width in flexbox
                marginBottom = 10,
                unityFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/VisualsTools/Fonts/RobotoMono/RobotoMono-Regular.ttf")
            }
        };

        int idx = 0;
        foreach(VisualElement bin in bins) {
            bins[idx] = new VisualElement(){
                style = {
                    flexGrow = 1,
                    flexShrink = 1,
                    height = 2,
                    backgroundColor = Color.red//new Color(68.0f,68.0f,136.0f,1.0f)
                }
            };
            visualizerContainer.Add(bins[idx]);
            idx++;
        }
        customInspector.Add(visualizerContainer);

        //AudioSource control
        customInspector.Add(new PropertyField(serializedObject.FindProperty("source")));
        EditorApplication.update += Refresh;
        return customInspector;
    }

    public void Refresh() {
        AudioSourceVisualizer audioSourceVisualizer;
        try {
            audioSourceVisualizer = target as AudioSourceVisualizer;
        }
        catch(System.Exception e) {
            Debug.Log("Not ready");
            return;
        }
        AudioSource audioSource = audioSourceVisualizer.source;

        //First get the RMS of the audio channel as a whole
        /*float[] intensityBins = new float[AudioSourceVisualizerEditor.samples];
        audioSource.GetOutputData(intensityBins, 0);
        float sum = 0;
        foreach(float sample in intensityBins) {
            sum += sample * sample; // sum squared samples
        }
        float rms = Mathf.Sqrt(sum / AudioSourceVisualizerEditor.samples); // rms = square root of average*/


        float[] spectrumBins = new float[AudioSourceVisualizerEditor.samples];
        audioSource.GetSpectrumData(spectrumBins, 0, FFTWindow.Rectangular);

        int idx = 0;
        foreach(VisualElement bin in bins) {
            float db = 20 * Mathf.Log10(spectrumBins[idx] / 1.0e-6f); // calculate dB
            float linear = Mathf.Clamp(db / 120.0f, 0.0f,1.0f);

            bin.style.height = linear * (AudioSourceVisualizerEditor.height-2) + 2;
            bin.style.backgroundColor = new Color(linear,68.0f/255.0f,136.0f/255.0f,1.0f);
            idx++;
        }
    }
}

}