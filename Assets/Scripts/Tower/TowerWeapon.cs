using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum WeaponType { Cannon = 0, Laser, Slow, Buff, }
public enum WeaponState { SearchTarget = 0, TryAttackCannon, TryAttackLaser, }

public class TowerWeapon : MonoBehaviour
{
    [Header("Commons")]
    [SerializeField]
    private TowerTemplate towertemplate;
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private WeaponType weaponType;

    [Header("Cannon")]
    [SerializeField]
    private GameObject projectilePrefab;

    [Header("Laser")]
    [SerializeField]
    private LineRenderer lineRenderer;
    [SerializeField]
    private Transform hitEffect;
    [SerializeField]
    private LayerMask targetLayer;

    private int level = 0;
    private WeaponState weaponState = WeaponState.SearchTarget;
    private Transform attackTarget = null;
    private TowerSpawner towerSpawner;
    private EnemySpawner enemySpawner;
    private PlayerGold playerGold;
    private Tile owerTile;

    private float addedDamage;
    private int buffLevel;

    public Sprite TowerSprite => towertemplate.towerSprite;
    public float Damage => towertemplate.weapon[level].damage;
    public float Rate => towertemplate.weapon[level].rate;
    public float Range => towertemplate.weapon[level].range;
    public int UpgradeCost => level < MaxLevel ? towertemplate.weapon[level + 1].cost : 0;
    public int SellCost => towertemplate.weapon[level].sell;
    public int Level => level + 1;
    public int MaxLevel => towertemplate.weapon.Length;
    public float Slow => towertemplate.weapon[level].slow;
    public float Buff => towertemplate.weapon[level].buff;
    public WeaponType WeaponType => weaponType;
    public float AddedDamage
    {
        set => addedDamage = Mathf.Max(0, value);
        get => addedDamage;
    }
    public int BuffLevel
    {
        set => buffLevel = Mathf.Max(0, value);
        get => buffLevel;
    }

    public void Setup(TowerSpawner towerSpawner, EnemySpawner enemySpawner, PlayerGold playerGold, Tile owerTile)
    {
        this.towerSpawner = towerSpawner;
        this.enemySpawner = enemySpawner;
        this.playerGold = playerGold;
        this.owerTile = owerTile;

        if(weaponType == WeaponType.Cannon || weaponType == WeaponType.Laser)
        {
            ChangeState(WeaponState.SearchTarget);
        }
    }

    public void ChangeState(WeaponState newState)
    {
        StopCoroutine(weaponState.ToString());
        weaponState = newState;
        StartCoroutine(weaponState.ToString());
    }

    private void Update()
    {
        if(attackTarget != null)
        {
            RotateToTarget();
        }
    }

    private void RotateToTarget()
    {
        float dx = attackTarget.position.x - transform.position.x;
        float dy = attackTarget.position.y - transform.position.y;

        float degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;

        if (degree > 90 || degree < -90)
        {
            transform.localScale = new Vector3(1, -1, 1); // 뒤집힌 거 방지
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        transform.rotation = Quaternion.Euler(0, 0, degree);
    }

    private IEnumerator SearchTarget()
    {
        while (true)
        {
            attackTarget = FindClosestAttackTarget();

            if(attackTarget != null)
            {
                if(weaponType == WeaponType.Cannon)
                {
                    ChangeState(WeaponState.TryAttackCannon);
                }
                else if(weaponType == WeaponType.Laser)
                {
                    ChangeState(WeaponState.TryAttackLaser);
                }
            }

            

            yield return null;
        }
    }

    private IEnumerator TryAttackCannon()
    {
        while (true)
        {
           if(IsPossibleToAttackTarget() == false)
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            yield return new WaitForSeconds(towertemplate.weapon[level].rate);

            SpawnProjectile();
        }
    }

    private IEnumerator TryAttackLaser()
    {
        EnableLaser();

        while (true)
        {
            if(IsPossibleToAttackTarget() == false)
            {
                DisableLaser();
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            SpawnLaser();

            yield return null;
        }
    }

    public void OnBuffAroundTower()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        for(int i = 0; i < towers.Length; i++)
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();

            if(weapon.BuffLevel > Level)
            {
                continue;
            }

            if(Vector3.Distance(weapon.transform.position, transform.position) <= towertemplate.weapon[level].range)
            {
                if(weapon.WeaponType == WeaponType.Cannon || weapon.WeaponType == WeaponType.Laser)
                {
                    weapon.AddedDamage = weapon.Damage * (towertemplate.weapon[level].buff);
                    weapon.BuffLevel = Level;
                }
            }
        }
    }



    private Transform FindClosestAttackTarget()
    {
        float closestDistSqr = Mathf.Infinity;

        for(int i =0; i < enemySpawner.EnemyList.Count; i++)
        {
            float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);

            if(distance <= towertemplate.weapon[level].range && distance <= closestDistSqr)
            {
                closestDistSqr = distance;
                attackTarget = enemySpawner.EnemyList[i].transform;
            }
        }

        return attackTarget;
    }

    private bool IsPossibleToAttackTarget()
    {
        if(attackTarget == null)
        {
            return false;
        }

        float distance = Vector3.Distance(attackTarget.position, transform.position);
        if(distance > towertemplate.weapon[level].range)
        {
            attackTarget = null;
            return false;
        }

        return true;
    }

    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        float damage = towertemplate.weapon[level].damage + AddedDamage;
        clone.GetComponent<Projectile>().Setup(attackTarget, towertemplate.weapon[level].damage);
    }

    private void EnableLaser()
    {
        lineRenderer.gameObject.SetActive(true);
        hitEffect.gameObject.SetActive(true);
    }

    private void DisableLaser()
    {
        lineRenderer.gameObject.SetActive(false);
        hitEffect.gameObject.SetActive(false);
    }

    private void SpawnLaser()
    {
        Vector3 direction = attackTarget.position - spawnPoint.position;
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, direction, towertemplate.weapon[level].range, targetLayer);

        for(int i=0; i<hit.Length; i++)
        {
            if(hit[i].transform == attackTarget)
            {
                lineRenderer.SetPosition(0, spawnPoint.position);
                lineRenderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);
                hitEffect.position = hit[i].point;
                float damage = towertemplate.weapon[level].damage + AddedDamage;
                attackTarget.GetComponent<EnemyHp>().TakeDamage(damage * Time.deltaTime);
            }
        }

    }

    public bool Upgrade()
    {
        if(playerGold.CurrentGold < towertemplate.weapon[level + 1].cost)
        {
            return false;
        }

        level++;
        playerGold.CurrentGold -= towertemplate.weapon[level].cost;

        if(weaponType == WeaponType.Laser)
        {
            lineRenderer.startWidth = 0.05f + level * 0.05f;
            lineRenderer.endWidth = 0.05f;
        }

        towerSpawner.OnBuffAllBuffTowers();

        return true;
    }

    public void Sell()
    {
        playerGold.CurrentGold += towertemplate.weapon[level].sell;
        owerTile.IsBuildTower = false;
        Destroy(gameObject);
    }
}
