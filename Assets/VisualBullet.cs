using UnityEngine;

public class VisualBullet : MonoBehaviour
{
    public float speed = 40f;
    public float maxDistance = 30f;
    
    [Header("Pure Transform Hit Detection")]
    public float creatureKillDistance = 0.6f;
    public float playerKillDistance = 0.5f; 
    private Vector3 spawnPoint;
    private Transform playerTransform;

    void Start()
    {
        if (transform.parent != null) 
        {
            gameObject.SetActive(false);
        }
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    void OnEnable()
    {
        spawnPoint = transform.position;
        if (transform.parent != null)
        {
            transform.localPosition = Vector3.zero;
        }
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        CheckTransformCollisions();
        if (Vector3.Distance(spawnPoint, transform.position) > maxDistance)
        {
            HandleRemoval();
        }
    }

    private void CheckTransformCollisions()
    {
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= playerKillDistance)
            {
                PlayerMovement playerScript = playerTransform.GetComponent<PlayerMovement>();
                if (playerScript != null)
                {
                    Debug.Log("[TURRET] Bullet scored a direct hit on the Player!.");
                    playerScript.Die();
                }
                HandleRemoval();
                return; 
            }
        }
        CreatureMovement[] activeCreatures = FindObjectsByType<CreatureMovement>(FindObjectsSortMode.None);
        foreach (CreatureMovement creature in activeCreatures)
        {
            if (creature == null) continue;

            float distanceToCreature = Vector3.Distance(transform.position, creature.transform.position);
            if (distanceToCreature <= creatureKillDistance)
            {
                Debug.Log($"[TURRET] Bullet destroyed creature: {creature.gameObject.name} in one hit!");
                
                Destroy(creature.gameObject);
                HandleRemoval();
                break; 
            }
        }
    }

    void HandleRemoval() 
    {
        if (transform.parent != null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}