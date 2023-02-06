using UnityEngine;

public class CylinderWeaponComponent : WeaponComponent
{
    [SerializeField] private float _HeightChangeSpeed = 10.0f;
    private float _CurrentHeight = 0.0f;
    private const float _MaxHeight = 10f;
    private float _HeightDirection = 1.0f;
    private float _Duration = 1f;

    private void FixedUpdate()
    {
        _CurrentHeight += _HeightChangeSpeed * _HeightDirection * Time.fixedDeltaTime;
        if (_CurrentHeight >= _MaxHeight)
        {
            _HeightDirection = -1.0f;
        }
        else if (_CurrentHeight <= 0.0f)
        {
            _HeightDirection = 1.0f;
        }
        transform.position = new Vector3(transform.position.x, _CurrentHeight, transform.position.z);

        _Duration -= Time.fixedDeltaTime;
        if (_Duration <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void SetDuration(float duration)
    {
        _Duration = duration;
    }
}
