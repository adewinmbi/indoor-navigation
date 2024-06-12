using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Lidar : MonoBehaviour {

    [SerializeField] private PointCloud pointCloud;
    private List<Vector3> hitPoints;

    private readonly int layerMask = ~(1 << 2); // Hit all layers except player layer 2

    private void Start() {
        hitPoints = new List<Vector3>();
    }

    void FixedUpdate() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask)) {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

            Vector3 currentHitPos = hit.point;
            if (hitPoints.LastOrDefault() != currentHitPos) { // Don't repeat several data points in a row, prevents unneeded points when the walker is static
                AddPoint(currentHitPos);
            }
        } else {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
        }
    }

    private void AddPoint(Vector3 hitPoint) {
        hitPoints.Add(hitPoint);
        pointCloud.AddPoint(hitPoint);
    }

}
