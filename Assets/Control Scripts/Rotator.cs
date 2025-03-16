using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float acceleration = 0.1f;

    float speed = 0f;
    public float maxSpeed = 5f;

    public float damping = 0.95f;
    float stopThreshold = 0.001f;

    void Update()
    {
        int inputAxis = 0;
//#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.E))
            inputAxis--;
        if (Input.GetKey(KeyCode.Q))
            inputAxis++;
//#else
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase != TouchPhase.Stationary || touch.tapCount > 1)
                continue;
            if (touch.position.x > 206)
                inputAxis--;
            else
                inputAxis++;
        }
//#endif

        bool damp = false;
        if (Input.GetKey(KeyCode.Space))
            damp = true;

        speed += acceleration * inputAxis;
        if (Mathf.Abs(speed) > maxSpeed)
            speed = maxSpeed * Mathf.Abs(speed) / speed;
        if (damp) speed *= damping;
        if (Mathf.Abs(speed) < stopThreshold) speed = 0f;

        transform.Rotate(new Vector3(0f, 0f, 1f), speed);
    }
}
