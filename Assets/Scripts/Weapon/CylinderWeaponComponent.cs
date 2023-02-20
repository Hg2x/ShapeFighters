using UnityEngine;

public class CylinderWeaponComponent : WeaponComponent
{
    private float _VerticalSpeed;
    private float _CurrentHeight = 0.0f;
    private const float _MaxHeight = 10f;
    private float _HeightDirection = 1.0f;
    private float _Duration;

    private void FixedUpdate()
    {
        _CurrentHeight += _VerticalSpeed * _HeightDirection * Time.fixedDeltaTime;
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

    public void SetVerticalSpeed(float speed)
    {         
        _VerticalSpeed = speed;
    }

    public void SetDuration(float duration)
    {
        _Duration = duration;
    }
}
