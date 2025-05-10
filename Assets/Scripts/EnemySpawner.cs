using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private float spawnTime;
    [SerializeField]
    private Transform[] wayPoints;

    private void Awake()
    {
        StartCoroutine(spawnEnemy());
    }

    private IEnumerator spawnEnemy()
    {
        while (true)
        {
            GameObject clone = Instantiate(enemyPrefab,Vector3.zero, Quaternion.identity);
            Enemy enemy = clone.GetComponent<Enemy>();

            enemy.SetUp(wayPoints);

            yield return new WaitForSeconds(spawnTime);
        }
    }
}
