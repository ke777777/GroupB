using UnityEngine;
using Photon.Pun;

namespace Complete
{
    public class TankMovement : MonoBehaviourPun, IPunObservable
    {
        public int m_PlayerNumber;              // Used to identify which tank belongs to which player.  This is set by this tank's manager.
        public float m_Speed = 12f;                 // How fast the tank moves forward and back.
        public float m_TurnSpeed = 180f;            // How fast the tank turns in degrees per second.
        public float m_TurretTurnSpeed = 90f;       // How fast the turret rotates in degrees per second.
        public string m_TurretTurnAxisName;         // The name of the input axis for turret turning.
        public GameObject m_Turret;                  // Reference to the turret GameObject.

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
        private bool isMine;                        // 自分のタンクかどうか判定
        private ParticleSystem[] m_particleSystems; // References to all the particles systems used by the Tanks
        private Transform m_TurretTransform;
        private PhotonTransformView photonTransformView;
        private bool isInvincible = false; // ワームホール使用中の無敵確認
        private bool canAct = true;
        private Renderer[] renderers; // 点滅エフェクト用
        public bool isCooldown = false; // クールダウン判定
        public delegate void InvincibilityChanged(bool state);
        public event InvincibilityChanged OnInvincibilityChanged;

        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_TurretTransform = transform.Find("TankRenderers/TankTurret");
            photonTransformView = GetComponent<PhotonTransformView>();

            if (photonView.InstantiationData != null && photonView.InstantiationData.Length > 0)
            {
                m_PlayerNumber = (int)photonView.InstantiationData[0];
            }
            else
            {
                Debug.LogError("m_PlayerNumber not set in InstantiationData");
            }
        }


        private void OnEnable()
        {
            m_Rigidbody.isKinematic = false;

            m_MovementInputValue = 0f;
            m_TurnInputValue = 0f;

            m_particleSystems = GetComponentsInChildren<ParticleSystem>();
            foreach (var ps in m_particleSystems)
            {
                ps.Play();
            }
        }


        private void OnDisable()
        {
            m_Rigidbody.isKinematic = true;

            foreach (var ps in m_particleSystems)
            {
                ps.Stop();
            }
        }


        private void Start()
        {
            isMine = photonView.IsMine;
            m_OriginalPitch = m_MovementAudio.pitch;
            if (photonView.IsMine)
            {
                m_MovementAxisName = "Vertical1";
                m_TurnAxisName = "Horizontal1";
                m_TurretTurnAxisName = "TurretTurn1";
            }
            renderers = GetComponentsInChildren<Renderer>();
        }


        private void Update()
        {
            if (!photonView.IsMine) return; // 自分のタンク以外は操作しない
            m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
            m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

            EngineAudio();
            if (!canAct) return; // 行動制限中は操作不可
        }

        public void SetInvincible(bool state)
        {
            isInvincible = state;
            OnInvincibilityChanged?.Invoke(state); // 状態変更時に通知
        }
        private void FixedUpdate()
        {
            if (!isMine || isInvincible) return;

            Move();
            Turn();
            TurretTurn();
        }

        public void DisableActions()
        {
            canAct = false;
        }

        public void EnableActions()
        {
            canAct = true;
        }

        private void Move()
        {
            Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        }


        private void Turn()
        {
            float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
        }


        private void TurretTurn()
        {
            if (!photonView.IsMine) return;

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

        public void Teleport(Vector3 position)
        {
            if (photonView.IsMine)
            {
                transform.position = position;
                photonView.RPC("RPC_Teleport", RpcTarget.Others, position);
            }
        }

        [PunRPC]
        private void RPC_Teleport(Vector3 position)
        {
            transform.position = position;
            if (photonTransformView != null)
            {
                photonTransformView.enabled = false;
                photonTransformView.enabled = true;
            }
        }
        private void EngineAudio()
        {
            if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
            {
                if (m_MovementAudio.clip == m_EngineDriving)
                {
                    m_MovementAudio.clip = m_EngineIdling;
                    m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                    m_MovementAudio.Play();
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
        public void StartBlinking()
        {
            photonView.RPC("RPC_StartBlinking", RpcTarget.All);
        }

        [PunRPC]
        private void RPC_StartBlinking()
        {
            StartCoroutine(BlinkEffect());
        }

        [PunRPC]
        public void RPC_StopBlinking()
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
            photonView.RPC("RPC_StartCooldown", RpcTarget.All, duration);
        }

        [PunRPC]
        private void RPC_StartCooldown(float duration)
        {
            StartCoroutine(CooldownCoroutine(duration));
        }
        private System.Collections.IEnumerator CooldownCoroutine(float duration)
        {
            isCooldown = true;
            yield return new WaitForSeconds(duration);
            isCooldown = false;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(m_MovementInputValue);
                stream.SendNext(m_TurnInputValue);
                stream.SendNext(isInvincible);
                stream.SendNext(m_TurretTransform.localRotation);
            }
            else
            {
                //m_MovementInputValue = (float)stream.ReceiveNext();
                //m_TurnInputValue = (float)stream.ReceiveNext();
                Vector3 receivedPosition = (Vector3)stream.ReceiveNext();
                Quaternion receivedRotation = (Quaternion)stream.ReceiveNext();
                m_Rigidbody.position = Vector3.Lerp(m_Rigidbody.position, receivedPosition, Time.deltaTime * 5);
                m_Rigidbody.rotation = Quaternion.Lerp(m_Rigidbody.rotation, receivedRotation, Time.deltaTime * 5);
                isInvincible = (bool)stream.ReceiveNext();
                m_TurretTransform.localRotation = (Quaternion)stream.ReceiveNext();
            }
        }
    }
}
