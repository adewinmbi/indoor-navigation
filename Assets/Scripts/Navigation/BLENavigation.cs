using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BLENavigation : MonoBehaviour {

    [SerializeField] private Rotate lidarRotate;
    [SerializeField] private Distance distanceSensor;
    [SerializeField] private BLEBeacon beaconL;
    [SerializeField] private BLEBeacon beaconR;
    [SerializeField] private BLEBeacon beaconM;
    [SerializeField] private float rotationSpeed;
    private readonly float tolerance = 0.3f;
    private float rotation = 0;
    private Rigidbody rBody;

    private bool navEnabled = false; // When true, navigation algorithm is activated.

    private enum WalkerState {
        DirectDrive,
        BLEDrive,
        LidarDrive,
        Idle,
        Arrived
    }

    private WalkerState walkerState = WalkerState.Idle;

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

        bool result = middleIsGreater && leftEqualsRight;
        if (result) { 
            
            // Condition to switch to lidar navigation.
            if (beaconM.GetReading() > distanceSensor.GetReading() + 1) { // 1 is a tolerance, as if no obstacles are present, the beacon and distance readings are extremely close.
                walkerState = WalkerState.LidarDrive;

            } else { // Condition to drive straight towards watch
                walkerState = WalkerState.DirectDrive;
            }
        }

        return result;
    }

    // Check if value is within a tolerance of its target.
    public static bool InRange(float value, float target, float tolerance) {
        float upperBound = target + tolerance;
        float lowerBound = target - tolerance;

        return (value > lowerBound) && (value < upperBound);
    }

    int debug = 0;
    private bool lidarDebounce = false;
    private void FixedUpdate() {
        // Debug.Log(walkerState);
        Debug.Log(walkerState.ToString() + " " + debug.ToString());

        if (navEnabled && !AtSetpoint() && walkerState != WalkerState.DirectDrive && walkerState != WalkerState.LidarDrive) {
            walkerState = WalkerState.BLEDrive;

            // This could be replaced with a control loop in real life
            if (beaconL.GetReading() > beaconR.GetReading()) {
                rotation = -1; // Turn left
            }

            if (beaconR.GetReading() > beaconL.GetReading()) {
                rotation = 1; // Turn right
            }

            // Clamp x and z rotation
            transform.Rotate(Vector3.up, rotation);

        } else if (navEnabled && walkerState == WalkerState.DirectDrive) { // Direct drive
            if (distanceSensor.GetReading() > distanceSensor.distanceTolerance) {
                Vector3 translation = Vector3.forward;
                translation *= Time.deltaTime * 5;
                transform.Translate(translation);
            } else {
                walkerState = WalkerState.Arrived;
            }

        } else if (navEnabled && walkerState == WalkerState.LidarDrive) { // Lidar drive
            if (lidarDebounce) {
                lidarRotate.ResetRotationCounter();
                lidarDebounce = false;
            }

            if (AtSetpoint()) { // Case in which walker is lidar navigating, and it is aligned with the watch. 
                if (beaconM.GetReading() <= distanceSensor.GetReading() + 1) {
                    walkerState = WalkerState.DirectDrive;
                } else {
                    // Next point
                    if (lidarRotate.FullRotation()) {
                        // Generate A* path.
                        // Navigate to next point in path.
                        debug++;
                        lidarDebounce = true;
                    }
                }
            } else { // WORKSSSSSSSSSS
                // next point
                if (lidarRotate.FullRotation()) {
                    // Generate A* path.
                    // Navigate to next point in path.
                    debug++;
                    lidarDebounce = true;
                }
            }

        } else { // Navigation disabled
            walkerState = WalkerState.Idle;
            rBody.angularVelocity = Vector3.zero; // Disable movement when at setpoint
        }
    }

}
