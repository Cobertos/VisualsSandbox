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

    public override VisualElement CreateInspectorGUI() {
        VisualElement customInspector = new VisualElement();
        ToolbarMenu menu = new ToolbarMenu();
        menu.menu.AppendAction("Default Device", OnSourceSelect, (a) => DropdownMenuAction.Status.Normal, null);
        foreach(string deviceName in Microphone.devices) {
            menu.menu.AppendAction(deviceName, OnSourceSelect, (a) => DropdownMenuAction.Status.Normal, null);
        }
        menuLabel = new Label(){
            text = "Default Device"
        };
        menu.Add(menuLabel);
        customInspector.Add(menu);
        return customInspector;
    }

    public void OnSourceSelect(DropdownMenuAction ev) {

        MicrophoneAudioSource microphoneSource;
        try {
            microphoneSource = target as MicrophoneAudioSource;
        }
        catch(System.Exception e) {
            Debug.Log("Not ready");
            return;
        }

        microphoneSource.deviceName = ev.name == "Default Device" ? null : ev.name;
        menuLabel.text = ev.name;
    }
}

}