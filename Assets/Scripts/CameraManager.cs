using Cinemachine;
using UnityEngine;
using DG.Tweening;
using System;
using DG.Tweening.Core.Easing;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraManager : MonoBehaviour
{
    private CinemachineVirtualCamera _VirtualCamera;
    private CinemachineTransposer _Transposer;

    private void Awake()
    {
        _VirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void SetCameraTarget(GameObject target)
    {
        if (target != null)
        {
            if (_VirtualCamera != null)
            {
                _VirtualCamera.LookAt = target.transform;
                _VirtualCamera.Follow = target.transform;
            }
        }

        InitCameraConfigTest();
    }

    private void InitCameraConfigTest()
    {
        if (_VirtualCamera != null)
        {
            //_VirtualCamera.m_Lens.FieldOfView = 90f;
            _Transposer = _VirtualCamera.AddCinemachineComponent<CinemachineTransposer>();
            _Transposer.m_BindingMode = CinemachineTransposer.BindingMode.LockToTargetOnAssign;
            _Transposer.m_FollowOffset = new Vector3(0f, 20f, -10f);
            _Transposer.m_XDamping = 0f;
            _Transposer.m_YDamping = 0f;
            _Transposer.m_ZDamping = 0f;

            _VirtualCamera.AddCinemachineComponent<CinemachineHardLookAt>();
        }
    }

    public void ActiveSkillSequence(float zoomAmount, float duration, float durationCutOff = 0.5f)
    {
        void ZoomOutCallback() { ZoomOut(zoomAmount, duration * (1f - durationCutOff)); }
        ZoomIn(zoomAmount, duration * durationCutOff, ZoomOutCallback);
    }

    private void ZoomIn(float zoomAmount, float duration, Action onCompleteCallback = null, Ease easeCurve = Ease.InQuad)
    {
        DOTween.To(() => _Transposer.m_FollowOffset.y, y => _Transposer.m_FollowOffset.y = y, _Transposer.m_FollowOffset.y - zoomAmount, duration)
            .SetEase(easeCurve)
            .OnComplete(() => onCompleteCallback?.Invoke());
    }
  
    private void ZoomOut(float zoomAmount, float duration, Action onCompleteCallback = null, Ease easeCurve = Ease.OutQuad)
    {
        DOTween.To(() => _Transposer.m_FollowOffset.y, y => _Transposer.m_FollowOffset.y = y, _Transposer.m_FollowOffset.y + zoomAmount, duration)
                    .SetEase(easeCurve)
                    //.SetDelay(duration)
                    .OnComplete(() => onCompleteCallback?.Invoke());
    }
}
