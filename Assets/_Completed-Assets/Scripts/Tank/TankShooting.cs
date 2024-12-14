using UnityEngine;
using UnityEngine.UI;
using System;

namespace Complete
{
    public class TankShooting : MonoBehaviour
    {
        public int m_PlayerNumber = 1;              // Used to identify the different players.
        public Rigidbody m_Shell;                   // Prefab of the shell.
        public Transform m_FireTransform;           // A child of the tank where the shells are spawned.
        public Slider m_AimSlider;                  // A child of the tank that displays the current launch force.
        public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
        public AudioClip m_ChargingClip;            // Audio that plays when each shot is charging up.
        public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
        public float m_MinLaunchForce = 15f;        // The force given to the shell if the fire button is not held.
        public float m_MaxLaunchForce = 30f;        // The force given to the shell if the fire button is held for the max charge time.
        public float m_MaxChargeTime = 0.75f;       // How long the shell can charge for before it is fired at max force.


        private string m_FireButton;                // The input axis that is used for launching shells.
        private float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released.
        private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
        private bool m_Fired;                       // Whether or not the shell has been launched with this button press.
        private bool m_IsCharging;                  // Whether the charging of the launch force is ongoing.
        private bool m_IsIncreasing = true; // ゲージが伸びているか縮んでいるかを表す変数
        private float m_CurrentChargeTime = 0f; // 現在のチャージ時間


        public int m_InitialAmmo = 10;               // ゲーム開始時の砲弾数
        public int m_CurrentAmmo;                    // 現在の砲弾数
        public int m_MaxAmmo = 50;                   // 最大所持可能な砲弾数
        public int m_AmmoRefillAmount = 10;          // 補充する砲弾の数


        public event Action<int> OnShellStockChanged; // 砲弾の所持数が変化したときに通知するイベント


        private void OnEnable()
        {
            // When the tank is turned on, reset the launch force and the UI
            m_CurrentLaunchForce = m_MinLaunchForce;
            m_AimSlider.value = m_MinLaunchForce;
            m_CurrentAmmo = m_InitialAmmo;
            m_IsCharging = false;
        }


        private void Start ()
        {
            // The fire axis is based on the player number.
            m_FireButton = "Fire" + m_PlayerNumber;

            // The rate that the launch force charges up is the range of possible forces by the max charge time.
            m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
        }


        private void Update ()
        {
            if (m_CurrentAmmo <= 0)
            {
                Debug.Log("No ammo left! Cannot fire.");
                return;
            }

            // The slider should have a default value of the minimum launch force.
            m_AimSlider.value = m_MinLaunchForce;

            // 砲弾の発射ゲージが伸びるか縮むかを管理
            if (Input.GetButtonDown(m_FireButton))
            {
                // チャージ開始
                m_Fired = false;
                m_CurrentLaunchForce = m_MinLaunchForce;
                m_IsIncreasing = true;
                m_ShootingAudio.clip = m_ChargingClip;
                m_ShootingAudio.Play();
            }
            // チャージ中の場合
            else if (Input.GetButton(m_FireButton) && !m_Fired)
            {
                // ゲージが伸びている場合
                if (m_IsIncreasing)
                {
                    m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
                    if (m_CurrentLaunchForce >= m_MaxLaunchForce)
                    {
                        m_CurrentLaunchForce = m_MaxLaunchForce;
                        m_IsIncreasing = false; // 最大値に達したら縮み始める
                    }
                }
                // ゲージが縮んでいる場合
                else
                {
                    m_CurrentLaunchForce -= m_ChargeSpeed * Time.deltaTime;
                    if (m_CurrentLaunchForce <= m_MinLaunchForce)
                    {
                        m_CurrentLaunchForce = m_MinLaunchForce;
                        m_IsIncreasing = true; // 最小値に達したら伸び始める
                    }
                }

                m_AimSlider.value = m_CurrentLaunchForce; // ゲージの表示を更新
            }
            // 発射ボタンを離したら発射
            else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
            {
                Fire();
            }
        }


        private void Fire ()
        {
            // Set the fired flag so only Fire is only called once.
            m_Fired = true;

            // Create an instance of the shell and store a reference to it's rigidbody.
            Rigidbody shellInstance =
                Instantiate (m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

            // Set the shell's velocity to the launch force in the fire position's forward direction.
            shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward; 

            // Change the clip to the firing clip and play it.
            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play ();

            // Reset the launch force.  This is a precaution in case of missing button events.
            m_CurrentLaunchForce = m_MinLaunchForce;


            if (m_CurrentAmmo > 0)
            {
                // 砲弾を発射し、発射後に砲弾数を1減少
                m_CurrentAmmo--;
                OnShellStockChanged?.Invoke(m_CurrentAmmo);
                Debug.Log("Fired! Remaining ammo: " + m_CurrentAmmo);
            }
            else
            {
                Debug.Log("Cannot fire. Ammo is empty.");
                return;
            }
        }


        public void RefillAmmo()
        {
            if (m_CurrentAmmo < m_MaxAmmo)
            {
                m_CurrentAmmo += m_AmmoRefillAmount;
                if (m_CurrentAmmo > m_MaxAmmo)
                {
                    m_CurrentAmmo = m_MaxAmmo;
                }
                OnShellStockChanged?.Invoke(m_CurrentAmmo);
                Debug.Log("Ammo refilled! Current ammo: " + m_CurrentAmmo);
            }
            else
            {
                Debug.Log("Ammo already full.");
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            // 衝突相手がShellCartridgeタグの場合、砲弾を補充
            if (collision.gameObject.CompareTag("ShellCartridge"))
            {
                RefillAmmo(); // 砲弾補充メソッドを呼び出し
                Destroy(collision.gameObject); // 衝突したShellCartridgeを消去
            }
        }
    }
}