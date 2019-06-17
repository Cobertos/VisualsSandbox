using System.Collections.Generic;
using UnityEngine;

namespace CobVisuals {

/// <summary> 
/// Sequences multiple delegates along with a beat
/// </summary>
//TODO: This class
//TODO: Use TapTool and an array of functions
//to put together a sequence of code that will get
//run based on a TapTool in sequence instead of having to hand
//code that shit
/*public class Sequencer {
    public BPMSource bpmSource;
    public double frac;
    public List<OnceEveryFunc> sequence;
    public SequenceSource(BPMSource bpmSource, double triggerFraction, List<OnceEveryFunc> sequence){
        //The fractional beat upon which the trigger first. 0.25
        //would mean every 16th note, 1 would be every quarter
        this.triggerFraction = triggerFraction; 
    }

    public double lengthInBeats {
        get { return this.sequence.size * this.frac;
    }

    public double lengthInTriggers {
        get { return this.sequence.Size; }
    }

    //Amount of triggers that happen in one beat
    /*get triggersPerBeat() {
        return 1/this.triggerFraction;
    }

    get currentTriggerIndex() {
        let currTrigger = this._tapTool.getFractionalBeat(this.triggerFraction); //Current resolution beat int
        currTrigger = Math.floor(currTrigger % this.lengthInTriggers);
        return currTrigger;
    }*

    public void updoot() {
        double currBeat = this.bpmSource.getFractionalBeat(this.frac);
        //double currSeq = this.bpmSource.getFractionalBeat(this.frac);

        let currBeat = this._tapTool.getFractionalBeat(this.triggerFraction); //Current resolution beat int
        let currSeq = this._tapTool.getFractionalBeat(this.lengthInBeats);     //Current sequence int
        if(this._lastBeat !== currBeat) {
            this._lastBeat = currBeat;
            //Pos in the sequence array
            let func = this._sequence[this.currentTriggerIndex];
            if(typeof func === "function") {
                func(currSeq, currBeat, this._tapTool);
            }
        }
    }
}
*/

}