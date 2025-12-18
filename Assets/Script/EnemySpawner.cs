using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.PlayerLoop;
using Unity.VisualScripting;
public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]

    public GameObject spawnerObject;
    public GameObject enemyPrefab;
    public int enemiesPerWave = 5;
    public Vector2 mapSize = new Vector2(20, 20); // width and height of your map

    public int enemiesAlive = 0;

    public float timeBetweenSpawns = 1f;

    [SerializeField] TextMeshProUGUI enemiesRemainingText;


    void Start()
    {
        StartNextWave();
    }

    public void StartNextWave()
    {

        if (enemiesAlive == 0)
        {
            enemiesPerWave += 3;
            Debug.Log("Starting next wave. Enemies this wave: " + enemiesPerWave);
            StartCoroutine(SpawnEnemies());
            enemiesRemainingText.SetText("Enemies Remaining: {0}", enemiesPerWave);
        }

    }


    IEnumerator SpawnEnemies()
    {

        enemiesAlive = enemiesPerWave;


        for (int i = 0; i < enemiesPerWave; i++)
        {
            Vector3 spawnPos = GetRandomSpawnPosition();
            GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            // Get Enemy component and pass the spawner reference
            Enemy enemyScript = enemyObj.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.Initialize(this); // ⭐ Important
            }
            else
            {
                Debug.LogError("Enemy prefab missing Enemy script!");
            }
            yield return new WaitForSeconds(timeBetweenSpawns);

        }



    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;
        Debug.Log("Enemy killed! Remaining: " + enemiesAlive);
        enemiesRemainingText.SetText("Enemies Remaining: {0}", enemiesAlive);

        if (enemiesAlive <= 0)
        {
            Debug.Log("Wave cleared!");
            StartNextWave();
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        Vector2 offset = new Vector2(
            Random.Range(-mapSize.x / 2, mapSize.x / 2),
            Random.Range(-mapSize.y / 2, mapSize.y / 2)
        );

        return new Vector3(
            spawnerObject.transform.position.x + offset.x,
            spawnerObject.transform.position.y + offset.y,
            0
        );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(mapSize.x, mapSize.y, 0));
    }


}
