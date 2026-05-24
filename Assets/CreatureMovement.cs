using UnityEngine;

public class CreatureMovement : MonoBehaviour
{
    public enum CurveType { QuadraticArc, CubicSCurve }

    [Header("Movement Setup")]
    public CurveType pathType = CurveType.QuadraticArc;
    public float moveSpeed = 4f; 
    [HideInInspector] public float sideOffsetDistance = 5f; 

    private Vector3 startPoint;
    private Transform targetPlayer; 
    private float progress = 0f;
    private float initialDistance;

    public void InitializePath(Vector3 start, Transform player)
    {
        startPoint = start;
        targetPlayer = player;
        startPoint.y = 0; 
        progress = 0f;

        Vector3 endPoint = targetPlayer.position;
        endPoint.y = 0;
        initialDistance = Vector3.Distance(startPoint, endPoint);

        if (initialDistance < 0.1f) initialDistance = 0.1f;
    }

    void Update()
    {
        if (targetPlayer == null || progress >= 1f) return;

        Vector3 endPoint = targetPlayer.position;
        endPoint.y = 0; 

        Vector3 direction = (endPoint - startPoint).normalized;
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;

        // Uses the offset distance directly passed from the spawner profile mirror calculations
        Vector3 sideOffset = perpendicular * sideOffsetDistance;

        Vector3 controlPoint1 = Vector3.zero;
        Vector3 controlPoint2 = Vector3.zero;

        if (pathType == CurveType.QuadraticArc)
        {
            Vector3 midPoint = Vector3.Lerp(startPoint, endPoint, 0.5f);
            controlPoint1 = midPoint + sideOffset;
        }
        else
        {
            Vector3 firstThird = Vector3.Lerp(startPoint, endPoint, 0.33f);
            Vector3 secondThird = Vector3.Lerp(startPoint, endPoint, 0.66f);
            controlPoint1 = firstThird + sideOffset;
            controlPoint2 = secondThird - sideOffset;
        }

        progress += (moveSpeed / initialDistance) * Time.deltaTime;
        progress = Mathf.Clamp01(progress);

        Vector3 nextPosition;
        if (pathType == CurveType.QuadraticArc)
        {
            nextPosition = Bezier.GetQuadraticPoint(startPoint, controlPoint1, endPoint, progress);
        }
        else
        {
            nextPosition = Bezier.GetCubicPoint(startPoint, controlPoint1, controlPoint2, endPoint, progress);
        }

        nextPosition.y = 0;

        Vector3 moveDirection = (nextPosition - transform.position).normalized;
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }

        transform.position = nextPosition;

        if (progress >= 0.95f || Vector3.Distance(transform.position, endPoint) < 0.6f)
        {
            PlayerMovement playerScript = targetPlayer.GetComponent<PlayerMovement>();
            if (playerScript != null)
            {
                Debug.Log("Creature caught the player!");
                playerScript.Die();
            }
            Destroy(gameObject);
        }
    }
}