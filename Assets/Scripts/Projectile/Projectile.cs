using System.Runtime.CompilerServices;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private MonsterMove monsterMove;
    private Transform target;
    private int damage;

    public void Setup(Transform target, int damage)
    {
        monsterMove = GetComponent<MonsterMove>();
        this.target = target;
        this.damage = damage;
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

        collision.GetComponent<EnemyHp>().TakeDamage(damage);
        Destroy(gameObject);
    }
}
