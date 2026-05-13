using UnityEngine;

public class GoalPost : MonoBehaviour
{
    public Transform player;
    public GameObject winUI;
    public float winDistance=2f;

    void Update()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) < winDistance)
        {
            Win();
        }
    }

 void Win()
    {
        winUI.SetActive(true);
        player.GetComponent<PlayerMovement>().hasWon = true;
        
        BaseTurret[] allTurrets = Object.FindObjectsByType<BaseTurret>(FindObjectsSortMode.None);
        foreach (BaseTurret t in allTurrets) t.Deactivate();
    }
}
