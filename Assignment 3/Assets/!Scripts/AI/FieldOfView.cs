using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private float m_viewAngle = 90f;
    [SerializeField] private float m_distance = 3f;
    [SerializeField] private int m_resolution = 8;

    [Space]
    [SerializeField] private MeshRenderer m_meshRenderer;
    [SerializeField] private MeshFilter m_meshFilter;

    private Mesh m_viewMesh;

    private void Awake()
    {
        m_viewMesh = ConstructViewMesh();
        m_meshFilter.mesh = m_viewMesh;
    }

    private Mesh ConstructViewMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[m_resolution + 1]; // +1 is starting vert
        Vector2[] uvs = new Vector2[vertices.Length];
        vertices[^1] = Vector3.zero; // set last index to starting pos
        uvs[^1] = new Vector2(vertices[^1].x + 0.5f, vertices[^1].z);

        float angleIncrement = m_viewAngle / m_resolution;
        float angle = -m_viewAngle / 2f + angleIncrement / 2f;
        float initAngle = angle;

        for (int i = 0; i < m_resolution; i++)
        {
            vertices[i] = Quaternion.Euler(0, angle, 0) * Vector3.forward * m_distance;
            

            float uvX = Mathf.InverseLerp(initAngle, m_viewAngle / 2f, angle);
            float uvY = vertices[i].z / m_distance;
            uvs[i] = new Vector2(uvX, uvY);

            Debug.DrawLine(transform.position, vertices[i], Color.red, 20f);
            angle += angleIncrement;
        }

        // final vert
        

        int[] triangles = new int[m_resolution * 3];

        int triIndex = 0;
        for (int i = 0; i < triangles.Length - 2; i+= 3)
        {
            triangles[i] = m_resolution;
            triangles[i + 1] = triIndex;
            triangles[i + 2] = ++triIndex;
        }



        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.Optimize();
        
        return mesh;
    }

    public bool CheckForTarget(int targetLayer)
    {
        for (int i = 0; i < m_viewMesh.vertices.Length; i++)
        {
            if (Physics.Raycast(transform.position, (m_viewMesh.vertices[i] - transform.position).normalized, out RaycastHit hit, m_distance) && hit.collider.gameObject.layer == targetLayer)
                return true;
        }

        return false;
    }
}
