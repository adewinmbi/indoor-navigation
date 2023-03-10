using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

    public int speed = 1;

    void Update() {
        transform.Rotate(Vector3.up * Time.deltaTime * speed);
    }


}
