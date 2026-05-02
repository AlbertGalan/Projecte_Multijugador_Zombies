using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    public GameObject player;
    public GameManager gameManager;
    public Animator enemyAnimator;
    public float damage = 20f; // Ahora quitará 20 por golpe

    public float health = 100f;
    [SerializeField] private int scoreValue = 100;

    [SerializeField] private float runSpeedThreshold = 0.1f;
    [SerializeField] private float destinationSampleRadius = 2f;
    public bool playerInReach;

    private NavMeshAgent agent;
    private PlayerManager playerManager;
    private bool isDead;
    
    [Header("Ajustes de Ataque")]
    public float attackCooldown = 1.5f; // Tiempo entre ataques
    private float nextAttackTime;

    public int ScoreValue => scoreValue;

    public PhotonView photonView;

    private GameObject[] playersInScene;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playersInScene = GameObject.FindGameObjectsWithTag("Player");

        if (enemyAnimator == null)
        {
            enemyAnimator = GetComponentInChildren<Animator>();
        }
    }

void Update()
{
    // IMPORTANTE: Solo el MasterClient debe mover al zombie para evitar desincronización
    if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient) return;

    if (isDead || agent == null) return;

    // Actualizamos la lista de jugadores dinámicamente si está vacía o cada poco tiempo
    playersInScene = GameObject.FindGameObjectsWithTag("Player");

    GetClosestPlayer();

    if (player == null) return; 

    if (playerInReach)
    {
        agent.isStopped = true; // Mejor que ResetPath
        if (enemyAnimator != null) enemyAnimator.SetBool("isRunning", false);

        if (Time.time >= nextAttackTime)
        {
            AttackPlayer();
            nextAttackTime = Time.time + attackCooldown;
        }
        return;
    }

    // Movimiento
    agent.isStopped = false;
    Vector3 targetPosition = player.transform.position;
    if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, destinationSampleRadius, NavMesh.AllAreas))
    {
        agent.SetDestination(hit.position);
    }

    if (enemyAnimator != null)
    {
        enemyAnimator.SetBool("isRunning", agent.velocity.magnitude > runSpeedThreshold);
    }
}

    // Nuevo método para atacar
    void AttackPlayer()
    {
        if (enemyAnimator != null)
        {
            enemyAnimator.SetTrigger("isAttacking");
        }

        // Sincronizamos el ataque a través de la red
        if (player != null)
        {
            if (PhotonNetwork.InRoom)
            {
                photonView.RPC("DealDamageToPlayer", RpcTarget.All, player.GetComponent<PhotonView>().ViewID, damage);
            }
            else
            {
                if (playerManager != null)
                {
                    playerManager.Hit(damage);
                }
            }
            Debug.Log("¡El zombie ha golpeado al jugador! Daño: " + damage);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInReach = true;
            if (playerManager == null)
            {
                playerManager = collision.gameObject.GetComponent<PlayerManager>();
            }
        }
    }

    // Eliminamos OnCollisionStay para que no quite vida por "rozar" al jugador
    
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInReach = false;
            playerManager = null;
        }
    }

// Modifica el método Hit para recibir el ID del atacante
public void Hit(float dmg, int shooterID)
{
    if (isDead) return;

    if (PhotonNetwork.InRoom)
    {
        // Enviamos el daño y quién disparó a todos
        photonView.RPC("TakeDamage", RpcTarget.All, dmg, shooterID);
    }
    else
    {
        ApplyDamage(dmg, shooterID);
    }
}

[PunRPC]
public void TakeDamage(float dmg, int shooterID)
{
    ApplyDamage(dmg, shooterID);
}

private void ApplyDamage(float dmg, int shooterID)
{
    if (isDead) return;
    health -= dmg;

    if (health <= 0)
    {
        // Buscamos al jugador que disparó mediante su ViewID para darle puntos
        PhotonView shooterPV = PhotonView.Find(shooterID);
        if (shooterPV != null)
        {
            PlayerManager pm = shooterPV.GetComponent<PlayerManager>();
            if (pm != null) pm.AddScore(scoreValue);
        }

        Die();
    }
}
    private void Die()
    {
        isDead = true;
        agent.isStopped = true;
        if (enemyAnimator != null) enemyAnimator.SetTrigger("isDead");
        if (gameManager != null)
        {
            gameManager.enemiesAlive--;

            // Sincronizar enemiesAlive en multijugador (sólo MasterClient debe publicar)
            if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
            {
                Hashtable hash = new Hashtable();
                hash["enemiesAlive"] = gameManager.enemiesAlive;
                PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
            }
        }
        Destroy(gameObject, 3f);
    }

    private void GetClosestPlayer()
    {
        GameObject closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject playerObj in playersInScene)
        {
            float distance = Vector3.Distance(transform.position, playerObj.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = playerObj;
            }
        }

        player = closestPlayer;
    }

    [PunRPC]
    public void DealDamageToPlayer(int playerViewID, float dmg)
    {
        PhotonView playerPV = PhotonView.Find(playerViewID);
        if (playerPV != null)
        {
            PlayerManager pm = playerPV.GetComponent<PlayerManager>();
            if (pm != null)
            {
                pm.PlayerTakeDamage(dmg, playerViewID);
            }
        }
    }
}