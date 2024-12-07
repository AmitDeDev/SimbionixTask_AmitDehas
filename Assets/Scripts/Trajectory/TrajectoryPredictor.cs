using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryPredictor : MonoBehaviour
{
    #region Fields and Properties
    private LineRenderer trajectoryLine;

    [Header("Trajectory Settings")]
    [SerializeField, Range(10, 100)]
    public int maxPoints = 50; 

    [SerializeField, Range(0.01f, 0.5f)]
    private float increment = 0.025f;

    [SerializeField, Range(1.05f, 2f)]
    private float rayOverlap = 1.1f; 
    #endregion

    private void Start()
    {
        trajectoryLine = GetComponent<LineRenderer>();
        SetTrajectoryVisible(true);
    }
    
    public void PredictTrajectory(ProjectileProperties projectile)
    {
        Vector3 velocity = projectile.direction * (projectile.initialSpeed / projectile.mass);
        Vector3 position = projectile.initialPosition;

        UpdateLineRender(1, (0, position)); // Set initial point

        for (int i = 1; i < maxPoints; i++)
        {
            // Estimate new velocity and position
            velocity = CalculateNewVelocity(velocity, projectile.drag, increment);
            Vector3 nextPosition = position + velocity * increment;

            // Calculate ray overlap to ensure continuous detection
            float overlap = Vector3.Distance(position, nextPosition) * rayOverlap;

            // Check for collision using raycasting
            if (Physics.Raycast(position, velocity.normalized, out RaycastHit hit, overlap))
            {
                UpdateLineRender(i, (i - 1, hit.point));
                break;
            }

            // Update trajectory line with calculated position
            position = nextPosition;
            UpdateLineRender(i + 1, (i, position));
        }
    }
    
    private void UpdateLineRender(int count, (int point, Vector3 pos) pointData)
    {
        trajectoryLine.positionCount = count;
        trajectoryLine.SetPosition(pointData.point, pointData.pos);
    }
    
    private Vector3 CalculateNewVelocity(Vector3 velocity, float drag, float increment)
    {
        velocity += Physics.gravity * increment;
        velocity *= Mathf.Clamp01(1f - drag * increment);
        return velocity;
    }
    
    public void SetTrajectoryVisible(bool visible)
    {
        if (trajectoryLine != null)
        {
            trajectoryLine.enabled = visible;
        }
    }
}
