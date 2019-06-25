using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CobVisuals;

namespace CobVisualsExamples {

public class CubeChanger : MonoBehaviour {
    public BPMSource bpmSource;
    public System.Action changeFunc;
    public System.Action changeFuncFixed;

    // Start is called before the first frame update
    void Start() {
        changeFunc = bpmSource.onceEvery(1, 0.5, ()=>{
            this.GetComponent<Renderer>().material.color = Mathf.Floor((float)bpmSource.beat % 2) == 0 ? Color.red : Color.green;
        });
        changeFuncFixed = bpmSource.onceEvery(1, 0, ()=>{
            //Calculate a random position in the bounds of the object
            Bounds meshBounds = this.GetComponent<MeshFilter>().sharedMesh.bounds;
            Vector3 forcePosition = new Vector3(Random.value*2.0f-1.0f, Random.value*2.0f-1.0f, Random.value*2.0f-1.0f);
            forcePosition.Scale(meshBounds.extents);
            forcePosition += this.transform.position;
            Vector3 forceDirection = new Vector3(Random.value*2.0f-1.0f, Random.value*2.0f-1.0f, Random.value*2.0f-1.0f);
            forceDirection.Normalize();
            forceDirection *= 4;
            this.GetComponent<Rigidbody>().AddForceAtPosition(forceDirection, forcePosition, ForceMode.VelocityChange);//.
        });
    }

    // Update is called once per frame
    void Update() {
        changeFunc();
    }

    void FixedUpdate() {
        changeFuncFixed();
        if(!Input.GetKey(KeyCode.C)) {
            //Acceleration toward the origin
            Vector3 vec2Origin = this.transform.position * -1;
            vec2Origin.Normalize();
            vec2Origin *= 40f;
            this.GetComponent<Rigidbody>().AddForce(vec2Origin, ForceMode.Acceleration);
        }
    }
}

}