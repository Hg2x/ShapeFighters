using System.Collections;
using UnityEngine;

public class WeaponCube : WeaponBase
{
    [SerializeField] private GameObject _WideCubeRef;
    [SerializeField] private float _Speed = 10f;
    private float _Distance;
    private float _DurationLeft = 0f;
    private GameObject _WideCube;

    public override int GetID()
    {
        _WeaponID = 2;
        return base.GetID();
    }

    protected override void Awake()
    {
        base.Awake();

        _AttackSpeed = 1f;
        _ActiveSkillCooldown = 60f;

        _WideCube = Instantiate(_WideCubeRef, transform);
        _WideCube.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (_DurationLeft > 0)
        {
            _DurationLeft -= Time.fixedDeltaTime;
            _WideCube.TryGetComponent(out Rigidbody rb);
            if (rb != null)
            {
                if (_DurationLeft > 0)
                {
                    _WideCube.SetActive(true);
                    _Distance += _Speed * Time.fixedDeltaTime;
                    Vector3 pos = _Player.transform.position + new Vector3(0, 0, _Distance + 1f);
                    rb.MovePosition(pos);
                }
                else
                {
                    _WideCube.SetActive(false);
                    _Distance = 0f;
                    rb.MovePosition(_Player.transform.position + new Vector3(0, 0, 1f));
                }
            }
        }
    }

    protected override void ActiveSkill()
    {
        base.ActiveSkill();

        // TODO: implement as buff later
        GameInstance.GetLevelManager().PlayerStatusData.ModifySetVariable("IsInvincible", true);
        _ActiveSkillCooldownLeft = _ActiveSkillCooldown;
        StartCoroutine(TurnOffInvinciblility());
    }

    private IEnumerator TurnOffInvinciblility()
    {
        yield return new WaitForSeconds(5f);
        GameInstance.GetLevelManager().PlayerStatusData.ModifySetVariable("IsInvincible", false);
    }

    protected override void UpperBodySkill() 
    {
        _DurationLeft = 0.5f;
    }

    protected override void ApplyUpperBodyPassive()
    {
        base.ApplyUpperBodyPassive();

        // implement as buff later
        GameInstance.GetLevelManager().PlayerStatusData.ModifySetVariable("DefenseModifier", 0.5f, "+");
    }

    protected override void RemoveUpperBodyPassive()
    {
        base.RemoveUpperBodyPassive();

        // implement as buff later
        GameInstance.GetLevelManager().PlayerStatusData.ModifySetVariable("DefenseModifier", -0.5f, "+");
    }
}
