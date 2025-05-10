using System.Collections;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    /// <summary>
    /// �̵� ��� ����
    /// </summary>
    private int         wayCount;

    /// <summary>
    /// �̵� ���
    /// </summary>
    private Transform[] wayPoints;

    /// <summary>
    /// ���� ��ǥ���� �ε���
    /// </summary>
    private int         currentIndex = 0;

    /// <summary>
    /// ������Ʈ �̵� ����
    /// </summary>
    private MonsterMove monsterMove;

    public void SetUp(Transform[] wayPoints)
    {
        monsterMove = GetComponent<MonsterMove>();

        // �� �̵���� waypoints ���� ����
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
