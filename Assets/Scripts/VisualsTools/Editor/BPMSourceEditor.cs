using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using Math = System.Math;
using DateTime = System.DateTime;
using TimeSpan = System.TimeSpan;

namespace CobVisuals {

/// <summary>
/// Base class and functionality for getting BPM
/// </summary>
[CustomEditor(typeof(BPMSource))]
[CanEditMultipleObjects]
public class BPMSourceEditor : Editor {
    SerializedProperty beatEpoch;
    VisualElement bpmIndicator;
    Label bpmLabel;
    Label beatLabel;

    void OnEnable() {
        beatEpoch = serializedObject.FindProperty("beatEpoch");
        
    }

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement customInspector = new VisualElement();
        //BPM indicator header
        bpmIndicator = new VisualElement(){
            style = {
                flexDirection = FlexDirection.Column,
                alignItems = Align.Stretch,
                justifyContent = Justify.FlexEnd,

                backgroundColor = Color.black,
                color = Color.white,
                fontSize = 16,
                height = 50,
                //width = ..., defaults to auto, full width in flexbox
                marginBottom = 10,
                unityFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/VisualsTools/Fonts/RobotoMono/RobotoMono-Regular.ttf")
            }
        };
        VisualElement bpmIndicatorBottomRow = new VisualElement(){
            style = {
                flexDirection = FlexDirection.Row,
                alignItems = Align.Center,
                justifyContent = Justify.SpaceBetween,
            }
        };
        bpmLabel = new Label(){
            text = "bpm",
        };
        beatLabel = new Label(){
            text = "beat"
        };
        bpmIndicatorBottomRow.Add(bpmLabel);
        bpmIndicatorBottomRow.Add(beatLabel);
        bpmIndicator.Add(bpmIndicatorBottomRow);
        customInspector.Add(bpmIndicator);

        //Epoch control
        VisualElement epochContainer = new VisualElement(){
            style = {
                flexDirection = FlexDirection.Row,
                alignItems = Align.Center,
                justifyContent = Justify.SpaceBetween
            }
        };
        epochContainer.Add(new PropertyField(beatEpoch));
        epochContainer.Add(new Button(SetBeatEpochNow){
            text = "Now"
        });
        customInspector.Add(epochContainer);
        customInspector.Add(new PropertyField(serializedObject.FindProperty("bpm")));
        EditorApplication.update += Refresh;
        return customInspector;
    }

    public void SetBeatEpochNow(){
        beatEpoch.doubleValue = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        serializedObject.ApplyModifiedProperties();
    }

    public void Refresh() {
        BPMSource bpmSource;
        try {
            bpmSource = target as BPMSource;
        }
        catch(System.Exception e) {
            Debug.Log("Not ready");
            return;
        }

        double blinkValue = 1.0 - (bpmSource.beat - Math.Truncate(bpmSource.beat));
        bpmIndicator.style.backgroundColor = new Color(0,0,(float)blinkValue, 1);

        bpmLabel.text = "BPM " + bpmSource.bpm;

        double beat2 = Math.Truncate(bpmSource.beat * 100.0) / 100.0;
        string beatFormatted = beat2.ToString("n2");
        beatLabel.text = "B# " + beatFormatted;
    }
}

}