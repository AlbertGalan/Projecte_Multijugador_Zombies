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

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (enemyAnimator == null)
        {
            enemyAnimator = GetComponentInChildren<Animator>();
        }
    }

    void Update()
    {
        if (isDead || agent == null || player == null) return;

        if (playerInReach)
        {
            agent.ResetPath();
            if (enemyAnimator != null) enemyAnimator.SetBool("isRunning", false);

            // LÓGICA DE ATAQUE POR GOLPES
            if (Time.time >= nextAttackTime)
            {
                AttackPlayer();
                nextAttackTime = Time.time + attackCooldown;
            }
            return;
        }

        // Movimiento hacia el jugador
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

        // Aplicamos el daño directo (20 de vida)
        if (playerManager != null)
        {
            playerManager.Hit(damage); 
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

    public bool Hit(float damage)
    {
        if (isDead) return false;
        health -= damage;
        if (health <= 0f)
        {
            Die();
            return true;
        }
        return false;
    }

    private void Die()
    {
        isDead = true;
        agent.isStopped = true;
        if (enemyAnimator != null) enemyAnimator.SetTrigger("isDead");
        if (gameManager != null) gameManager.enemiesAlive--;
        Destroy(gameObject, 3f);
    }
}