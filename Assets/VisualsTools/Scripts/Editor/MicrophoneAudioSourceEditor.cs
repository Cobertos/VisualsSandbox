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
        VisualElement customInspector = new VisualElement();
        VisualElement selectorContainer = new VisualElement(){
            style = {
                flexDirection = FlexDirection.Row,
                alignItems = Align.Stretch,
                justifyContent = Justify.FlexStart
            }
        };
        //Status text
        statusLabel = new Label(){
            text = "?",
            style = {
                width = 20 //Constant width to stop the selector from moving
            }
        };
        selectorContainer.Add(statusLabel);
        customInspector.Add(selectorContainer);
        //Dropdown selector
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

        Refresh();
        editorTarget.DeviceStateChanged += Refresh; //Refresh when the device changes
        return customInspector;
    }

    public void Refresh() {
        if(editorTarget == null) {
            return;
        }

        menuLabel.text = editorTarget.deviceName ?? "Default Device";
        statusLabel.text = (editorTarget.enabled && editorTarget.isListening) ? "✓" : "X";
    }

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