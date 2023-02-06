using UnityEngine;

public class EnemyUnit : UnitBase, IDamageable
{
    public int SpawnIndex { get; protected set; }
    private Transform _Target;
    private Vector2 _MoveDirection;

    // TODO: implement better pathfinding
    public void Damage(int damageTaken, DamageSource source)
    {
        if (source == DamageSource.Friendly)
        {
            TakeDamage(damageTaken);

            if (_UnitData.Health <= 0)
            {
                _UnitData.Health = 0;
                GameInstance.GetLevelManager().GivePlayerExp(_UnitData.GetExpWorth());
                gameObject.SetActive(false);
            }
        }
    }

    public void SetTargetPlayer(Transform target)
    {
        _Target = target;
    }
    public void SetSpawnIndex(int index)
    {
        SpawnIndex = index;
    }

    protected override void Awake()
    {
        base.Awake();

        _Rigidbody = GetComponent<Rigidbody>();
    }

    protected void FixedUpdate()
    {
        Move(_UnitData.MoveSpeed * _MoveDirection);
    }

    protected override void Update()
    {
        if (_Target != null)
        {
            var direction = _Target.position - transform.position;
            _MoveDirection = new Vector2(direction.x, direction.z).normalized;
        }
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
}
