using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using Math = System.Math;

namespace CobVisuals {

/// <summary>
/// VisualElement that holds markings for an axis
/// </summary>
/// <remarks>
/// Overwrite GetLabel() to tell it what labels to draw for your markings
/// TODO: Only supports horizontal axis right now
/// TODO: Only supports 5 labels right now
/// </remarks>
public class AxisMarkingsElement : VisualElement {

    public AxisMarkingsElement() : base() {
        Redraw();
    }

    /// <returns>
    /// The string for the label that should be displayed at position `amount`
    /// where `amount` is a [0,1] value
    /// </returns>
    public virtual string GetLabel(double amount){
        return amount.ToString("n2");
    }

    public void Redraw(){
        this.Clear(); //Clear all the child elements
        //Redraw/readd all the client elements
        for(int i=0; i<5; i++) {
            //Width of the entire axis object
            double thisWidth = System.Single.IsNaN(this.resolvedStyle.width) ? 300.0 : (double)this.resolvedStyle.width;

            VisualElement markerContainer = new VisualElement(){
                style = {
                    position = Position.Absolute,
                    alignItems = Align.FlexStart,
                    width = 20,
                    left = (float)(thisWidth / 4 * i - 1.0) //-1.0 to center the 2px marker
                }
            };
            VisualElement marker = new VisualElement(){
                style = {
                    width = 2,
                    height = 7,
                    backgroundColor = this.resolvedStyle.color,
                }
            };
            markerContainer.Add(marker);
            VisualElement label = new Label(){
                text = this.GetLabel((double)i / 4)
            };
            markerContainer.Add(label);
            this.Add(markerContainer);
        }
    }
}

}