using UnityEngine;

public class VisualBullet : MonoBehaviour
{
    public float speed = 40f;
    public float maxDistance = 30f;
    private Vector3 spawnPoint;
    void Start()
    {

        if (transform.parent != null) 
        {
            gameObject.SetActive(false);
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
        if (Vector3.Distance(spawnPoint, transform.position) > maxDistance)
        {
            HandleRemoval();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerScript = other.GetComponent<PlayerMovement>();
            
            if (playerScript != null)
            {
                Debug.Log("Bullet Hit Player!");
                playerScript.Die(); 
            }
            
            HandleRemoval();
        }
    }

    void HandleRemoval() //Dhidesthe bullet or delete it entirely
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