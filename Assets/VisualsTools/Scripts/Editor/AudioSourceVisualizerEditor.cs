using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using Math = System.Math;

namespace CobVisuals {

public class FrequencyAxisMarkingElement : AxisMarkingsElement {
    private bool _useLogScale = true;
    public bool useLogScale {
        get { return _useLogScale; }
        set { if(_useLogScale != value) { _useLogScale = value; Redraw(); } }
    }
    private int _frequencyMin = 10;
    public int frequencyMin {
        get { return _frequencyMin; }
        set { if(_frequencyMin != value) { _frequencyMin = value; Redraw(); } }
    }
    private int _frequencyMax = 40000;
    public int frequencyMax {
        get { return _frequencyMax; }
        set { if(_frequencyMax != value) { _frequencyMax = value; Redraw(); } }
    }

    public static string formatFrequency(double num) {
        if(num >= 1000.0) {
            return $"{(num / 1000.0).ToString("n0")}k";
        }
        return num.ToString("n1");
    }

    public override string GetLabel(double amount) {
        if(useLogScale) {
            double logRange = Math.Log10(frequencyMax) - Math.Log10(frequencyMin); //TODO: Could simplify, log laws
            return formatFrequency(Math.Pow(10, amount * logRange + Math.Log10(frequencyMin)));
        }
        else {
            int range = frequencyMax - frequencyMin;
            return formatFrequency(range * amount + frequencyMin);
        }
    }
}

/// <summary>
/// Editor class for the AudioSourceVisualizer
/// </summary>
[CustomEditor(typeof(AudioSourceVisualizer))]
[CanEditMultipleObjects]
public class AudioSourceVisualizerEditor : Editor {

    static int height = 50;
    static int samples = 8192; //TODO: Configurable, must be power of 2 as well according to docs, 2 - 8192
    static int outputSamples = 128; //The actual output frequency bars, on a log scale
    static int frequencyMin = 10; //Minimum frequency for the visualizer

    VisualElement root;
    VisualElement[] bins = new VisualElement[AudioSourceVisualizerEditor.outputSamples];
    FrequencyAxisMarkingElement xAxis;
    double lastWidth;

    public double GetVisualizerWidth() {
        return System.Single.IsNaN(root.resolvedStyle.width) ? 300.0 : (double)root.resolvedStyle.width;
    }

    public override VisualElement CreateInspectorGUI()
    {
        root = new VisualElement();
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
        root.Add(visualizerContainer);

        //X axis
        xAxis = new FrequencyAxisMarkingElement() {
            style = {
                height = 20
            }
        };
        root.Add(xAxis);

        //AudioSource control
        root.Add(new PropertyField(serializedObject.FindProperty("source")));
        root.Add(new PropertyField(serializedObject.FindProperty("useLogScale")));
        EditorApplication.update += Refresh;
        return root;
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
        AudioClip clip = audioSource.clip;
        if(clip == null) {
            return; //Not ready
        }

        //First get the RMS of the audio channel as a whole
        /*float[] intensityBins = new float[AudioSourceVisualizerEditor.samples];
        audioSource.GetOutputData(intensityBins, 0);
        float sum = 0;
        foreach(float sample in intensityBins) {
            sum += sample * sample; // sum squared samples
        }
        float rms = Mathf.Sqrt(sum / AudioSourceVisualizerEditor.samples); // rms = square root of average*/


        float[] spectrumBins = new float[samples];
        audioSource.GetSpectrumData(spectrumBins, 0, FFTWindow.Hanning);

        System.Func<double, float> frequencyIntensity = (double freq)=>{
            if(freq > clip.frequency || freq < 0) {
                return 0; //No data for this frequency
            }

            //spectrumBins is in the range 0 - clip.frequency
            int binIdx = (int)Math.Truncate(freq / clip.frequency * samples);
            return spectrumBins[binIdx];
        };

        int idx = 0;
        foreach(VisualElement bin in bins) {
            double binRange = (double)idx / bins.Length;
            double sampleFreq;
            if(audioSourceVisualizer.useLogScale) {
                double fMin10 = Math.Log10(frequencyMin);
                double fMax10 = Math.Log10(clip.frequency);
                //Map from linear space with bins.Length to log space with samples as length
                sampleFreq = Math.Pow(10.0,binRange*(fMax10 - fMin10) + fMin10);
            }
            else {
                //Linear from frequencyMin to frequencyMax
                sampleFreq = binRange * (clip.frequency - 
                    frequencyMin) + frequencyMin;
            }

            float intensity = frequencyIntensity(sampleFreq);
            float db = 20 * Mathf.Log10(intensity / 1.0e-6f); // calculate dB from linear getSpectrumData
            float linear = Mathf.Clamp(db / 80.0f, 0.0f,1.0f);

            bin.style.height = linear * (AudioSourceVisualizerEditor.height-2) + 2;
            bin.style.backgroundColor = new Color(linear,68.0f/255.0f,136.0f/255.0f,1.0f);

            idx++;
        }

        //Update xAxis
        xAxis.useLogScale = audioSourceVisualizer.useLogScale;
        xAxis.frequencyMin = frequencyMin;
        xAxis.frequencyMax = clip.frequency;
        if(GetVisualizerWidth() != lastWidth) {
            xAxis.Redraw(); //Let it know it needs to redraw bc the width changed
        }
        lastWidth = GetVisualizerWidth();
    }
}

}