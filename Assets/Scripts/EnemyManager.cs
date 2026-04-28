using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    public GameObject player;
    public GameManager gameManager;
    public Animator enemyAnimator;
    public float damage = 20f;

    public float health = 100f;
    [SerializeField] private int scoreValue = 100;

    [SerializeField] private float runSpeedThreshold = 0.1f;
    [SerializeField] private float destinationSampleRadius = 2f;

    private NavMeshAgent agent;
    private bool isDead;

    public int ScoreValue => scoreValue;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (enemyAnimator == null)
        {
            // Root sol tenir l'Animator en molts prefabs de zombie.
            enemyAnimator = GetComponentInChildren<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (agent == null || player == null)
        {
            return;
        }

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

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            PlayerManager playerManager = collision.gameObject.GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                playerManager.Hit(damage);
            }
        }
    }

    // Salut de l'enemic
    public bool Hit(float damage)
    {
        if (isDead)
        {
            return false;
        }

        health -= damage;

        if (health <= 0f)
        {
            isDead = true;

            if (gameManager != null)
            {
                gameManager.enemiesAlive--;
            }

            // Destrium l'enemic quan la seva salut arriba a zero.
            Destroy(gameObject);
            return true;
        }

        return false;
    }

}
