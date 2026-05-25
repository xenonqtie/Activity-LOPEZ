using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 7f;
    public bool hasWon = false;

    [Header("Health Pool")]
    public int currentHP = 20;

    void Update()
    {
        if (hasWon) return;

        float x = 0;
        float z = 0;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) z = 1;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) z = -1;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) x = -1;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) x = 1;
        }

        Vector3 moveDir = new Vector3(x, 0, z).normalized;
        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
    }
    public void TakeDamage(int amount)
    {
        if (hasWon) return;

        currentHP -= amount;
        Debug.Log($"[PLAYER] Hit by creature! Remaining HP: {currentHP}/20");

        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("[PLAYER] HP hit 0.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}