using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;
namespace Complete
{
    public class TankShooting : MonoBehaviourPun
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

        /*
        public int initcannonball = 10; //初期の砲弾数
        public int currentcannonball; //現在の砲弾数
        public int maxCannonball = 50; //砲弾数の最大値
        public int replenishCannonball = 10; // 補充時に追加される砲弾数
        public bool isCharge; //飛距離ゲージのbool変数
        */

        private string m_FireButton;                // The input axis that is used for launching shells.
        private float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released.
        private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
        private bool m_Fired;                       // Whether or not the shell has been launched with this button press.

        private Dictionary<string, WeaponStockData> weaponStockDictionary = new Dictionary<string, WeaponStockData>()
        {
            { "Shell", new WeaponStockData(10, 50, 10) },
            { "Mine", new WeaponStockData(1, 3, 1) }
        };
        private WeaponStockData shellStockData; //砲弾の所持数
        private WeaponStockData mineStockData; //地雷の所持数

        [SerializeField] private GameObject minePrefab;
        private string minePlaceButton;
        public delegate void OnWeaponStockChanged(string weaponName, int currentStock);   // 地雷の所持数が変化したことを通知するイベントを呼びだす
        public event OnWeaponStockChanged WeaponStockChanged;            // 地雷所持数の変化を通知

        public delegate void OnMinePlaced();   // 地雷設置されたことを通知するイベントを呼び出す
        public event OnMinePlaced MinePlaced;
        private void OnEnable()
        {
            // When the tank is turned on, reset the launch force and the UI
            m_CurrentLaunchForce = m_MinLaunchForce;
            m_AimSlider.value = m_MinLaunchForce;
        }


        /*private void Start ()
        {   currentcannonball = initcannonball; // 砲弾の所持数を初期化
            // The fire axis is based on the player number.
            m_FireButton = "Fire" + m_PlayerNumber;

            // The rate that the launch force charges up is the range of possible forces by the max charge time.
            m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;

            ShellStockChanged?.Invoke(currentcannonball);
            minePlaceButton = "PlaceMine" + m_PlayerNumber;
            WeaponStockChanged?.Invoke(mineStockData.CurrentMineNumber); // 初期地雷所持数を通知
        }*/

        private void Start()
        {
            shellStockData = new WeaponStockData(10, 50, 10);
            mineStockData = new WeaponStockData(1, 3, 1);

            // 武器の初期化と辞書登録
            weaponStockDictionary = new Dictionary<string, WeaponStockData>
            {
                { "Shell", shellStockData },
                { "Mine", mineStockData }
            };

            // 各武器の初期所持数を設定
            foreach (var weapon in weaponStockDictionary)
            {
                weapon.Value.InitializeWeaponNumber();
                WeaponStockChanged?.Invoke(weapon.Key, weapon.Value.CurrentWeaponNumber); // 初期所持数を通知
            }

            m_FireButton = "Fire" + m_PlayerNumber;
            minePlaceButton = "PlaceMine" + m_PlayerNumber;

            m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
        }
        private void Update()
        {
            m_AimSlider.value = m_CurrentLaunchForce;

            if (Input.GetButtonDown(m_FireButton) && weaponStockDictionary["Shell"].CurrentWeaponNumber > 0)
            {
                m_Fired = false;
                m_CurrentLaunchForce = m_MinLaunchForce;

                m_ShootingAudio.clip = m_ChargingClip;
                m_ShootingAudio.Play();
            }

            else if (Input.GetButton(m_FireButton) && !m_Fired && weaponStockDictionary["Shell"].CurrentWeaponNumber > 0)
            {
                m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

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

                m_AimSlider.value = m_CurrentLaunchForce;
            }

            else if (Input.GetButtonUp(m_FireButton) && !m_Fired && weaponStockDictionary["Shell"].CurrentWeaponNumber > 0)
            {
                Fire();
            }
            if (Input.GetButtonDown(minePlaceButton))
            {
                PlaceMine();
            }
        }


        private void Fire()
        {
            // Set the fired flag so only Fire is only called once.
            m_Fired = true;

            weaponStockDictionary["Shell"].DecrementWeaponNumber();
            WeaponStockChanged?.Invoke("Shell", weaponStockDictionary["Shell"].CurrentWeaponNumber);

            // Create an instance of the shell and store a reference to it's rigidbody.
            Rigidbody shellInstance =
                Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

            // Set the shell's velocity to the launch force in the fire position's forward direction.
            shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

            // Change the clip to the firing clip and play it.
            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play();

            // Reset the launch force.  This is a precaution in case of missing button events.
            m_CurrentLaunchForce = m_MinLaunchForce;
            // Reset the fired flag so the tank can fire again.
            m_Fired = false;
        }

        /* public void ReplenishCannonballs()
        {
            currentcannonball += replenishCannonball; //現在の所持数＋追加取得数

            // 砲弾の所持数が最大数を超えないように調整
            if (currentcannonball > maxCannonball)
            {
                currentcannonball = maxCannonball;
            }
            ShellStockChanged?.Invoke(currentcannonball); // 所持数が変化(+10)したことをイベントで通知
        } */

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("ShellCartridge"))
            {
                weaponStockDictionary["Shell"].GainingWeaponNunber();
                WeaponStockChanged?.Invoke("Shell", weaponStockDictionary["Shell"].CurrentWeaponNumber);
                Destroy(collision.gameObject);
            }

            if (collision.gameObject.CompareTag("MineCartridge"))
            {
                weaponStockDictionary["Mine"].GainingWeaponNunber();
                WeaponStockChanged?.Invoke("Mine", weaponStockDictionary["Mine"].CurrentWeaponNumber);
                Destroy(collision.gameObject);
            }
        }

        private void PlaceMine()
        {
            // 地雷の所持数が0以下の場合は設置できないようにする
            if (weaponStockDictionary["Mine"].CurrentWeaponNumber <= 0)
            {
                Debug.Log("No mines available to place."); // 地雷がない場合
                return;
            }

            // 地雷を生成
            Instantiate(minePrefab, transform.position, Quaternion.identity);

            weaponStockDictionary["Mine"].DecrementWeaponNumber();
            WeaponStockChanged?.Invoke("Mine", weaponStockDictionary["Mine"].CurrentWeaponNumber);

            // 地雷を設置したことを通知
            MinePlaced?.Invoke();
        }
    }
}
