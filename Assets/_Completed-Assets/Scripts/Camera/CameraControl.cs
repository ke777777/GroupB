using UnityEngine;

namespace Complete
{
    public class CameraControl : MonoBehaviour
    {
<<<<<<< HEAD
        public Transform SpawnPoint1;                    // Spawn point reference
        public float m_DampTime = 0.2f;                  // Approximate time for the camera to refocus.
        public float m_ScreenEdgeBuffer = 4f;            // Space between the top/bottom most target and the screen edge.
        public float m_MinSize = 6.5f;                   // The smallest orthographic size the camera can be.
        [HideInInspector] public Transform[] m_Targets;  // All the targets the camera needs to encompass.

        private Camera m_Camera;                         // Used for referencing the camera.
        private float m_ZoomSpeed;                       // Reference speed for the smooth damping of the orthographic size.
        private Vector3 m_MoveVelocity;                  // Reference velocity for the smooth damping of the position.
        private Vector3 m_DesiredPosition;               // The position the camera is moving towards.
=======
        public float m_DampTime = 0.05f;                 // Approximate time for the camera to refocus.
        public float m_FollowDistance = 2f;             // The distance behind the target the camera should stay.
        public float m_FollowHeight = 0.5f;               // The height of the camera relative to the target.
        [HideInInspector] public Transform[] m_Targets; // All the targets the camera needs to encompass.

        private Camera m_Camera;                        // Used for referencing the camera.
        private Vector3 m_MoveVelocity;                 // Reference velocity for the smooth damping of the position.
        private Vector3 m_CurrentPosition;              // The current position of the camera.
>>>>>>> 885564796fbc270a44e49626c4c58979e1e81469

        private void Awake()
        {
            m_Camera = GetComponentInChildren<Camera>();
        }

        private void FixedUpdate()
        {
<<<<<<< HEAD
            // Move the camera towards a desired position.
            Move();

            // Change the size of the camera based.
            Zoom();
        }

        private void Move()
        {
            if (m_Targets.Length == 0) return;

            // Assume the first target is the tank to track
            Transform target = m_Targets[0];

            // Set the desired position to the target's position, adjusted behind the tank.
            m_DesiredPosition = target.position + new Vector3(0, 2, -5); // Adjust as needed

            // Smoothly transition to that position.
            transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
        }

        private void Zoom()
        {
            // Find the required size based on the desired position and smoothly transition to that size.
            float requiredSize = FindRequiredSize();
            m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
        }

        private float FindRequiredSize()
        {
            // Start the camera's size calculation at zero.
            float size = 0f;

            // Go through all the targets...
            for (int i = 0; i < m_Targets.Length; i++)
=======
            if (m_Targets.Length > 0 && m_Targets[0] != null)
            {
                Move();  // Move the camera towards the first target's position.
            }
        }

        public void Move()
        {
            // ターゲットが存在する場合、そのターゲットの後ろにカメラを配置
            if (m_Targets.Length > 0 && m_Targets[0] != null)
>>>>>>> 885564796fbc270a44e49626c4c58979e1e81469
            {
                Transform target = m_Targets[0];  // 最初のターゲットを使用

<<<<<<< HEAD
                // Find the position of the target in the camera's local space.
                Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);

                // Choose the largest out of the current size and the distance of the tank 'up' or 'down' from the camera.
                size = Mathf.Max(size, Mathf.Abs(targetLocalPos.y));

                // Choose the largest out of the current size and the calculated size based on the tank being to the left or right of the camera.
                size = Mathf.Max(size, Mathf.Abs(targetLocalPos.x) / m_Camera.aspect);
            }

            // Add the edge buffer to the size.
            size += m_ScreenEdgeBuffer;

            // Make sure the camera's size isn't below the minimum.
            size = Mathf.Max(size, m_MinSize);

            return size;
        }

        public void SetStartPositionAndSize()
        {
            // Set the camera's position to the desired position without damping.
            if (m_Targets.Length > 0)
            {
                // Using the first target to set the camera position
                transform.position = m_Targets[0].position + new Vector3(0, 2, -5); // Adjust as needed
            }
            else
            {
                // Fallback to SpawnPoint1 if no targets are available
                transform.position = SpawnPoint1.position + new Vector3(0, 5, 15); // Initial position if no targets
            }

            // Find and set the required size of the camera.
            m_Camera.orthographicSize = FindRequiredSize();
        }
=======
                // ターゲットの後ろにカメラを配置する位置を計算
                Vector3 targetPosition = target.position - (target.forward * m_FollowDistance);

                // Y座標の設定
                targetPosition.y = target.position.y + m_FollowHeight + 5f;

                // 斜め上に移動
                targetPosition += target.up * 0.3f; // 斜め上に移動（必要に応じて調整）

                // カメラの位置を即座に設定
                transform.position = targetPosition;

                // カメラをターゲットの方向に向ける
                transform.LookAt(target.position + Vector3.up * (m_FollowHeight + 5f));
            }
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
>>>>>>> 885564796fbc270a44e49626c4c58979e1e81469
    }
}
