using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Math = System.Math;
using UnityEngine;
using CobVisuals;

namespace CobVisualsExamples {

/// <summary>
/// A volume that will wrap all objects internally every frame
/// in the bounds of it's collider
/// </summary>
public class ModuloVolume : MonoBehaviour {
    private Bounds bounds;

    public enum WhichBounds { Collider, Mesh };
    public WhichBounds whichBounds;

    //True modulo, not remainder
    float TrueModf(float a, float b) {
        return a - b * (float)Math.Floor(a / b);
    }

    void Awake() {
        if(whichBounds == WhichBounds.Collider) {
            bounds = GetComponent<Collider>().bounds;
        }
        else if(whichBounds == WhichBounds.Mesh) {
            bounds = GetComponent<MeshFilter>().sharedMesh.bounds;
        }
        //Make local space, not world space
        bounds.center -= this.transform.position;
        Debug.Log(bounds);
    }

    void Update() {
        transform.Cast<Transform>().ToList()
            .Where(child => !bounds.Contains(child.localPosition)).ToList()
            .ForEach((child)=>{
                child.localPosition = new Vector3(
                    TrueModf(child.localPosition.x - bounds.min.x //Substract min (get it in a 0 - +Bounds range)
                    , bounds.size.x) +                            //Do the modulo wrapping
                    bounds.min.x,                                 //Readd the min
                    TrueModf(child.localPosition.y - bounds.min.y, bounds.size.y) + bounds.min.y,
                    TrueModf(child.localPosition.z - bounds.min.z, bounds.size.z) + bounds.min.z);
                });
    }
}

}