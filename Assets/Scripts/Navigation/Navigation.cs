using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Navigation : MonoBehaviour {

    [SerializeField] private AStar aStar;
    [SerializeField] private Rotate lidarRotate;
    [SerializeField] private Distance distanceSensor;
    [SerializeField] private BLEBeacon beaconL, beaconR, beaconM;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Text walkerStatusUpdate;

    [SerializeField] private readonly float bleRotationTolerance = 0.3f;
    [SerializeField] private readonly float bleProximityTolerance = 3;
    [SerializeField] private readonly float movementSpeed = 5;

    private Rigidbody rBody;
    private float rotation = 0;
    private bool navEnabled = false;

    private enum WalkerState {
        DirectDrive,
        BLEDrive,
        LidarDrive,
        Idle,
        Arrived
    }

    private WalkerState walkerState = WalkerState.Idle;

    public void ToggleNavigation(bool isEnabled) {
        navEnabled = isEnabled;

        if (!isEnabled) {
            rBody.angularVelocity = Vector3.zero;
        }
    }

    public static bool InRange(float value, float target, float tolerance) {
        float upperBound = target + tolerance;
        float lowerBound = target - tolerance;

        return (value > lowerBound) && (value < upperBound);
    }

    private void Start() {
        rBody = GetComponent<Rigidbody>();
        rBody.detectCollisions = false;
        StartCoroutine(RunNavigation());
    }

    /// <summary>
    /// Determines if bluetooth beacons reached their target.
    /// </summary>
    /// <returns>Return true when middle is greater than both left and right, 
    /// and left and right are within tolerance of each other (nearly the same value).</returns>
    private bool AtSetpoint() {
        bool middleIsGreatest = (beaconM.GetReading() > beaconL.GetReading()) && (beaconM.GetReading() > beaconR.GetReading());
        bool leftEqualsRight = InRange(beaconL.GetReading(), beaconR.GetReading(), bleRotationTolerance);

        bool result = middleIsGreatest && leftEqualsRight;
        if (result) {

            // Condition to switch to lidar navigation.
            if (beaconM.GetReading() > distanceSensor.GetReading() + 1) { // 1 is a tolerance, because if no obstacles are present, the beacon and distance readings are extremely close.
                walkerState = WalkerState.LidarDrive;

            } else { // Condition to drive straight towards watch
                walkerState = WalkerState.DirectDrive;
            }
        }

        return result;
    }

    private bool lidarDebounce = false;
    private IEnumerator RunNavigation() {
        while (true) {
            walkerStatusUpdate.text = walkerState.ToString();
            if (navEnabled) {
                switch (walkerState) {

                    case WalkerState.DirectDrive:
                        if (distanceSensor.GetReading() > distanceSensor.distanceTolerance) {
                            Vector3 translation = Vector3.forward;
                            translation *= Time.deltaTime * movementSpeed;
                            transform.Translate(translation);
                        } else {
                            walkerState = WalkerState.Arrived;
                        }
                        break;

                    case WalkerState.LidarDrive:
                        if (lidarDebounce) {
                            lidarRotate.ResetRotationCounter();
                            lidarDebounce = false;
                        }

                        if (lidarRotate.FullRotation()) {
                            aStar.GeneratePath();

                            // Navigate to next point in path.
                            yield return new WaitUntil(() => aStar.MoveToNextPoint(movementSpeed));

                            if (navEnabled && IsBeaconInRange()) {
                                /* When using A* navigation, there is rarely a time when the walker 
                                 * is directly aligned with the watch enough to go into direct drive mode. 
                                 * So, this uses beacons to check for proximity to turn off the walker. */
                                walkerState = WalkerState.Arrived;
                            }
                            lidarDebounce = true;
                        }
                        break;

                    default:
                        if (!AtSetpoint()) {
                            walkerState = WalkerState.BLEDrive;

                            // This could be replaced with a control loop in real life.
                            if (beaconL.GetReading() > beaconR.GetReading()) {
                                rotation = -1; // Turn left
                            }

                            if (beaconR.GetReading() > beaconL.GetReading()) {
                                rotation = 1; // Turn right
                            }

                            // Clamp x and z rotation
                            transform.Rotate(Vector3.up, rotation);
                        }
                        break;
                }

            } else { // Navigation disabled
                walkerState = WalkerState.Idle;
                rBody.angularVelocity = Vector3.zero;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private bool IsBeaconInRange() {
        return Mathf.Abs(beaconL.GetReading()) < bleProximityTolerance
            && Mathf.Abs(beaconM.GetReading()) < bleProximityTolerance
            && Mathf.Abs(beaconR.GetReading()) < bleProximityTolerance;
    }

}
