using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace Complete
{
    public class TankHealth : MonoBehaviourPunCallbacks, IPunObservable

    {
        public float m_StartingHealth = 100f;               // The amount of health each tank starts with.
        public Slider m_Slider;                             // The slider to represent how much health the tank currently has.
        public Image m_FillImage;                           // The image component of the slider.
        public Color m_FullHealthColor = Color.green;       // The color the health bar will be when on full health.
        public Color m_ZeroHealthColor = Color.red;         // The color the health bar will be when on no health.
        public GameObject m_ExplosionPrefab;                // A prefab that will be instantiated in Awake, then used whenever the tank dies.
        private AudioSource m_ExplosionAudio;               // The audio source to play when the tank explodes.
        private ParticleSystem m_ExplosionParticles;        // The particle system the will play when the tank is destroyed.
        private float m_CurrentHealth;                      // How much health the tank currently has.
        private bool m_Dead;                                // Has the tank been reduced beyond zero health yet?

        public delegate void OnHealthChangedDelegate(float currentHealth, float maxHealth, int playerNumber);
        public event OnHealthChangedDelegate OnHealthChanged;

        public int PlayerNumber { get; set; }
        public float CurrentHealth // 現在の体力を取得
        {
            get { return m_CurrentHealth; }
        }

        public float StartingHealth // 初期体力を取得
        {
            get { return m_StartingHealth; }
        }

        private void Awake()
        {
            // Instantiate the explosion prefab and get a reference to the particle system on it.
            m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();

            // Get a reference to the audio source on the instantiated prefab.
            m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

            // Disable the prefab so it can be activated when it's required.
            m_ExplosionParticles.gameObject.SetActive(false);
        }

        public void Initialize(int playerNumber)
        {
            PlayerNumber = playerNumber;
        }
        public new void OnEnable()
        {
            m_CurrentHealth = m_StartingHealth;
            m_Dead = false;
            SetHealthUI();
            NotifyHealthChange();
        }



        private void NotifyHealthChange()
        {
            OnHealthChanged?.Invoke(m_CurrentHealth, m_StartingHealth, PlayerNumber);
        }
        public void TakeDamage(float amount)
        {
            if (!photonView.IsMine) return;
            // Reduce current health by the amount of damage done.
            m_CurrentHealth -= amount;

            // Change the UI elements appropriately.
            SetHealthUI();
            NotifyHealthChange();

            // If the current health is at or below zero and it has not yet been registered, call OnDeath.
            if (m_CurrentHealth <= 0f && !m_Dead)
            {
                OnDeath();
            }
        }

        private void SetHealthUI()
        {
            if (m_Slider != null)
            {
                // Set the slider's value appropriately.
                m_Slider.value = m_CurrentHealth;
                float healthPercentage = m_CurrentHealth / m_StartingHealth;
                // Interpolate the color of the bar between the chosen colors based on the current percentage of the starting health.
                m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
            }
        }

        private void OnDeath()
        {
            if (!photonView.IsMine) return;
            // Set the flag so that this function is only called once.
            m_Dead = true;

            // Move the instantiated explosion prefab to the tank's position and turn it on.
            m_ExplosionParticles.transform.position = transform.position;
            m_ExplosionParticles.gameObject.SetActive(true);

            // Play the particle system of the tank exploding.
            m_ExplosionParticles.Play();

            // Play the tank explosion sound effect.
            m_ExplosionAudio.Play();

            // Turn the tank off.
            gameObject.SetActive(false);
        }
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // 自分のクライアントのデータを送信
                stream.SendNext(m_CurrentHealth);
                stream.SendNext(m_Dead);
            }
            else
            {
                // 他のクライアントからのデータを受信
                m_CurrentHealth = (float)stream.ReceiveNext();
                m_Dead = (bool)stream.ReceiveNext();

                SetHealthUI(); // UIを更新
            }
        }
    }
}
