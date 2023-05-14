using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

    public int speed = 1;
    private float rotationCounter = 0;
    private bool rotationCountDebounce = false;

    void Update() {
        transform.Rotate(Vector3.up * Time.deltaTime * speed);
        Vector3 eulerRotation = transform.localRotation.eulerAngles;

        if (BLENavigation.InRange(eulerRotation.y, 0, 5)) { // If rotation is within 5 degrees of 0, increment the counter.
            if (rotationCountDebounce == true) {
                rotationCounter++;
            }
            rotationCountDebounce = false;
        }

        // Debounce to ensure rotationCounter is not incremented several times.
        if (BLENavigation.InRange(eulerRotation.y, 180, 5)) {
            rotationCountDebounce = true;
        }

        Debug.Log(rotationCounter);
    }

    public void InitRotationCounter() {
        rotationCounter = 0;
    }

    /// <summary>
    /// Keeps track of if a full rotation has occurred.
    /// </summary>
    /// <returns>Returns true if one full rotation has passed since InitRotationCounter was called.</returns>
    public bool FullRotation() {
        return true;
    }

}
