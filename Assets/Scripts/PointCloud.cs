using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointCloud : MonoBehaviour {

    [SerializeField] private Image pointIcon;
    [SerializeField] private Image pointCloudHolder;
    private int scale = 17;

    public void AddPoint(Vector3 hitPoint) {
        Image newPointIcon = Instantiate(pointIcon, pointCloudHolder.transform);
        newPointIcon.rectTransform.localPosition = new Vector3(hitPoint.x * scale, hitPoint.z * scale);
    }

    public void ShiftPointCloud(Vector3 walkerDisplacement) {
        pointCloudHolder.rectTransform.localPosition = new Vector3(walkerDisplacement.x * -scale, walkerDisplacement.z * -scale);
    }

}
