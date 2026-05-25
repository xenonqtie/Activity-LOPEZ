using UnityEngine;
using UnityEngine.UI;

public class UIHealth : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerScript; 
    public Slider realSlider;        
    public Slider ghostSlider;        
    [Header("Ghost Effect Settings")]
    public float ghostDelay = 0.5f;     
    public float easeSpeed = 5f;       
    private float ghostTargetValue;
    private float delayTimer = 0f;

    void Start()
    {
        if (playerScript != null && realSlider != null && ghostSlider != null)
        {
            realSlider.maxValue = playerScript.currentHP;
            ghostSlider.maxValue = playerScript.currentHP;

            realSlider.value = playerScript.currentHP;
            ghostSlider.value = playerScript.currentHP;
            ghostTargetValue = playerScript.currentHP;
        }
    }

    void Update()
    {
        if (playerScript == null || realSlider == null || ghostSlider == null) return;

        float currentHP = playerScript.currentHP;

        realSlider.value = currentHP;
        if (currentHP < ghostTargetValue)
        {
            delayTimer = ghostDelay; 
            ghostTargetValue = currentHP;
        }
        else if (currentHP > ghostTargetValue)
        {
            ghostTargetValue = currentHP;
            ghostSlider.value = currentHP;
        }

        if (delayTimer > 0f)
        {
            delayTimer -= Time.deltaTime;
        }
        else
        {
            ghostSlider.value = Mathf.Lerp(ghostSlider.value, ghostTargetValue, easeSpeed * Time.deltaTime);
        }
    }
}