using ICKT.ServiceLocator;
using UnityEngine;

public class EnemyUnit : UnitBase, IDamageable
{
    public int SpawnIndex { get; protected set; }
    private Transform _Target;
    private Vector3 _MoveDirection;

    // TODO: implement better pathfinding

    public void Damage(int damageTaken, DamageSource source)
    {
        if (source == DamageSource.Friendly)
        {
            TakeDamage(damageTaken);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        OnUnitDied += OnDeath;
    }

    public void SetTargetPlayer(Transform target)
    {
        _Target = target;
    }

    public void SetSpawnIndex(int index)
    {
        SpawnIndex = index;
    }

    protected override void Update()
    {
        if (_Target != null)
        {
            _MoveDirection = _Target.position - transform.position;
        }
        Move(_MoveDirection.normalized);
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamageable damageableObject))
        {
            damageableObject.Damage(CalculateDamageToPlayer(), DamageSource.Enemy);
        }
    }

    protected int CalculateDamageToPlayer()
    {
        return (int)(Const.ENEMY_BASE_DAMAGE * _UnitData.AttackModifier);
    }

    protected override void OnDeath(UnitBase unit)
    {
        if (unit == this)
        {
            ServiceLocator.Get<LevelManager>().GivePlayerExp(_UnitData.GetExpWorth());
            gameObject.SetActive(false);
        }
    }
}
