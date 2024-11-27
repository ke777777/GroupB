using System;
using UnityEngine;
using System.Collections;
using Photon.Pun;

namespace Complete
{
    [Serializable]
    public class TankManager
    {
        // This class is to manage various settings on a tank.
        // It works with the GameManager class to control how the tanks behave
        // and whether or not players have control of their tank in the
        // different phases of the game.

        public Color m_PlayerColor;                             // This is the color this tank will be tinted.
        public Transform m_SpawnPoint;                          // The position and direction the tank will have when it spawns.
        [HideInInspector] public int m_PlayerNumber;            // This specifies which player this the manager for.
        [HideInInspector] public string m_ColoredPlayerText;    // A string that represents the player with their number colored to match their tank.
        [HideInInspector] public GameObject m_Instance;         // A reference to the instance of the tank when it is created.
        [HideInInspector] public int m_Wins;                    // The number of wins this player has so far.


        private TankMovement m_Movement;                        // Reference to tank's movement script, used to disable and enable control.
        private TankShooting m_Shooting;                        // Reference to tank's shooting script, used to disable and enable control.
        private GameObject m_CanvasGameObject;                  // Used to disable the world space UI during the Starting and Ending phases of each round.
        public Transform m_TurretTransform;                     // 砲塔のTransformを格納するプロパティ

        public delegate void OnWeaponStockChanged(int playerNumber, string weaponName, int currentStock);
        public event OnWeaponStockChanged WeaponStockChanged;   // 砲弾所持数が変化したときのイベント
        public void Setup()
        {
            if (m_Instance == null)
            {
                Debug.LogWarning("Tank instance is null in TankManager.Setup");
                return;
            }
            // Get references to the components.
            m_Movement = m_Instance.GetComponent<TankMovement>();
            m_Shooting = m_Instance.GetComponent<TankShooting>();
            m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;
            m_TurretTransform = m_Instance.transform.Find("TankRenderers/TankTurret"); // 砲塔のTransformを取得

            // Set the player numbers to be consistent across the scripts.
            if (m_Movement != null) m_Movement.m_PlayerNumber = m_PlayerNumber;
            if (m_Shooting != null)
            {
                m_Shooting.Initialize(m_PlayerNumber); // TankShooting にプレイヤー番号を設定
            }


            // Create a string using the correct color that says 'PLAYER 1' etc based on the tank's color and the player's number.
            m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";


            // Get all of the renderers of the tank.
            MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

            // Go through all the renderers...
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = m_PlayerColor;
            }
            if (m_Shooting != null)
            {
                m_Shooting.WeaponStockChanged += HandleWeaponStockChanged;
                m_Shooting.MinePlaced += HandleMinePlaced;
            }
        }
        private void HandleWeaponStockChanged(string weaponName, int currentStock)
        {
            WeaponStockChanged?.Invoke(m_PlayerNumber, weaponName, currentStock); //プレイヤー番号と砲弾の所持数の通知を行う
        }

        private IEnumerator TemporarilyStopMovement()
        {
            DisableControl();
            yield return new WaitForSeconds(2.0f); // 2秒間動きを止める
            EnableControl();
        }
        private void HandleMinePlaced()
        {
            if (m_Instance == null || !m_Instance.GetComponent<PhotonView>().IsMine)
                return;

            MonoBehaviour behaviour = m_Instance.GetComponent<MonoBehaviour>();
            behaviour?.StartCoroutine(TemporarilyStopMovement());
        }
        // Used during the phases of the game where the player shouldn't be able to control their tank.
        public void DisableControl()
        {
            if (m_Instance == null)
            {
                Debug.LogWarning("Tank instance is null in DisableControl");
                return;
            }

            PhotonView photonView = m_Instance.GetComponent<PhotonView>();
            if (photonView != null && !photonView.IsMine)
                return;

            if (m_Movement != null) m_Movement.enabled = false;
            if (m_Shooting != null) m_Shooting.enabled = false;
            if (m_CanvasGameObject != null) m_CanvasGameObject.SetActive(false);
        }



        // Used during the phases of the game where the player should be able to control their tank.
        public void EnableControl()
        {
            if (m_Instance == null)
            {
                Debug.LogWarning("Tank instance is null in EnableControl");
                return;
            }

            PhotonView photonView = m_Instance.GetComponent<PhotonView>();
            if (photonView != null && !photonView.IsMine)
                return;

            if (m_Movement != null) m_Movement.enabled = true;
            if (m_Shooting != null) m_Shooting.enabled = true;
            if (m_CanvasGameObject != null) m_CanvasGameObject.SetActive(true);
        }



        // Used at the start of each round to put the tank into it's default state.
        public void Reset()
        {
            if (m_Instance == null)
            {
                Debug.LogWarning("Tank instance is null in TankManager.Reset");
                return;
            }

            PhotonView photonView = m_Instance.GetComponent<PhotonView>();
            if (photonView != null && !photonView.IsMine)
                return;

            m_Instance.transform.position = m_SpawnPoint.position;
            m_Instance.transform.rotation = m_SpawnPoint.rotation;

            m_Instance.SetActive(true);
        }
    }
}