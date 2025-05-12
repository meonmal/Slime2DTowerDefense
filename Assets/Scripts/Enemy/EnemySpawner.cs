using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private float spawnTime;
    [SerializeField]
    private Transform[] wayPoints;
    private List<Enemy> enemyList;

    public List<Enemy> EnemyList => enemyList;

    private void Awake()
    {
        enemyList = new List<Enemy>();

        StartCoroutine(spawnEnemy());
    }

    private IEnumerator spawnEnemy()
    {
        while (true)
        {
            GameObject clone = Instantiate(enemyPrefab,Vector3.zero, Quaternion.identity);
            Enemy enemy = clone.GetComponent<Enemy>();

            enemy.SetUp(this, wayPoints);
            enemyList.Add(enemy);

            yield return new WaitForSeconds(spawnTime);
        }
    }

    public void DestroyEnemy(Enemy enemy)
    {
        enemyList.Remove(enemy);

        Destroy(enemy.gameObject);
    }
}
