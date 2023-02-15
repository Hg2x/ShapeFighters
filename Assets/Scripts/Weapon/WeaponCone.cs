using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCone : WeaponBase
{
    [SerializeField] private GameObject _ConeRef;
    [SerializeField] private float _Speed = 1000f;

    private readonly float _Duration = 3f;

    override public int GetID()
    {
        _WeaponID = 3;
        return base.GetID();
    }

    protected override void Awake()
    {
        base.Awake();

        _AttackSpeed = 1f;
        _ActiveSkillCooldown = 30f;
    }

    override protected void LowerBodySkill() 
    {
        var curDirection = _Player.GetComponent<UnitBase>().CurrentDirection;
        var position = _Player.transform.localPosition + Vector3.Scale(curDirection, new Vector3(3f, 0.5f, 3f));
        var rotation = Quaternion.Euler(new Vector3(curDirection.z * 90f, 0.5f, curDirection.x * -90f));
        Debug.Log(curDirection);

        var cone = Instantiate(_ConeRef, position, rotation, transform);
        cone.SetActive(false);
        cone.TryGetComponent(out Rigidbody rb);
        if (rb != null)
        {
            cone.SetActive(true);
            rb.AddRelativeForce(new Vector3(0, _Speed, 0));
        }
        Destroy(cone, _Duration);
    }

    protected override void ApplyUpperBodyPassive()
    {
        base.ApplyUpperBodyPassive();

        // implement as buff later
        GameInstance.GetLevelManager().PlayerStatusData.ModifySetVariable("AttackModifier", 1f, "+");
    }

    protected override void RemoveUpperBodyPassive()
    {
        base.RemoveUpperBodyPassive();

        // implement as buff later
        GameInstance.GetLevelManager().PlayerStatusData.ModifySetVariable("AttackModifier", -1f, "+");
    }
}
