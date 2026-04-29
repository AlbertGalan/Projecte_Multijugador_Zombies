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

    [Header("Efectes de Dany (Camera Shake)")]
    public Transform playerCameraTransform; // Assigna aquí la Càmera (filla del pivote)
    private float shakeTime = 1f; 
    private float shakeDuration = 0.5f;
    private Quaternion cameraInitialLocalRotation;

    [Header("Efectes de Dany (Hit Panel)")]
    public CanvasGroup hitPanel; 

    private float health;
    private int score;
    private bool isDead;

    public int CurrentScore => score;

    void Start()
    {
        health = maxHealth;

        if (playerCameraTransform != null)
        {
            cameraInitialLocalRotation = playerCameraTransform.localRotation;
        }

        if (hitPanel != null)
        {
            hitPanel.alpha = 0f;
            hitPanel.interactable = false;
            hitPanel.blocksRaycasts = false;
        }

        if (gameMenuManager == null) gameMenuManager = FindAnyObjectByType<GameMenuManager>();
        if (gameManager == null) gameManager = FindAnyObjectByType<GameManager>();

        UpdateHUD();
    }

    void Update()
    {
        // Lògica del Camera Shake (Independint del MouseLook)
        if (!isDead && shakeTime < shakeDuration)
        {
            shakeTime += Time.deltaTime;
            CameraShake();
        }
        else if (playerCameraTransform != null && playerCameraTransform.localRotation != cameraInitialLocalRotation)
        {
            playerCameraTransform.localRotation = cameraInitialLocalRotation;
        }

        // Lògica del Fade Out del Hit Panel
        if (hitPanel != null)
        {
            if (hitPanel.alpha > 0f)
            {
                hitPanel.alpha -= Time.unscaledDeltaTime;
            }

            if (hitPanel.alpha <= 0f)
            {
                hitPanel.alpha = 0f;
                hitPanel.interactable = false;
                hitPanel.blocksRaycasts = false;
            }
        }

        UpdateHUD();
    }

    public void Hit(float damage)
    {
        if (isDead) return;

        health -= damage;
        shakeTime = 0f; // Dispara el vibrat a l'Update

        if (hitPanel != null)
        {
            hitPanel.alpha = 1f;
            hitPanel.interactable = true;
            hitPanel.blocksRaycasts = true;
        }

        if (health <= 0f)
        {
            health = 0f;
            isDead = true;
            if (gameMenuManager != null) gameMenuManager.ShowGameOver();
        }

        UpdateHUD();
    }

    public void CameraShake()
    {
        if (playerCameraTransform != null)
        {
            playerCameraTransform.localRotation = Quaternion.Euler(Random.Range(-2f, 2f), 0, 0);
        }
    }

    // --- Mètodes de Score i HUD ---
    public void AddScore(int amount)
    {
        if (amount <= 0 || isDead) return;
        score += amount;
        UpdateHUD();
    }

    public bool TrySpendScore(int amount)
    {
        if (amount <= 0 || isDead || score < amount) return false;
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
            healthText.text = "Salut: " + $"{currentHealth}/{maxHealthInt}";
        }
        if (scoreText != null) scoreText.text = "Puntuació: " + score.ToString();
        if (roundText != null && gameManager != null)
            roundText.text = "Ronda: " + gameManager.round.ToString();
    }
}