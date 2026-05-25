using UnityEngine;

public class FlameTurret : BaseTurret
{
    [Header("Flame Settings")]
    public VisualBullet bulletPrefab;
    public float coneAngle = 30f;
    public float fireRate = 0.1f;   
    public float rotationSpeed = 5f;
    private float nextFireTime;

    protected override void DrawRange()
    {   
        lineRenderer.positionCount = 3;
        Vector3 leftEdge = Quaternion.Euler(0, -coneAngle, 0) * Vector3.forward * range;
        Vector3 rightEdge = Quaternion.Euler(0, coneAngle, 0) * Vector3.forward * range;
        
        lineRenderer.SetPosition(0, leftEdge);
        lineRenderer.SetPosition(1, Vector3.zero);
        lineRenderer.SetPosition(2, rightEdge);
    }

    protected override void Attack()
    {
        if (!isActive) return;

        Transform currentTarget = GetPriorityTarget();
        if (currentTarget == null) return;

        Vector3 dirToTarget = (currentTarget.position - transform.position).normalized;
        dirToTarget.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(dirToTarget); 
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        float angleToTarget = Vector3.Angle(transform.forward, dirToTarget);

        if (angleToTarget < coneAngle)
        {
            if (Time.time >= nextFireTime)
            {
                FireFlame();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void FireFlame()
    {
        if (bulletPrefab == null) return;

        float randomSpread = Random.Range(-5f, 5f);
        Quaternion bulletRotation = transform.rotation * Quaternion.Euler(0, randomSpread, 0);

        VisualBullet tempBullet = Instantiate(bulletPrefab, transform.position, bulletRotation);
        tempBullet.transform.parent = null; 
        tempBullet.gameObject.SetActive(true);
        
        tempBullet.speed = 20f; 
        tempBullet.maxDistance = range; 
    }

    private Transform GetPriorityTarget()
    {
        if (player != null)
        {
            float distToPlayer = Vector3.Distance(transform.position, player.position);
            if (distToPlayer <= range)
            {
                return player; 
            }
        }

        Transform closestCreature = null;
        float closestDistance = range;

        CreatureMovement[] activeCreatures = FindObjectsByType<CreatureMovement>(FindObjectsSortMode.None);
        foreach (CreatureMovement creature in activeCreatures)
        {
            if (creature == null) continue;
            float distToCreature = Vector3.Distance(transform.position, creature.transform.position);
            if (distToCreature < closestDistance)
            {
                closestDistance = distToCreature;
                closestCreature = creature.transform;
            }
        }

        return closestCreature;
    }

    void Update() => Attack();
}