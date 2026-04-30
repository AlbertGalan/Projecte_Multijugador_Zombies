using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int enemiesAlive;
    public int round;

    public GameObject[] spawnPoints;
    public GameObject enemyPrefab;

    void Start()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawners");
    }
    // Update is called once per frame
    void Update()
    {
        if (enemiesAlive <= 0)
        {
            round++;
            NextWave(round);
        }
    }

    void NextWave(int roundNumber)
    {
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("GameManager: Falta assignar enemyPrefab o spawnPoints.");
            return;
        }

        for (int i = 0; i < roundNumber; i++)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            GameObject spawnPoint = spawnPoints[randomIndex];

            if (spawnPoint == null)
            {
                continue;
            }

            GameObject enemyInstance = Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
            EnemyManager enemyManager = enemyInstance.GetComponent<EnemyManager>();
            if (enemyManager != null)
            {
                enemyManager.gameManager = this;
            }

            enemiesAlive++;
        }
    }
}
