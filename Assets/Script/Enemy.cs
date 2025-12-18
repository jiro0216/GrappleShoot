using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemySpawner spawner;

    public void Initialize(EnemySpawner spawnerRef)
    {
        spawner = spawnerRef;


    }

    void OnDestroy()
    {
        if (spawner != null)
        {
            Debug.Log("Enemy destroyed");
            spawner.OnEnemyKilled();
        }
    }

}
