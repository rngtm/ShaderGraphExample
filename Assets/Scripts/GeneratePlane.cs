using UnityEngine;

/// <summary>
/// 板状のメッシュを作成するC#スクリプト
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class GeneratePlane : MonoBehaviour
{
    [SerializeField] bool generateMeshOnUpdate = false; // 毎フレームメッシュ更新
    [SerializeField] int xCount = 25; // x方向の頂点数
    [SerializeField] int zCount = 25; // y方向の頂点数
    [SerializeField] float planeSize = 10f; // 板のサイズ
    Mesh mesh;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        Create();
    }

    private void Update()
    {
        if (generateMeshOnUpdate)
        {
            Create();
        }
    }

    /// <summary>
    /// メッシュ作成
    /// </summary>
    private void Create()
    {
        if (xCount < 2) xCount = 2;
        if (zCount < 2) zCount = 2;

        // 頂点座標(vertices)と法線(normals)作成
        Vector3[] vertices = new Vector3[xCount * zCount];
        Vector3[] normals = new Vector3[vertices.Length];
        int vi = 0;
        for (int z = 0; z < zCount; z++)
        {
            for (int x = 0; x < xCount; x++)
            {
                vertices[vi++] = new Vector3(
                    (float)x / (xCount - 1) - 0.5f, 
                    0f, 
                    (float)z / (zCount - 1) - 0.5f
                    ) * planeSize;
            }
        }
        for (int i = 0; i < vertices.Length; i++)
        {
            normals[i] = new Vector3(0f, 1f, 0f);
        }

        // 頂点インデックス(triangles)作成
        int[] triangles = new int[(xCount - 1) * (zCount - 1) * 6];
        int ti = 0;
        int triangleOffset = 0;
        for (int z = 0; z < zCount - 1; z++)
        {
            for (int x = 0; x < xCount - 1; x++)
            {
                triangles[ti++] = triangleOffset + xCount; // 2
                triangles[ti++] = triangleOffset + 1; // 1
                triangles[ti++] = triangleOffset + 0; // 0
                triangles[ti++] = triangleOffset + xCount + 1; // 3
                triangles[ti++] = triangleOffset + 1; // 1
                triangles[ti++] = triangleOffset + xCount; // 2
                triangleOffset++;
            }
            triangleOffset++;
        }

        // メッシュ更新
        mesh.triangles = null;
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = triangles;
    }
}
