using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distance : MonoBehaviour {

    private float currentDistance = 0;

    void FixedUpdate() {
        // Bit shift the index of the layer (2) to get a bit mask
        int layerMask = 1 << 2;

        // This would cast rays only against colliders in layer 2.
        // But instead we want to collide against everything except layer 2. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask)) {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.blue);

            Vector3 currentHitPos = hit.point;
            currentDistance = Vector3.Distance(currentHitPos, transform.position);
            Debug.Log(currentDistance);
        } else {
            // Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            //Debug.Log("Did not Hit");
        }
    }

}
