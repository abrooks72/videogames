using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PGC_Mesh : MonoBehaviour
{
    Mesh mesh;
    public Material grassMat;

    Vector3[] vertices;
    int[] triangles;

    public int xSize = 20;
    public int zSize = 20;

    void Start()
    {
        mesh = new Mesh();
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer render = gameObject.AddComponent<MeshRenderer>();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<Renderer>().material = grassMat;
        CreateShape();
        UpdateMesh();
    }


    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        int i = 0;
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.PerlinNoise(x * .5f, z * .5f) * 10f;
                vertices[i] = new Vector3(x, height, z);
                i++;
            }
        }

        int myVertices = 0;
        int myTriangles = 0;
        triangles = new int[xSize * zSize * 6];

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[myTriangles + 0] = myVertices + (0);
                triangles[myTriangles + 1] = myVertices + (xSize + 1);
                triangles[myTriangles + 2] = myVertices + (1);
                triangles[myTriangles + 3] = myVertices + (1);
                triangles[myTriangles + 4] = myVertices + (xSize + 1);
                triangles[myTriangles + 5] = myVertices + (xSize + 2);

                myVertices++;
                myTriangles += 6;
            }
            myVertices++;
        }


    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        MeshCollider collider = gameObject.AddComponent<MeshCollider>();
        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
            return;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
    }

}
