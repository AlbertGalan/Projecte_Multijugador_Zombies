using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 100f;

    [Header("References")]
    [SerializeField] private GameMenuManager gameMenuManager;
    [SerializeField] private GameManager gameManager;

    [Header("HUD")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text roundText;

    private float health;
    private int score;

    private bool isDead;

    public int CurrentScore => score;


    void Start()
    {
        health = maxHealth;

        if (gameMenuManager == null)
        {
            gameMenuManager = FindAnyObjectByType<GameMenuManager>();
        }

        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }

        UpdateHUD();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHUD();
    }

    public void Hit(float damage)
    {
        if (isDead)
        {
            return;
        }

        health -= damage;

        if (health <= 0f)
        {
            health = 0f;
            isDead = true;

            if (gameMenuManager != null)
            {
                gameMenuManager.ShowGameOver();
            }
        }

        UpdateHUD();
    }

    public void AddScore(int amount)
    {
        if (amount <= 0 || isDead)
        {
            return;
        }

        score += amount;
        UpdateHUD();
    }

    public bool TrySpendScore(int amount)
    {
        if (amount <= 0 || isDead)
        {
            return false;
        }

        if (score < amount)
        {
            return false;
        }

        score -= amount;
        UpdateHUD();
        return true;
    }

    private void UpdateHUD()
    {
        if (healthText != null)
        {
            int currentHealth = Mathf.CeilToInt(health);
            int maxHealthInt = Mathf.CeilToInt(maxHealth);
            healthText.text ="Salut: " + $"{currentHealth}/{maxHealthInt}";
        }

        if (scoreText != null)
        {
            scoreText.text = "Puntuació: " + score.ToString();
        }

        if (roundText != null && gameManager != null)
        {
            roundText.text = "Ronda:" + gameManager.round.ToString();
        }
    }
}
