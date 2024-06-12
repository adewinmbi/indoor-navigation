using UnityEngine;

public class Distance : MonoBehaviour {

    [SerializeField] public readonly float distanceTolerance = 3; // Distance away the walker should stop from the watch.
    private readonly int layerMask = ~(1 << 2); // Mask for everything EXCEPT layer 2, the player layer

    private float currentDistance = 0;

    void FixedUpdate() {

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask)) {

            Vector3 currentHitPos = hit.point;
            currentDistance = Vector3.Distance(currentHitPos, transform.position);
            
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.blue);
        }
    }

    public float GetReading() {
        return currentDistance;
    }

}
