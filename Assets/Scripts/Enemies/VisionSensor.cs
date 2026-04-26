using UnityEngine;

public class VisionSensor : Sensor  
{
    [Header("Vision Settings")]
    [SerializeField] private float visionRange = 10f;
    [SerializeField] private float visionAngle = 90f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask obstacleLayer;

    private Transform _target;

    public override bool Detect(out Transform target)
    {
        target = null;
        Collider[] hits = Physics.OverlapSphere(transform.position, visionRange, targetLayer);
        foreach (Collider hit in hits)
        {
            Transform potentialTarget = hit.transform;
            Vector3 directionToTarget = (potentialTarget.position - transform.position).normalized;
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

            if (angleToTarget < visionAngle / 2)
            {
                if (!Physics.Linecast(transform.position, potentialTarget.position, obstacleLayer))
                {
                    _target = potentialTarget;
                    target = _target;
                    return true;
                }
            }
        }
        return false;
    }

}
