using UnityEngine;

public class WatchMovement : MonoBehaviour {

    [SerializeField] private PointCloud pointCloud;
    [SerializeField] private Vector2 translationSpeed;
    [SerializeField] private double rotationSpeed;

    private void Update() {
        Vector2 translation = new Vector2();
        if (Input.GetKey(KeyCode.UpArrow)) {
            translation.y = 1;
        }

        if (Input.GetKey(KeyCode.DownArrow)) {
            translation.y = -1;
        }

        if (Input.GetKey(KeyCode.RightArrow)) {
            translation.x = 1;
        }

        if (Input.GetKey(KeyCode.LeftArrow)) {
            translation.x = -1;
        }

        translation *= Time.deltaTime * translationSpeed;
        transform.Translate(new Vector3(translation.x, 0, translation.y));

        pointCloud.UpdateWalkerPosition(transform.position);
        pointCloud.ShiftPointCloud(transform.position);
    }

}
