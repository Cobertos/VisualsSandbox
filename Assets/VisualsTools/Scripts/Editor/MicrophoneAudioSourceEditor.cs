using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace CobVisuals {

/// <summary>
/// Editor for microphone audio source
/// </summary>
[CustomEditor(typeof(MicrophoneAudioSource))]
[CanEditMultipleObjects]
public class MicrophoneAudioSourceEditor : Editor {
    Label menuLabel;
    Label statusLabel;
    Label sampleFrequencyLabel;
    VisualElement root;

    public MicrophoneAudioSource editorTarget {
        get {
            try {
                return target as MicrophoneAudioSource;
            }
            catch(System.Exception e) {
                Debug.Log("Not ready");
                return null;
            }
        }
    }

    public override VisualElement CreateInspectorGUI() {
        root = new VisualElement();
        VisualElement selectorContainer = new VisualElement(){
            style = {
                flexDirection = FlexDirection.Row,
                alignItems = Align.Stretch,
                justifyContent = Justify.FlexStart
            }
        };
        //Status text/character
        statusLabel = new Label(){
            text = "?",
            style = {
                width = 20 //Constant width to stop the selector from moving
            }
        };
        selectorContainer.Add(statusLabel);
        root.Add(selectorContainer);
        //Microphone source dropdown
        ToolbarMenu menu = new ToolbarMenu();
        menu.menu.AppendAction("Off", OnSourceSelect, (a) => DropdownMenuAction.Status.Normal, null);
        menu.menu.AppendAction("Default Device", OnSourceSelect, (a) => DropdownMenuAction.Status.Normal, null);
        foreach(string deviceName in Microphone.devices) {
            menu.menu.AppendAction(deviceName, OnSourceSelect, (a) => DropdownMenuAction.Status.Normal, null);
        }
        menuLabel = new Label(){
            text = "---"
        };
        menu.Add(menuLabel);
        selectorContainer.Add(menu);

        //Sample rate label
        sampleFrequencyLabel = new Label(){
            text = "@ ???Hz"
        };
        root.Add(sampleFrequencyLabel);

        Refresh();
        editorTarget.DeviceStateChanged += Refresh; //Refresh when the device changes
        return root;
    }

    /// <summary> Refresh the UI with the state of the MicrophoneAudioSource </summary>
    public void Refresh() {
        if(editorTarget == null) {
            return;
        }

        menuLabel.text = editorTarget.deviceName ?? "Default Device";
        bool isListening = editorTarget.enabled && Microphone.IsRecording(editorTarget.deviceName);
        statusLabel.text = isListening ? "✓" : "X";
        statusLabel.style.color = isListening ? Color.green : Color.red;
        //Only show the sample freq when listening
        sampleFrequencyLabel.style.display = isListening ? DisplayStyle.Flex : DisplayStyle.None;
        sampleFrequencyLabel.text = $"@ {editorTarget.sampleFrequency}Hz";
    }

    /// <summary> Change the MicrophoneAudioSource, refresh the UI with the new listening state immediately
    /// and also wait for the DeviceStateChanged event </summary>
    public void OnSourceSelect(DropdownMenuAction ev) {
        if(editorTarget == null) {
            return;
        }

        if(ev.name == "Off") {
            editorTarget.enabled = false;
        }
        else {
            editorTarget.deviceName = ev.name == "Default Device" ? null : ev.name;
            editorTarget.enabled = true;
        }
        Refresh();
    }
}

}