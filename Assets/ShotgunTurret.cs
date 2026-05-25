using UnityEngine;

public class ShotgunTurret : BaseTurret
{
    [Header("Shotgun Settings")]
    public VisualBullet bulletPrefab;
    public int bulletCount = 5;
    public float spreadAngle = 45f;
    public float fireRate = 2f;
    public float rotationSpeed = 10f; 
    private float nextFireTime;

    protected override void DrawRange()
    {   
        int segments = 50;
        lineRenderer.positionCount = segments + 1;
        lineRenderer.loop = true;
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2 / segments;
            float x = Mathf.Cos(angle) * range;
            float z = Mathf.Sin(angle) * range;
            lineRenderer.SetPosition(i, new Vector3(x, 0, z));
        }
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
        
        if (Time.time >= nextFireTime)
        {
            FireShotgun();
            nextFireTime = Time.time + fireRate;
        }
    }

    void FireShotgun()
    {   
        if (bulletPrefab == null) return;

        float startAngle = -spreadAngle / 2f;
        float angleStep = spreadAngle / (bulletCount - 1);

        for (int i = 0; i < bulletCount; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            Quaternion rotation = transform.rotation * Quaternion.Euler(0, currentAngle, 0);
            
            VisualBullet tempBullet = Instantiate(bulletPrefab, transform.position, rotation);
            tempBullet.transform.parent = null; 
            tempBullet.gameObject.SetActive(true);
            tempBullet.maxDistance = range; 
        }
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