using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TMP_Text scoreText; // Reference to the score text
    public Slider dashCooldownSlider; // Reference to the dash cooldown UI

    private int score = 0;
    private float lastDashTime = -999f; // Tracks last dash time
    private float dashCooldown = 1f;    // Default cooldown value

    // a failsafe to ensure that only one instance of GameManager exists in the scene
    private void Awake()
    {
        // Ensure there's only one instance of GameManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        UpdateDashCooldownUI();
    }

    public void AddScore(int amount)
    {
        score += amount; // Increase score
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString();
    }

    public void StartDashCooldown(float cooldownDuration)
    {
        dashCooldown = cooldownDuration;
        lastDashTime = Time.time; // Set last dash time
    }

    private void UpdateDashCooldownUI()
    {
        if (dashCooldownSlider != null)
        {
            float cooldownProgress = Mathf.Clamp01((Time.time - lastDashTime) / dashCooldown);
            dashCooldownSlider.value = cooldownProgress;
        }
    }
}
