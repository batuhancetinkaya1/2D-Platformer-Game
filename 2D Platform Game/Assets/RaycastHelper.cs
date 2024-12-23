using UnityEngine;

public class RaycastHelper : MonoBehaviour
{
    /// <summary>
    /// Oyuncunun belirli bir görüþ mesafesi ve açýsýnda olup olmadýðýný kontrol eder.
    /// </summary>
    public static bool IsTargetInView(Vector2 origin, Transform target, float viewDistance, float viewAngle, int facingDirection, LayerMask layerMask)
    {
        if (target == null) return false;

        // Koniyi küçük açýlara böl
        int segments = 30; // Daha fazla segment daha hassas algýlama saðlar
        for (int i = 0; i <= segments; i++)
        {
            // Her segmentin açýsýný hesapla
            float angle = Mathf.Lerp(-viewAngle / 2f, viewAngle / 2f, (float)i / segments);
            Vector2 direction = Quaternion.Euler(0, 0, angle) * (Vector2.right * facingDirection);

            // Raycast gönder
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, viewDistance, layerMask);

            // Eðer hedef algýlandýysa
            if (hit.collider != null && hit.collider.transform == target)
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Gizmos ile algýlama konisini sahnede çizer.
    /// </summary>
    public static void DrawGizmos(Vector2 origin, float viewDistance, float viewAngle, int facingDirection)
    {
        // Green fill color for the sector
        Color sectorFillColor = new Color(0f, 1f, 0f, 0.5f); // Semi-transparent green

        // Create the mesh for the circular sector
        Mesh sectorMesh = CreateSectorMesh(origin, viewDistance, viewAngle, facingDirection);
        if (sectorMesh != null)
        {
            Material material = new Material(Shader.Find("Sprites/Default")); // Unity default shader
            material.color = sectorFillColor;

            // Draw the mesh
            material.SetPass(0);
            Graphics.DrawMeshNow(sectorMesh, Matrix4x4.identity);
        }
    }



    /// <summary>
    /// Bir görüþ konisi için Mesh oluþturur.
    /// </summary>
    private static Mesh CreateSectorMesh(Vector2 origin, float radius, float angle, int facingDirection, int segments = 50)
    {
        Mesh mesh = new Mesh();

        // Number of vertices (center + segment edges)
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3]; // Each triangle has 3 indices

        // Center vertex
        vertices[0] = origin;

        // Generate vertices along the arc
        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = Mathf.Lerp(-angle / 2f, angle / 2f, (float)i / segments);
            Vector2 direction = Quaternion.Euler(0, 0, currentAngle) * (Vector2.right * facingDirection);
            vertices[i + 1] = origin + direction * radius;

            if (i < segments)
            {
                // Create a triangle with the center and two consecutive points on the arc
                triangles[i * 3] = 0;        // Center
                triangles[i * 3 + 1] = i + 1; // Current arc vertex
                triangles[i * 3 + 2] = i + 2; // Next arc vertex
            }
        }

        // Assign vertices and triangles to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

}
