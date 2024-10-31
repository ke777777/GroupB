using UnityEngine;
using UnityEngine.UI;

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

        public int initcannonball = 10; //初期の砲弾数
        public int currentcannonball; //現在の砲弾数
        public int maxCannonball = 50; //砲弾数の最大値
        public int replenishCannonball = 10; // 補充時に追加される砲弾数
        public bool isCharge; //飛距離ゲージのbool変数

        private string m_FireButton;                // The input axis that is used for launching shells.
        private float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released.
        private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
        private bool m_Fired;                       // Whether or not the shell has been launched with this button press.

        public delegate void OnShellStockChanged(int currentStock); // 砲弾所持数変化イベント参照
        public event OnShellStockChanged ShellStockChanged;         // 砲弾所持数が変化したときのイベント

        private void OnEnable()
        {
            // When the tank is turned on, reset the launch force and the UI
            m_CurrentLaunchForce = m_MinLaunchForce;
            m_AimSlider.value = m_MinLaunchForce;
        }


        private void Start ()
        {   currentcannonball = initcannonball; // 砲弾の所持数を初期化
            // The fire axis is based on the player number.
            m_FireButton = "Fire" + m_PlayerNumber;

            // The rate that the launch force charges up is the range of possible forces by the max charge time.
            m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;

            ShellStockChanged?.Invoke(currentcannonball); // 初期所持数（１０個）をイベントで通知
        }


        private void Update()
        {
            if (currentcannonball <= 0)
            {
                return;
            }

            // エイムスライダーの値を更新
            m_AimSlider.value = m_CurrentLaunchForce;

            // 発射ボタンが押されたとき
            if (Input.GetButtonDown(m_FireButton))
            {
                m_Fired = false;
                m_CurrentLaunchForce = m_MinLaunchForce;

                // チャージ音を再生
                m_ShootingAudio.clip = m_ChargingClip;
                m_ShootingAudio.Play();
            }
            // 発射ボタンを押し続けている間
            else if (Input.GetButton(m_FireButton) && !m_Fired)
            {
                // 発射力を増減させる
                m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

                // 発射力が最大値または最小値に達したら、チャージ速度の符号を反転
                if (m_CurrentLaunchForce >= m_MaxLaunchForce)
                {
                    m_CurrentLaunchForce = m_MaxLaunchForce;
                    m_ChargeSpeed = -m_ChargeSpeed; // 減少に転じる
                }
                else if (m_CurrentLaunchForce <= m_MinLaunchForce)
                {
                    m_CurrentLaunchForce = m_MinLaunchForce;
                    m_ChargeSpeed = -m_ChargeSpeed; // 増加に転じる
                }

                // エイムスライダーの値を更新
                m_AimSlider.value = m_CurrentLaunchForce;
            }
            // 発射ボタンを離したとき
            else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
            {
                Fire();
            }
        }


        private void Fire ()
        {
            // Set the fired flag so only Fire is only called once.
            m_Fired = true;

            currentcannonball--; //砲弾を一つ消費
            ShellStockChanged?.Invoke(currentcannonball); // 所持数が変化(-1)したことをイベントで通知

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
             // Reset the fired flag so the tank can fire again.
            m_Fired = false;
        }
          public void ReplenishCannonballs()
        {
            currentcannonball += replenishCannonball; //現在の所持数＋追加取得数

            // 砲弾の所持数が最大数を超えないように調整
            if (currentcannonball > maxCannonball)
            {
                currentcannonball = maxCannonball;
            }
            ShellStockChanged?.Invoke(currentcannonball); // 所持数が変化(+10)したことをイベントで通知
        }
          // 衝突時に呼ばれるメソッド
        private void OnCollisionEnter(Collision collision)
        {
            // 衝突したオブジェクトのタグが"ShellCartridge"の場合
            if (collision.gameObject.CompareTag("ShellCartridge"))
            {
                // 砲弾を補充
                ReplenishCannonballs();
            }
        }
    }
}