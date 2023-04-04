using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BLEBeacon : MonoBehaviour {

    [SerializeField] private GameObject watch;
    [SerializeField] private string beaconName;
    [SerializeField] private Text displayText;

    // Update is called once per frame
    void Update() {
        float dot = Vector3.Dot(transform.up, (watch.transform.position - transform.position).normalized);
        float distance = Vector3.Distance(transform.position, watch.transform.position);
        displayText.text = beaconName + ": " + dot;
    }
}
