using UnityEngine;

public class SniperTurrent : BaseTurret 
{
    [Header("Rotation Settings")]
    public float searchSpeed = 50f;     
    public float lockOnSpeed = 2f;      
    
    [Header("Firing Settings")]
    public float shootThreshold = 0.80f; 
    public VisualBullet visualBullet;    

    protected override void Start()
    {
        base.Start();
        if(visualBullet != null) visualBullet.gameObject.SetActive(false);
    }

    protected override void DrawRange()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.forward * range);
    }

    protected override void Attack()
    {
        if (!isActive || player == null) return;

        if (PlayerRange())
        {

            Vector3 dirToPlayer = (player.position - transform.position).normalized; // Draws a single straight line forward 
            dirToPlayer.y = 0; 
            Quaternion lookRotation = Quaternion.LookRotation(dirToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lockOnSpeed);

            float alignment = Vector3.Dot(transform.forward, dirToPlayer);

            if (alignment > shootThreshold)
            {
                if (visualBullet != null && !visualBullet.gameObject.activeSelf)
                {
                    FireAtPlayer();
                }
            }
        }
        else
        {
            transform.Rotate(Vector3.up * searchSpeed * Time.deltaTime);
            if(visualBullet != null) visualBullet.gameObject.SetActive(false);
        }
    }

    void FireAtPlayer()
    {
        if (visualBullet != null) 
        {
            visualBullet.gameObject.SetActive(true);
        }
    }

    void Update() => Attack();
}