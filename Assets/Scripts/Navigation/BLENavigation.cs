using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BLENavigation : MonoBehaviour {

    [SerializeField] private Distance distanceSensor;

    [SerializeField] private BLEBeacon beaconL;
    [SerializeField] private BLEBeacon beaconR;
    [SerializeField] private BLEBeacon beaconM;
    [SerializeField] private float rotationSpeed;
    private readonly float tolerance = 0.3f;
    private float rotation = 0;
    private Rigidbody rBody;

    private bool navEnabled = false; // When true, navigation algorithm is activated.
    private bool clearPath = false;

    private void Start() {
        rBody = GetComponent<Rigidbody>();
        rBody.detectCollisions = false;
    }

    public void ToggleNavigation(bool isEnabled) {
        navEnabled = isEnabled;
        if (!isEnabled) {
            rBody.angularVelocity = Vector3.zero; // Stop movement when navigation ends.
        }
    }

    // Return true when middle is greater than both left and right, and left and right are within tolerance of eachother (nearly the same value).
    private bool AtSetpoint() {
        bool middleIsGreater = (beaconM.GetReading() > beaconL.GetReading()) && (beaconM.GetReading() > beaconR.GetReading());
        bool leftEqualsRight = InRange(beaconL.GetReading(), beaconR.GetReading(), tolerance);

        // Debug.Log("Middle largest: " + middleIsGreater + "\nLeftRight: " + leftEqualsRight);
        bool result = middleIsGreater && leftEqualsRight;
        if (result) { 
            
            // Condition to switch to lidar navigation.
            if (beaconM.GetReading() > distanceSensor.GetReading() + 1) { // 1 is a tolerance, as if no obstacles are present, the beacon and distance readings are extremely close.
                Debug.Log(Mathf.Round(beaconM.GetReading()) + " " + Mathf.Round(distanceSensor.GetReading()) + "Switch to lidar nav");

            } else { // Condition to drive straight towards watch
                clearPath = true;
            }
        }

        return result;
    }

    // Check if value is within a tolerance of its target.
    private bool InRange(float value, float target, float tolerance) {
        float upperBound = target + tolerance;
        float lowerBound = target - tolerance;

        return (value > lowerBound) && (value < upperBound);
    }

    private void FixedUpdate() {
        if (navEnabled && !AtSetpoint() && !clearPath) {
            // This could be replaced with a control loop in real life
            if (beaconL.GetReading() > beaconR.GetReading()) {
                rotation = -1; // Turn left
            }

            if (beaconR.GetReading() > beaconL.GetReading()) {
                rotation = 1; // Turn right
            }

            // Clamp x and z rotation
            transform.Rotate(Vector3.up, rotation);

        } else if (navEnabled && clearPath) { // Direct drive
            if (distanceSensor.GetReading() > distanceSensor.distanceTolerance) {
                Vector3 translation = Vector3.forward;
                translation *= Time.deltaTime * 5;

                Debug.Log(translation);
                transform.Translate(translation);
            } else {
                Debug.Log("Arrived!");
            }
        } else { // Navigation disabled
            rBody.angularVelocity = Vector3.zero; // Disable movement when at setpoint
        }
    }

}
