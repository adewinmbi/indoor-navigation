using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointCloud : MonoBehaviour {

    [SerializeField] private Image pointIcon;
    [SerializeField] private Image walkerIcon;
    [SerializeField] private Image pointCloudHolder;
    private int scale = 17; // Just for display purposes
    private int resolutionDecreaseFactor = 1; // Lidar points will be clamped to every X units vertically and horizontally
    private List<Vector2> hitPoints = new List<Vector2>();

    public void UpdateWalkerPosition(Vector3 position) {
        position.x = RoundToNearest(resolutionDecreaseFactor, position.x);
        position.z = RoundToNearest(resolutionDecreaseFactor, position.z);
        walkerIcon.rectTransform.localPosition = new Vector3(position.x * scale, position.z * scale);
    }

    public void AddPoint(Vector3 hitPoint) {

        // Decrease resolution of fixed hit point
        Vector2 fixedHitPoint = new Vector2();
        fixedHitPoint.x = RoundToNearest(resolutionDecreaseFactor, hitPoint.x);
        fixedHitPoint.y = RoundToNearest(resolutionDecreaseFactor, hitPoint.z);

        if (!hitPoints.Contains(fixedHitPoint)) {
            hitPoints.Add(fixedHitPoint);

            // UI for hit point
            Image newPointIcon = Instantiate(pointIcon, pointCloudHolder.transform);
            newPointIcon.gameObject.SetActive(true);
            newPointIcon.rectTransform.localPosition = new Vector3(fixedHitPoint.x * scale, fixedHitPoint.y * scale);

            Debug.Log("Lidar map has changed!");
            // Regenerate path here
        }
    }

    public void ShiftPointCloud(Vector3 walkerDisplacement) {
        pointCloudHolder.rectTransform.localPosition = new Vector3(walkerDisplacement.x * -scale, walkerDisplacement.z * -scale);
    }

    /// <summary>
    /// Rounds given number to nearest interval. Returns higher number if the number is exactly between upper and lower bounds.
    /// </summary>
    /// <returns>Rounded number</returns>
    private float RoundToNearest(float interval, float value) {
        float lowerBound = value - (value % interval);
        float upperBound = lowerBound + interval;
        float midPoint = (upperBound - lowerBound) / 2;

        if (value < midPoint) {
            return lowerBound;
        } else {
            return upperBound;
        }
    }

}
