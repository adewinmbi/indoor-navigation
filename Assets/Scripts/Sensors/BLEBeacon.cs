using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BLEBeacon : MonoBehaviour {

    [SerializeField] private GameObject watch;
    [SerializeField] private string beaconName;
    [SerializeField] private Text displayText;

    private float sensorReading = 0;

    public float GetReading() {
        return sensorReading;
    }

    // Update is called once per frame
    void Update() {
        float dot = Vector3.Dot(transform.up, (watch.transform.position - transform.position).normalized);
        float distance = Vector3.Distance(transform.position, watch.transform.position);
        sensorReading = dot * distance;

        displayText.text = beaconName + ": " + string.Format("{0:0.0#}", sensorReading);
    }
}
