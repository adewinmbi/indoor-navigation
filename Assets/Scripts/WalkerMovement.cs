using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerMovement : MonoBehaviour {

    [SerializeField] private PointCloud pointCloud;

    private void Update() {
        pointCloud.ShiftPointCloud(transform.position);
    }

}
