using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Lidar : MonoBehaviour {

    [SerializeField] private PointCloud pointCloud;
    private List<Vector3> hitPoints;

    private void Start() {
        hitPoints = new List<Vector3>();
    }

    void FixedUpdate() {
        // Bit shift the index of the layer (2) to get a bit mask
        int layerMask = 1 << 2;

        // This would cast rays only against colliders in layer 2.
        // But instead we want to collide against everything except layer 2. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask)) {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

            Vector3 currentHitPos = hit.point;
            if (hitPoints.LastOrDefault() != currentHitPos) { // Don't repeat several data points in a row, prevents unneeded points when the walker is static
                AddPoint(currentHitPos);
            }
        } else {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            //Debug.Log("Did not Hit");
        }
    }

    private void AddPoint(Vector3 hitPoint) {
        hitPoints.Add(hitPoint);
        pointCloud.AddPoint(hitPoint);
    }

}
