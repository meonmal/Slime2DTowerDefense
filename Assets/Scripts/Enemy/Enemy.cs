using System.Collections;
using UnityEditor;
using UnityEngine;

public enum EnemyDestroyType { Kill = 0, Arrive }

public class Enemy : MonoBehaviour
{
    private int         wayCount;
    private Transform[] wayPoints;
    private int         currentIndex = 0;
    private MonsterMove monsterMove;
    private EnemySpawner enemySpawner;
    [SerializeField]
    private int gold = 10;

    public void SetUp(EnemySpawner enemySpawner, Transform[] wayPoints)
    {
        monsterMove = GetComponent<MonsterMove>();
        this.enemySpawner = enemySpawner;

        // 적 이동경로 waypoints 정보 설정
        wayCount = wayPoints.Length;
        this.wayPoints = new Transform[wayCount];
        this.wayPoints = wayPoints;

        transform.position = wayPoints[currentIndex].position;

        StartCoroutine(OnMove());
    }

    private IEnumerator OnMove()
    {
        NextMoveTo();

        while (true)
        {
            if(Vector3.Distance(transform.position, wayPoints[currentIndex].position) < 0.02f * monsterMove.MoveSpeed)
            {
                NextMoveTo();
            }

            yield return null;
        }
    }

    private void NextMoveTo()
    {
        if(currentIndex < wayCount - 1)
        {
            transform.position = wayPoints[currentIndex].position;

            currentIndex++;
            Vector3 direction = (wayPoints[currentIndex].position - transform.position).normalized;
            monsterMove.MoveTo(direction); 
        }
        else
        {
            gold = 0;
            OnDie(EnemyDestroyType.Arrive);
        }
    }

    public void OnDie(EnemyDestroyType type)
    {
        enemySpawner.DestroyEnemy(type,this, gold);
    }
}
