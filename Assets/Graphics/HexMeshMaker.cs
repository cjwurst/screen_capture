using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMeshMaker : MonoBehaviour
{
    Mesh mesh;
    
    void Start()
    {
        const float SQRT3_2 = 0.86602540378f;

        Vector3[] vertices;
        Vector2[] uv;
        int[] triangles;

        vertices = new Vector3[]
        {
            Vector3.zero,
            new Vector3(-SQRT3_2, 0.5f, 0),
            new Vector3(0, 1, 0),
            new Vector3(SQRT3_2, 0.5f, 0),
            new Vector3(SQRT3_2, -0.5f, 0),
            new Vector3(0, -1, 0),
            new Vector3(-SQRT3_2, -0.5f, 0)
        };
        for (int i = 0; i < 7; i++)
            vertices[i] /= 2;
        uv = new Vector2[7];
        for (int i = 0; i < 7; i++)
            uv[i] = new Vector2(vertices[i].x, vertices[i].y) + new Vector2(0.5f, 0.5f);

        triangles = new int[] 
        {
            1, 2, 0,
            2, 3, 0,
            3, 4, 0,
            4, 5, 0,
            5, 6, 0,
            6, 1, 0
        };

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uv);
        mesh.SetTriangles(triangles, 0);
    }
}
