using UnityEngine;

public class CubeWeaponComponent : WeaponComponent
{
    public override void SetSize(float size)
    {
        base.SetSize(size);
        transform.localScale = new Vector3(2 * size, 1, 0.5f);
    }
}
