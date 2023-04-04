using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BLENavigation : MonoBehaviour {

    [SerializeField] private BLEBeacon beaconL;
    [SerializeField] private BLEBeacon beaconR;
    [SerializeField] private BLEBeacon beaconM;
    [SerializeField] private float rotationSpeed;
    private readonly float tolerance = 0.1f;
    private float rotation = 0;
    private Rigidbody rBody;

    private bool navEnabled = false; // When true, navigation algorithm is activated.

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

        Debug.Log("Middle largest: " + middleIsGreater + "\nLeftRight: " + leftEqualsRight);
        return middleIsGreater && leftEqualsRight;
    }

    // Check if value is within a tolerance of its target.
    private bool InRange(float value, float target, float tolerance) {
        float upperBound = target + tolerance;
        float lowerBound = target - tolerance;

        return (value > lowerBound) && (value < upperBound);
    }

    private void FixedUpdate() {
        if (navEnabled && !AtSetpoint()) {
            // This could be replaced with a control loop in real life
            if (beaconL.GetReading() > beaconR.GetReading()) {
                rotation = -1; // Turn left
            }

            if (beaconR.GetReading() > beaconL.GetReading()) {
                rotation = 1; // Turn right
            }

            rBody.angularVelocity = Time.deltaTime * Vector3.up * (rotation * rotationSpeed);
        } else {
            rBody.angularVelocity = Vector3.zero; // Disable movement when at setpoint
        }
    }

}
