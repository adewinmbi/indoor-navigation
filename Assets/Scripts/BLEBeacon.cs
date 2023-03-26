using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BLEBeacon : MonoBehaviour {

    [SerializeField] private GameObject walker;
    [SerializeField] private GameObject watch;

    // Update is called once per frame
    void Update() {
        float dot = Vector3.Dot(walker.transform.forward, (watch.transform.position - walker.transform.position).normalized);
        Debug.Log("Beacon: " + dot);
    }
}
