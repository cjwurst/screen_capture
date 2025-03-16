using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class RotateThatTurkey : MonoBehaviour
{
    public float rotateSpeed = 10f;

    void Update()
    {
        transform.RotateAround(Vector3.up, rotateSpeed * Time.deltaTime);
    }
}
