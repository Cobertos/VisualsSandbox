using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CobVisuals;

namespace CobVisualsExamples {

public class CubeSpawner : MonoBehaviour {
    public BPMSource bpmSource;
    public GameObject spawnPrefab;
    public System.Action spawnFunc;

    // Start is called before the first frame update
    void Start() {
        spawnFunc = bpmSource.onceEvery(4, 0, ()=>{
            GameObject go = Instantiate(spawnPrefab, Vector3.zero, Quaternion.identity);
            go.GetComponent<CubeChanger>().bpmSource = bpmSource;
        });
    }

    // Update is called once per frame
    void Update() {
        spawnFunc();
    }
}

}