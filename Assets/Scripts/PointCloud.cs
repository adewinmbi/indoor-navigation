using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointCloud : MonoBehaviour {

    [SerializeField] private Image pointIcon, walkerIcon, pointCloudHolder, pointCloudHolderPrefab;
    [SerializeField] private GameObject watch, debugPointIcon, pointCloudHolderParent;

    [SerializeField] private readonly int scale = 17; // Constants for display purposes
    [SerializeField] private readonly float resolutionDecreaseFactor = 1; // Lidar points will be clamped to every X units vertically and horizontally

    private List<Vector2> hitPoints = new();
    private Vector2 walkerPointPosition = new();
    private Vector2 watchPointPosition = new();
    private Dictionary<string, List<Vector2>> pointCloudGroups = new();

    private void Start() {
        // Draw watch (target) on the point cloud
        watchPointPosition = WorldToPoint(watch.transform.position);
        DrawPoint(watchPointPosition, Color.black);
    }

    public Vector2 WorldToPoint(Vector3 worldPos) {
        Vector2 pointPos = new Vector2();
        pointPos.x = RoundToNearest(resolutionDecreaseFactor, worldPos.x) * scale;
        pointPos.y = RoundToNearest(resolutionDecreaseFactor, worldPos.z) * scale;

        return pointPos;
    }

    /// <summary>
    /// Divides given vector by display scale.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public Vector2 PointToWorld(Vector2 point) {
        return point / scale;
    }

    public void UpdateWalkerPosition(Vector3 position) {
        Vector2 pointPos = WorldToPoint(position);
        walkerPointPosition = pointPos;
        walkerIcon.rectTransform.localPosition = new Vector3(pointPos.x, pointPos.y);
    }

    public void AddPoint(Vector3 hitPoint) {
        // Decrease resolution of fixed hit point
        Vector2 fixedHitPoint = new Vector2();
        fixedHitPoint.x = RoundToNearest(resolutionDecreaseFactor, hitPoint.x);
        fixedHitPoint.y = RoundToNearest(resolutionDecreaseFactor, hitPoint.z);

        if (!hitPoints.Contains(fixedHitPoint)) {
            hitPoints.Add(fixedHitPoint);

            // UI for hit point
            DrawPoint(fixedHitPoint, Color.red, "Obstacle");
        }
    }

    public void DrawPoint(Vector2 point, Color color) {
        Image newPointIcon = Instantiate(pointIcon, pointCloudHolder.transform);
        newPointIcon.gameObject.SetActive(true);
        newPointIcon.rectTransform.localPosition = new Vector3(point.x * scale, point.y * scale);
        newPointIcon.color = color;
    }

    public void DrawPoint(Vector2 point, Color color, string pointCloudGroupName) {
        GameObject pointCloudGroupHolder;

        // Find parent, or create one if needed
        if (!pointCloudGroups.ContainsKey(pointCloudGroupName)) {
            pointCloudGroupHolder = Instantiate(pointCloudHolderPrefab.gameObject, pointCloudHolderParent.transform);
            pointCloudGroupHolder.name = pointCloudGroupName;
            pointCloudGroups.Add(pointCloudGroupName, new List<Vector2>());
        } else {
            pointCloudGroupHolder = pointCloudHolderParent.transform.Find(pointCloudGroupName).gameObject;
        }

        Image newPointIcon = Instantiate(pointIcon, pointCloudGroupHolder.transform);
        newPointIcon.gameObject.SetActive(true);
        newPointIcon.rectTransform.localPosition = new Vector3(point.x * scale, point.y * scale);
        pointCloudGroups[pointCloudGroupName].Add(point);
        newPointIcon.color = color;
    }

    public void RemoveAllPoints(string pointCloudGroupName) {
        if (!pointCloudGroups.ContainsKey(pointCloudGroupName)) {
            return;
        }

        // Destroy all children.
        foreach (Transform child in pointCloudHolderParent.transform.Find(pointCloudGroupName).transform) {
            Destroy(child.gameObject);
        }
    }

    public void DrawDebugPoint(Vector2 point, Color color, string message) {
        GameObject newPoint = Instantiate(debugPointIcon, pointCloudHolder.transform);
        Image newPointIcon = newPoint.GetComponent<Image>();
        newPointIcon.gameObject.SetActive(true);
        newPointIcon.rectTransform.localPosition = new Vector3(point.x * scale, point.y * scale);
        newPointIcon.color = color;

        Text newPointText = newPoint.transform.Find("Text").GetComponent<Text>();
        newPointText.text = message;
    }

    // Very bad performance, calling GetComponent every frame
    public void ShiftPointCloud(Vector3 walkerDisplacement) {
        pointCloudHolder.rectTransform.localPosition = new Vector3(walkerDisplacement.x * -scale, walkerDisplacement.z * -scale);
        foreach (Transform pCGroupHolder in pointCloudHolderParent.transform) {
            pCGroupHolder.GetComponent<Image>().rectTransform.localPosition = new Vector3(walkerDisplacement.x * -scale, walkerDisplacement.z * -scale);
        }
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

    public void Clear() {
        hitPoints.Clear();
        RemoveAllPoints("Obstacle");
    }

    public List<Vector2> GetHitPoints() {
        return hitPoints;
    }

    public int GetScale() {
        return scale;
    }

    public Vector2 GetWalkerPointPosition() {
        return walkerPointPosition;
    }

    public Vector2 GetWatchPointPosition() {
        return watchPointPosition;
    }

}
