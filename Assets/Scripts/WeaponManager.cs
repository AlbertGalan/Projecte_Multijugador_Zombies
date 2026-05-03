using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    public GameObject playerCam; 
    public float range = 100f; // Fins on volem que arribin els tirs
    public float damage = 25f;

    [SerializeField] private Animator weaponAnimator;
    [SerializeField] private float shootAnimationDuration = 0.1f;
    [SerializeField] private AudioSource shootAudioSource;
    [SerializeField] private AudioClip shootSfx;
    [SerializeField] private GameObject shootEffect;

    public PhotonView photonView;

    public GameManager gameManager;
    private PlayerManager playerManager;
    

    private void Start()
    {
        playerManager = GetComponentInParent<PlayerManager>();
        if (playerManager == null)
        {
            playerManager = FindAnyObjectByType<PlayerManager>();
        }

        if (weaponAnimator == null)
        {
            weaponAnimator = GetComponentInChildren<Animator>();
        }

        if (shootAudioSource == null)
        {
            shootAudioSource = GetComponent<AudioSource>();
        }

        if (shootEffect != null)
        {
            ParticleSystem particleSystem = shootEffect.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                ParticleSystem.MainModule main = particleSystem.main;
                main.stopAction = ParticleSystemStopAction.None;
            }

            CFX_AutoDestructShuriken autoDestruct = shootEffect.GetComponent<CFX_AutoDestructShuriken>();
            if (autoDestruct != null)
            {
                autoDestruct.OnlyDeactivate = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
	{
		if(PhotonNetwork.InRoom && !photonView.IsMine)
		{
			return;
		}
        
        if(!gameManager.isPaused && !gameManager.isGameOver)
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
    }

    void Shoot()
    {
        if(PhotonNetwork.InRoom)
        {
            photonView.RPC("WeaponShootSFX", RpcTarget.All, photonView.ViewID);
        }
        else
        {
            ShootVFX(photonView.ViewID);
        }
        if (playerCam == null)
        {
            Debug.LogWarning("WeaponManager: playerCam no assignada.");
            return;
        }

        if (weaponAnimator != null)
        {
            weaponAnimator.SetBool("isShooting", true);
            CancelInvoke(nameof(ResetShootingAnimation));
            Invoke(nameof(ResetShootingAnimation), shootAnimationDuration);
        }
        else
        {
            Debug.LogWarning("WeaponManager: weaponAnimator no assignat.");
        }

        RaycastHit hit;
if (Physics.Raycast(playerCam.transform.position, transform.forward, out hit, range))
{
    EnemyManager enemy = hit.transform.GetComponent<EnemyManager>();
    if (enemy != null)
    {
        // Pasam el mal i el viewId del nostre jugador
        enemy.Hit(damage, playerManager.photonView.ViewID);
    }
}

        
    }

    public void ShootVFX(int viewID)
    {
        if(photonView.ViewID == viewID)
        {
            if (shootAudioSource != null)
        {
            if (shootSfx != null)
            {
                shootAudioSource.PlayOneShot(shootSfx);
            }
            else
            {
                shootAudioSource.Play();
            }
        }

        if (shootEffect != null)
        {
            shootEffect.SetActive(true);

            ParticleSystem particleSystem = shootEffect.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                particleSystem.Play();
            }
        }
        }
    }

    private void ResetShootingAnimation()
    {
        if (weaponAnimator != null)
        {
            weaponAnimator.SetBool("isShooting", false);
        }
    }
    
}
