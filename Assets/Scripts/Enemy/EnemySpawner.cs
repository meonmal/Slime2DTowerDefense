using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private GameObject enemyHpSliderPrefab;
    [SerializeField]
    private Transform canvasTransform;
    [SerializeField]
    private float spawnTime;
    [SerializeField]
    private Transform[] wayPoints;
    [SerializeField]
    private PlayerHp playerHp;
    [SerializeField]
    private PlayerGold playerGold;
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

            SpawnerEnemyHpSlider(clone);

            yield return new WaitForSeconds(spawnTime);
        }
    }

    public void DestroyEnemy(EnemyDestroyType type, Enemy enemy, int gold)
    {
        if(type == EnemyDestroyType.Arrive)
        {
            playerHp.TakeDamage(1);
        }
        else if(type == EnemyDestroyType.Kill)
        {
            playerGold.CurrentGold += gold;
        }

        enemyList.Remove(enemy);

        Destroy(enemy.gameObject);
    }

    private void SpawnerEnemyHpSlider(GameObject enemy)
    {
        GameObject sliderClone = Instantiate(enemyHpSliderPrefab);
        sliderClone.transform.SetParent(canvasTransform);
        sliderClone.transform.localScale = Vector3.one;

        sliderClone.GetComponent<SliderPositionAutoSetter>().Setup(enemy.transform);
        sliderClone.GetComponent<EnemyHpViewer>().Setup(enemy.GetComponent<EnemyHp>());
    }
}
