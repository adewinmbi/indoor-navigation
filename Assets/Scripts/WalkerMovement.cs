using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerMovement : MonoBehaviour {

    [SerializeField] private PointCloud pointCloud;
    [SerializeField] private Vector2 translationSpeed;
    [SerializeField] private double rotationSpeed;

    private void Update() {
        Vector2 translation = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * translationSpeed;
        translation *= Time.deltaTime;
        transform.Translate(new Vector3(translation.x, 0, translation.y));

        pointCloud.UpdateWalkerPosition(transform.position);
        pointCloud.ShiftPointCloud(transform.position);
    }

}
