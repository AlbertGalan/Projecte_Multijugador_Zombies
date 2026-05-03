using UnityEngine;
using TMPro;
using Photon.Pun;
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
   // [SerializeField] private TMP_Text roundText;

    [Header("Efectes de Dany (Camera Shake)")]
    public Transform playerCameraTransform;
    private float shakeTime = 1f; 
    private float shakeDuration = 0.5f;
    private Quaternion cameraInitialLocalRotation;

    [Header("Efectes de Dany (Hit Panel)")]
    public CanvasGroup hitPanel; 

    private float health;
    private int score;
    private bool isDead;

    public int CurrentScore => score;

    public GameObject activeWeapon; // Referència a l'arma actual del jugador

    public PhotonView photonView;
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
		if(PhotonNetwork.InRoom && !photonView.IsMine)
		{
            playerCameraTransform.gameObject.SetActive(false); // Desactiva la càmera dels altres jugadors
			return;
		}
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
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("PlayerTakeDamage", RpcTarget.All, damage, photonView.ViewID);
        }
        else
        {
            PlayerTakeDamage(damage, photonView.ViewID);
        }
    }

    [PunRPC]
    public void PlayerTakeDamage(float damage, int viewID)
    {
        if (photonView.ViewID == viewID)
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

        if (PhotonNetwork.InRoom && photonView.IsMine)
        {
            photonView.RPC("RPC_AddScore", RpcTarget.All, amount);
        }
        else if (!PhotonNetwork.InRoom)
        {
            score += amount;
            UpdateHUD();
        }
    }

    [PunRPC]
    public void RPC_AddScore(int amount)
    {
        if (isDead) return;
        score += amount;
        UpdateHUD();
    }

    public bool TrySpendScore(int amount)
    {
        if (amount <= 0 || isDead || score < amount) return false;

        if (PhotonNetwork.InRoom && photonView.IsMine)
        {
            photonView.RPC("RPC_SpendScore", RpcTarget.All, amount);
        }
        else if (!PhotonNetwork.InRoom)
        {
            score -= amount;
            UpdateHUD();
            return true;
        }
        return true;
    }

    [PunRPC]
    public void RPC_SpendScore(int amount)
    {
        if (score >= amount)
        {
            score -= amount;
            UpdateHUD();
        }
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
      /*  if (roundText != null && gameManager != null)
            roundText.text = "Ronda: " + gameManager.round.ToString();
    
    */}

    [PunRPC]
    public void WeaponShootSFX(int viewID)
    {
      activeWeapon.GetComponent<WeaponManager>().ShootVFX(viewID);
    }
}