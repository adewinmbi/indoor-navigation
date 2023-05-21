using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerMovement : MonoBehaviour {

    [SerializeField] private PointCloud pointCloud;
    [SerializeField] private Vector2 translationSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Navigation bleNavigation;

    private void Update() {
        Vector2 translation = new Vector2();
        float rotation = 0;

        if (Input.GetKey(KeyCode.W)) {
            translation.y = 1;
        }

        if (Input.GetKey(KeyCode.S)) {
            translation.y = -1;
        }

        if (Input.GetKey(KeyCode.D)) {
            translation.x = 1;
        }

        if (Input.GetKey(KeyCode.A)) {
            translation.x = -1;
        }

        if (Input.GetKey(KeyCode.Q)) {
            rotation = -1;
        }

        if (Input.GetKey(KeyCode.E)) {
            rotation = 1;
        }

        // Enable rotation navigation when 'B' is held
        bleNavigation.ToggleNavigation(Input.GetKey(KeyCode.B));

        translation *= Time.deltaTime * translationSpeed;
        rotation *= Time.deltaTime * rotationSpeed;
        transform.Translate(new Vector3(translation.x, 0, translation.y));
        transform.Rotate(Vector3.up, rotation);

        pointCloud.UpdateWalkerPosition(transform.position);
        pointCloud.ShiftPointCloud(transform.position);
    }

}
