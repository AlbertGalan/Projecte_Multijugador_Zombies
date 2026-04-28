using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    public GameObject playerCam; // Fa referencia a la camera del jugador FPS
    public float range = 100f; // Fins on volem que arribin els tirs
    public float damage = 25f;

    private PlayerManager playerManager;

    private void Start()
    {
        playerManager = GetComponentInParent<PlayerManager>();
        if (playerManager == null)
        {
            playerManager = FindAnyObjectByType<PlayerManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("Fire1 pressed");
            Shoot();
        }
    }

    void Shoot()
    {
        if (playerCam == null)
        {
            Debug.LogWarning("WeaponManager: playerCam no assignada.");
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, transform.forward, out hit, range))
        {
            EnemyManager enemyManager = hit.transform.GetComponent<EnemyManager>();
            if (enemyManager != null)
            {
                bool enemyKilled = enemyManager.Hit(damage);
                if (enemyKilled && playerManager != null)
                {
                    playerManager.AddScore(enemyManager.ScoreValue);
                }
            }
        }
    }
}
