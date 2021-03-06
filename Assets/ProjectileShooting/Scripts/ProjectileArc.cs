using UnityEngine;

public class ProjectileArc : MonoBehaviour 
{
    [SerializeField]
    int iterations = 20;

    [SerializeField]
    Color errorColor;

    private Color initialColor;
    private LineRenderer lineRenderer;
    Material lineMaterial;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        initialColor = lineRenderer.material.color;
        lineMaterial = lineRenderer.sharedMaterial;
    }

    Vector2 offset;
    public void UpdateArc(float speed, float distance, float gravity, float angle, Vector3 direction, bool valid)
    {

        offset = lineMaterial.mainTextureOffset;
        offset.x -= 0.01f;
        lineMaterial.mainTextureOffset = offset;

        var originParent = transform.parent;
        transform.parent = null;
        transform.localScale = Vector3.one;
        transform.parent = originParent;

        Vector2[] arcPoints = ProjectileMath.ProjectileArcPoints(iterations, speed, distance, gravity, angle);        
        Vector3[] points3d = new Vector3[arcPoints.Length];

        for (int i = 0; i < arcPoints.Length; i++)
        {
            points3d[i] = new Vector3(0, arcPoints[i].y, arcPoints[i].x);
        }

        lineRenderer.positionCount = arcPoints.Length;
        lineRenderer.SetPositions(points3d);

        transform.rotation = Quaternion.LookRotation(direction);

        lineRenderer.material.color = valid ? initialColor : errorColor;
    }
}
