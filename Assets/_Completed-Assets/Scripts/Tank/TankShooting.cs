using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Photon.Pun;

namespace Complete
{
    public class TankShooting : MonoBehaviourPun
    {
        public int m_PlayerNumber;                  // Used to identify the different players.
        public Rigidbody m_Shell;                   // Prefab of the shell.
        public Transform m_FireTransform;           // A child of the tank where the shells are spawned.
        public Slider m_AimSlider;                  // A child of the tank that displays the current launch force.
        public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
        public AudioClip m_ChargingClip;            // Audio that plays when each shot is charging up.
        public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
        public float m_MinLaunchForce = 15f;        // The force given to the shell if the fire button is not held.
        public float m_MaxLaunchForce = 30f;        // The force given to the shell if the fire button is held for the max charge time.
        public float m_MaxChargeTime = 0.75f;       // How long the shell can charge for before it is fired at max force.

        private string m_FireButton;                // The input axis for launching shells.
        private string minePlaceButton;             // The input axis for placing mines.
        private float m_CurrentLaunchForce;         // The force for the shell when fired.
        private float m_ChargeSpeed;                // Speed of force charge.
        private bool m_Fired;                       // Whether the shell has been fired.

        private Dictionary<string, WeaponStockData> weaponStockDictionary;
        private WeaponStockData shellStockData;
        private WeaponStockData mineStockData;

        [SerializeField] private GameObject minePrefab;

        public delegate void OnWeaponStockChanged(string weaponName, int currentStock);
        public event OnWeaponStockChanged WeaponStockChanged;

        public delegate void OnMinePlaced();
        public event OnMinePlaced MinePlaced;

        private void OnEnable()
        {
            // Reset launch force and UI when enabled
            m_CurrentLaunchForce = m_MinLaunchForce;
            m_AimSlider.value = m_MinLaunchForce;
        }

        private void Start()
        {
            // Initialize weapon stocks
            shellStockData = new WeaponStockData(10, 50, 10);
            mineStockData = new WeaponStockData(1, 3, 1);

            weaponStockDictionary = new Dictionary<string, WeaponStockData>
            {
                { "Shell", shellStockData },
                { "Mine", mineStockData }
            };

            foreach (var weapon in weaponStockDictionary)
            {
                weapon.Value.InitializeWeaponNumber();
                WeaponStockChanged?.Invoke(weapon.Key, weapon.Value.CurrentWeaponNumber);
            }

            if (photonView.IsMine)
            {
                m_FireButton = "Fire1";
                minePlaceButton = "PlaceMine1";
            }

            m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
        }
        public void Initialize(int playerNumber)
        {
            m_PlayerNumber = playerNumber; // TankManager などから設定される
        }
        private void Update()
        {
            if (!photonView.IsMine) return; // 自分のタンク以外は操作しない

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
                    m_ChargeSpeed = -m_ChargeSpeed;
                }
                else if (m_CurrentLaunchForce <= m_MinLaunchForce)
                {
                    m_CurrentLaunchForce = m_MinLaunchForce;
                    m_ChargeSpeed = -m_ChargeSpeed;
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
            m_Fired = true;

            weaponStockDictionary["Shell"].DecrementWeaponNumber();
            WeaponStockChanged?.Invoke("Shell", weaponStockDictionary["Shell"].CurrentWeaponNumber);

            object[] initData = new object[] { photonView.ViewID, m_CurrentLaunchForce }; // m_CurrentLaunchForce を追加
            GameObject shellObject = PhotonNetwork.Instantiate("CompleteShell", m_FireTransform.position, m_FireTransform.rotation, 0, initData);
            Rigidbody shellInstance = shellObject.GetComponent<Rigidbody>();

            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play();

            m_CurrentLaunchForce = m_MinLaunchForce;
            m_Fired = false;
        }

        private void PlaceMine()
        {
            if (weaponStockDictionary["Mine"].CurrentWeaponNumber <= 0)
            {
                Debug.Log("No mines available to place.");
                return;
            }

            object[] initData = new object[] { photonView.ViewID };
            PhotonNetwork.Instantiate("MinePrefab", transform.position, Quaternion.identity, 0, initData);

            weaponStockDictionary["Mine"].DecrementWeaponNumber();
            WeaponStockChanged?.Invoke("Mine", weaponStockDictionary["Mine"].CurrentWeaponNumber);

            MinePlaced?.Invoke();
        }
        public void GainingWeaponNumber(string weaponName)
        {
            if (weaponStockDictionary.ContainsKey(weaponName))
            {
                weaponStockDictionary[weaponName].GainingWeaponNumber();
                WeaponStockChanged?.Invoke(weaponName, weaponStockDictionary[weaponName].CurrentWeaponNumber);

                // 全クライアントにストック数の更新を通知
                photonView.RPC("UpdateWeaponStock", RpcTarget.Others, weaponName, weaponStockDictionary[weaponName].CurrentWeaponNumber);
            }
            else
            {
                Debug.LogWarning($"Weapon '{weaponName}' not found in stock dictionary.");
            }
        }

        [PunRPC]
        public void UpdateWeaponStock(string weaponName, int currentStock)
        {
            if (weaponStockDictionary.ContainsKey(weaponName))
            {
                weaponStockDictionary[weaponName].SetWeaponNumber(currentStock);
                WeaponStockChanged?.Invoke(weaponName, currentStock);
            }
        }

    }
}
