using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointCloud : MonoBehaviour {

    [SerializeField] private Image pointIcon;
    [SerializeField] private Image background;

    public void AddPoint(Vector3 hitPoint) {
        Image newPointIcon = Instantiate(pointIcon, background.transform);
        newPointIcon.rectTransform.localPosition = new Vector3(hitPoint.x, hitPoint.z);
    }

}
