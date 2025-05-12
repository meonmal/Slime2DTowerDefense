using System.Runtime.CompilerServices;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private MonsterMove monsterMove;
    private Transform target;

    public void Setup(Transform target)
    {
        monsterMove = GetComponent<MonsterMove>();
        this.target = target;
    }

    private void Update()
    {
        if(target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            monsterMove.MoveTo(direction);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
        {
            return;
        }
        if(collision.transform != target)
        {
            return;
        }

        collision.GetComponent<Enemy>().OnDie();
        Destroy(gameObject);
    }
}
