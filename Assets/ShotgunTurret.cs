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
    {   // Draws a 360-degree circle on the floor to show the turret's reach
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
        if (!isActive || player == null) return; // Stop if turret is disabled or player is missing

        if (PlayerRange())
        {
            //direction to player
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            dirToPlayer.y = 0; 

            //rotation
            Quaternion lookRotation = Quaternion.LookRotation(dirToPlayer);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            if (Time.time >= nextFireTime)
            {
                FireShotgun();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void FireShotgun()
    {   // Calculate the starting angle for the spread (the far left bullet)
        float startAngle = -spreadAngle / 2f;
        float angleStep = spreadAngle / (bulletCount - 1);

        for (int i = 0; i < bulletCount; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            Quaternion rotation = transform.rotation * Quaternion.Euler(0, currentAngle, 0);
            
            VisualBullet tempBullet = Instantiate(bulletPrefab, transform.position, rotation);
            tempBullet.gameObject.SetActive(true);
        }
    }

    void Update() => Attack();
}