using UnityEngine;

[CreateAssetMenu]
public class TowerTemplate : ScriptableObject
{
    public GameObject towerPrefab;
    public GameObject followTowerPrefab;
    public Weapon[] weapon;
    public Sprite towerSprite;

    [System.Serializable]
    public struct Weapon
    {
        public float damage;
        public float slow;
        public float buff;
        public float rate;
        public float range;
        public int cost;
        public int sell;
    }
}
