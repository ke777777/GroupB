using UnityEngine;

namespace Complete
{
    public class TankMovement : MonoBehaviour
    {
        public int m_PlayerNumber = 1;              // Used to identify which tank belongs to which player.  This is set by this tank's manager.
        public float m_Speed = 12f;                 // How fast the tank moves forward and back.
        public float m_TurnSpeed = 180f;            // How fast the tank turns in degrees per second.
        public float m_TurretTurnSpeed = 90f;       // How fast the turret rotates in degrees per second.
        public string m_TurretTurnAxisName;         // The name of the input axis for turret turning.
        public GameObject m_Turret;                  // Reference to the turret GameObject.
<<<<<<< HEAD
        
=======

>>>>>>> 885564796fbc270a44e49626c4c58979e1e81469
        public AudioSource m_MovementAudio;         // Reference to the audio source used to play engine sounds.
        public AudioClip m_EngineIdling;            // Audio to play when the tank isn't moving.
        public AudioClip m_EngineDriving;           // Audio to play when the tank is moving.
		public float m_PitchRange = 0.2f;           // The amount by which the pitch of the engine noises can vary.

        private string m_MovementAxisName;          // The name of the input axis for moving forward and back.
        private string m_TurnAxisName;              // The name of the input axis for turning.
        private Rigidbody m_Rigidbody;              // Reference used to move the tank.
        private float m_MovementInputValue;         // The current value of the movement input.
        private float m_TurnInputValue;             // The current value of the turn input.
        private float m_OriginalPitch;              // The pitch of the audio source at the start of the scene.
        private ParticleSystem[] m_particleSystems; // References to all the particles systems used by the Tanks

        private bool isInvincible = false; // ワームホール使用中の無敵確認
        private bool canAct = true;
        private Renderer[] renderers; // 点滅エフェクト用
        public bool isCooldown = false; // クールダウン判定
        private void Awake ()
        {
            m_Rigidbody = GetComponent<Rigidbody> ();
        }


        private void OnEnable ()
        {
            m_Rigidbody.isKinematic = false;

            m_MovementInputValue = 0f;
            m_TurnInputValue = 0f;

            m_particleSystems = GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < m_particleSystems.Length; ++i)
            {
                m_particleSystems[i].Play();
            }
        }


        private void OnDisable ()
        {
            m_Rigidbody.isKinematic = true;

            for(int i = 0; i < m_particleSystems.Length; ++i)
            {
                m_particleSystems[i].Stop();
            }
        }


        private void Start ()
        {
            m_MovementAxisName = "Vertical" + m_PlayerNumber;
            m_TurnAxisName = "Horizontal" + m_PlayerNumber;

            // Initialize the turret turn axis name based on player number
            m_TurretTurnAxisName = "TurretTurn" + m_PlayerNumber; // Assuming the turret turn axis is defined like this

            m_OriginalPitch = m_MovementAudio.pitch;
            renderers = GetComponentsInChildren<Renderer>();
        }


        private void Update ()
        {
            m_MovementInputValue = Input.GetAxis (m_MovementAxisName);
            m_TurnInputValue = Input.GetAxis (m_TurnAxisName);

            EngineAudio ();
            if (!canAct) return; // 行動制限中は操作不可
        }

        public void SetInvincible(bool state)
        {
        isInvincible = state;
        }
        private void FixedUpdate ()
        {
            Move ();
            Turn ();
            TurretTurn(); // Call the turret rotation method
        }

        public void DisableActions()
        {
            canAct = false;
        }

        public void EnableActions()
        {
            canAct = true;
        }

        private void Move ()
        {
            Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        }


        private void Turn ()
        {
            float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
            Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);
            m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation);
        }


        private void TurretTurn ()
        {
            // Get the input for turret turning
            float turretTurnInput = Input.GetAxis(m_TurretTurnAxisName);

            // Calculate the rotation angle based on input and turret turn speed
            float turretTurn = turretTurnInput * m_TurretTurnSpeed * Time.deltaTime;

            // Update the turret's rotation on the Y axis
            if (m_Turret != null) // Check if the turret reference is set
            {
                Quaternion turretTurnRotation = Quaternion.Euler(0f, turretTurn, 0f);
                m_Turret.transform.rotation = m_Turret.transform.rotation * turretTurnRotation;
            }
        }


        private void FixedUpdate ()
        {
            Move ();
            Turn ();
            TurretTurn(); // Call the turret rotation method
        }


        private void Move ()
        {
            Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        }


        private void Turn ()
        {
            float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
            Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);
            m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation);
        }


        private void TurretTurn ()
        {
            // Get the input for turret turning
            float turretTurnInput = Input.GetAxis(m_TurretTurnAxisName);

            // Calculate the rotation angle based on input and turret turn speed
            float turretTurn = turretTurnInput * m_TurretTurnSpeed * Time.deltaTime;

            // Update the turret's rotation on the Y axis
            if (m_Turret != null) // Check if the turret reference is set
            {
                Quaternion turretTurnRotation = Quaternion.Euler(0f, turretTurn, 0f);
                m_Turret.transform.rotation = m_Turret.transform.rotation * turretTurnRotation;
            }
        }


        private void EngineAudio ()
        {
            if (Mathf.Abs (m_MovementInputValue) < 0.1f && Mathf.Abs (m_TurnInputValue) < 0.1f)
            {
                if (m_MovementAudio.clip == m_EngineDriving)
                {
                    m_MovementAudio.clip = m_EngineIdling;
                    m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                    m_MovementAudio.Play ();
                }
            }
            else
            {
                if (m_MovementAudio.clip == m_EngineIdling)
                {
                    m_MovementAudio.clip = m_EngineDriving;
                    m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                    m_MovementAudio.Play();
                }
            }
        }
<<<<<<< HEAD
    }
=======
        public void StartBlinking()
        {
            StartCoroutine(BlinkEffect());
        }

        public void StopBlinking()
        {
            StopAllCoroutines(); // 点滅エフェクトを停止
            SetVisible(true);
        }

        private System.Collections.IEnumerator BlinkEffect()
        {
            while (true)
            {
                SetVisible(false);
                yield return new WaitForSeconds(0.1f);
                SetVisible(true);
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void SetVisible(bool isVisible)
        {
            foreach (var renderer in renderers)
            {
                renderer.enabled = isVisible;
            }
        }
        public void StartCooldown(float duration)
        {
            StartCoroutine(CooldownCoroutine(duration));
        }

        private System.Collections.IEnumerator CooldownCoroutine(float duration)
        {
        isCooldown = true;
        yield return new WaitForSeconds(duration);
        isCooldown = false;
    }
    }
>>>>>>> 885564796fbc270a44e49626c4c58979e1e81469
}
