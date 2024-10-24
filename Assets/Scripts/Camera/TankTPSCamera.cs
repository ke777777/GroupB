using UnityEngine;

public class TankTPSCamera : MonoBehaviour
{
    public Transform turretTransform; // 砲塔のTransform
    public Vector3 offset = new Vector3(0, 5, -7); // カメラのオフセット（斜め後ろの位置）
    public float smoothSpeed = 0.125f; // カメラの追従速度

    private void LateUpdate()
    {
        if (turretTransform != null)
        {
            // ターゲット位置に基づいてカメラの位置を計算
            Vector3 desiredPosition = turretTransform.position + turretTransform.rotation * offset;

            // スムーズにカメラ位置を補間
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            // カメラが常に砲塔を向くように
            transform.LookAt(turretTransform);
        }
    }
}
