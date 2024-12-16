using UnityEngine;
namespace Complete
{
    public class CameraControl : MonoBehaviour
    {
        public float m_DampTime = 0.05f;                 // Approximate time for the camera to refocus.
        public float m_FollowDistance = 2f;             // The distance behind the target the camera should stay.
        public float m_FollowHeight = 0.5f;               // The height of the camera relative to the target.
        // [HideInInspector] public Transform[] m_Targets; // All the targets the camera needs to encompass.
        private Transform m_Target;

        private Camera m_Camera;                        // Used for referencing the camera.
        private Vector3 m_MoveVelocity;                 // Reference velocity for the smooth damping of the position.
        private Vector3 m_CurrentPosition;              // The current position of the camera.

        private void Awake()
        {
            m_Camera = GetComponentInChildren<Camera>();
        }

        private void FixedUpdate()
        {
            // 自分のタンクを追跡
            if (m_Target != null)
            {
                Move();
            }
        }

        public void Move()
        {
            // Transform target = m_Targets[0];  // 最初のターゲットを使用

            // ターゲットの後ろにカメラを配置する位置を計算
            Vector3 targetPosition = m_Target.position - (m_Target.forward * m_FollowDistance);

            // Y座標の設定
            targetPosition.y = m_Target.position.y + m_FollowHeight + 5f;

            targetPosition += m_Target.up * 0.3f; // 斜め上に移動（必要に応じて調整）

            // カメラの位置を即座に設定
            transform.position = targetPosition;

            // カメラをターゲットの方向に向ける
            transform.LookAt(m_Target.position + Vector3.up * (m_FollowHeight + 5f));
        }


        public void SetTarget(Transform target)
        {
            m_Target = target;
        }
        /* public void SetStartPositionAndSize()
        {
            // カメラをターゲットの後ろに即座に移動させ、正しい高さに設定
            if (m_Targets.Length > 0 && m_Targets[0] != null)
            {
                Transform target = m_Targets[0];  // 最初のターゲットを使用

                // ターゲットの後ろにカメラを配置する位置を計算
                transform.position = target.position - (target.forward * m_FollowDistance) + (Vector3.up * m_FollowHeight);

                // ターゲットを向く
                transform.LookAt(target.position + Vector3.up * m_FollowHeight / 2f);
            }
        } */
    }
}
