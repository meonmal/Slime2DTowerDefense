using System.Collections;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    /// <summary>
    /// 이동 경로 개수
    /// </summary>
    private int         wayCount;

    /// <summary>
    /// 이동 경로
    /// </summary>
    private Transform[] wayPoints;

    /// <summary>
    /// 현재 목표지점 인덱스
    /// </summary>
    private int         currentIndex = 0;

    /// <summary>
    /// 오브젝트 이동 제어
    /// </summary>
    private MonsterMove monsterMove;

    public void SetUp(Transform[] wayPoints)
    {
        monsterMove = GetComponent<MonsterMove>();

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
            Destroy(gameObject);
        }
    }
}
