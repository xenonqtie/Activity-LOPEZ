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
    {   // Draws a "V" shape cone on the ground to show the range
        lineRenderer.positionCount = 3;
        Vector3 leftEdge = Quaternion.Euler(0, -coneAngle, 0) * Vector3.forward * range;
        Vector3 rightEdge = Quaternion.Euler(0, coneAngle, 0) * Vector3.forward * range;
        
        lineRenderer.SetPosition(0, leftEdge);
        lineRenderer.SetPosition(1, Vector3.zero);
        lineRenderer.SetPosition(2, rightEdge);
    }

    protected override void Attack()
    {
        if (!isActive || player == null) return;

        if (PlayerRange())
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            dirToPlayer.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(dirToPlayer); // rotate the turret toward the player's position
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            float angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);

            if (angleToPlayer < coneAngle)
            {
                if (Time.time >= nextFireTime)
                {
                    FireFlame();
                    nextFireTime = Time.time + fireRate;
                }
            }
        }
    }

    void FireFlame()
    {
        float randomSpread = Random.Range(-5f, 5f);
        Quaternion bulletRotation = transform.rotation * Quaternion.Euler(0, randomSpread, 0);

        VisualBullet tempBullet = Instantiate(bulletPrefab, transform.position, bulletRotation);
        tempBullet.gameObject.SetActive(true);
        
        tempBullet.speed = 20f; 
        Destroy(tempBullet.gameObject, 1.5f); 
    }

    void Update() => Attack();
}