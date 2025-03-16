using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveCoefficient = 1f;
    public float zoomCoefficient = 1f;

    Vector3 holdMousePosition;
    Vector3 holdCameraPosition;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            holdMousePosition = Input.mousePosition;
            holdCameraPosition = transform.position;
        }
        if (Input.GetMouseButton(1)) transform.position = holdCameraPosition + 
            moveCoefficient * Camera.main.orthographicSize / 100f * (holdMousePosition - Input.mousePosition);
        transform.position = new Vector3(transform.position.x, transform.position.y, -100f);
        Camera.main.orthographicSize -= zoomCoefficient * Input.mouseScrollDelta.y;
    }
}
