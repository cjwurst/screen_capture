using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public float speed = 0.05f;

    private void Update()
    {
        Vector2Int inputAxis = new Vector2Int();

        if (Input.GetKey(KeyCode.S))
            inputAxis += Vector2Int.down;
        if (Input.GetKey(KeyCode.W))
            inputAxis += Vector2Int.up;
        if (Input.GetKey(KeyCode.A))
            inputAxis += Vector2Int.left;
        if (Input.GetKey(KeyCode.D))
            inputAxis += Vector2Int.right;

        transform.position += speed * new Vector3(inputAxis.x, inputAxis.y, 0f);
    }
}
