using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraManager : MonoBehaviour
{
    private CinemachineVirtualCamera _VirtualCamera;

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
            var transposer = _VirtualCamera.AddCinemachineComponent<CinemachineTransposer>();
            transposer.m_BindingMode = CinemachineTransposer.BindingMode.LockToTargetOnAssign;
            transposer.m_FollowOffset = new Vector3(0f, 20f, -10f);
            transposer.m_XDamping = 0f;
            transposer.m_YDamping = 0f;
            transposer.m_ZDamping = 0f;

            _VirtualCamera.AddCinemachineComponent<CinemachineHardLookAt>();
        }
    }
}
