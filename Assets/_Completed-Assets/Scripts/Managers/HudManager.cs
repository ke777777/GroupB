using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using Photon.Pun;
namespace Complete
{
    public class HudManager : MonoBehaviour
    {
        [SerializeField] private PlayerStockArea playerStockArea;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private MySQLRequest mySQLRequest; // MySQLRequest???
        [SerializeField] private Text myPlayerName;
        [SerializeField] private Text opponentPlayerName;

        private int myPlayerNumber;
        private int opponentPlayerNumber;


        private void OnEnable()
        {
            RenameButton.OnUserNameUpdated += UpdateMyPlayerHUD;
            if (gameManager != null)
            {
                gameManager.GameStateChanged += HandleGameStateChanged;

                StartCoroutine(SubscribeToLocalTank());
            }
        }
        private IEnumerator SubscribeToLocalTank()
        {
            // ????????????????????
            TankManager myTank = null;
            while (myTank == null || myTank.m_Instance == null)
            {
                myTank = gameManager.m_Tanks.FirstOrDefault(t => t.m_Instance != null && t.m_Instance.GetComponent<PhotonView>().IsMine);
                yield return new WaitForSeconds(0.1f);
            }

            // ??????????????
            myPlayerNumber = myTank.m_PlayerNumber;
            PhotonView myTankView = myTank.m_Instance.GetComponent<PhotonView>();
            int myUserId = 0;
            if (myTankView.InstantiationData != null && myTankView.InstantiationData.Length > 2)
            {
                myUserId = (int)myTankView.InstantiationData[2];
            }

            // ??????????????
            if (myUserId != 0 && mySQLRequest != null)
            {
                mySQLRequest.GetPlayerData(myUserId, UpdateMyPlayerHUD, (error) =>
                {
                    Debug.LogError($"Failed to fetch player name for user {myUserId}: {error}");
                });
            }



            // ????????????????
            TankManager opponentTank = null;
            while (opponentTank == null || opponentTank.m_Instance == null || opponentTank.m_PlayerNumber == 0)
            {
                opponentTank = gameManager.m_Tanks.FirstOrDefault(t => t.m_PlayerNumber != myPlayerNumber && t.m_Instance != null);
                yield return new WaitForSeconds(0.1f);
            }

            opponentPlayerNumber = opponentTank.m_PlayerNumber;

            PhotonView opponentView = opponentTank.m_Instance.GetComponent<PhotonView>();
            int opponentUserId = 0;
            if (opponentView.InstantiationData != null && opponentView.InstantiationData.Length > 2)
            {
                opponentUserId = (int)opponentView.InstantiationData[2];
            }

            if (opponentUserId != 0 && mySQLRequest != null)
            {
                mySQLRequest.GetPlayerData(opponentUserId, UpdateOpponentPlayerHUD, (error) =>
                {
                    Debug.LogError($"Failed to fetch player name for opponent: {error}");
                });
            }

            // ??????????HUD???
            //UpdatePlayerNumbers();

            myTank.WeaponStockChanged += HandleWeaponStockChanged;

            // TankShooting???????????????UI???
            var tankShooting = myTank.m_Instance.GetComponent<TankShooting>();
            if (tankShooting != null)
            {
                var weaponStocks = tankShooting.GetWeaponStocks();
                foreach (var weapon in weaponStocks)
                {
                    HandleWeaponStockChanged(myTank.m_PlayerNumber, weapon.Key, weapon.Value.CurrentWeaponNumber);
                }
            }
            myTank.WeaponStockChanged += HandleWeaponStockChanged;
        }

        private void UpdateMyPlayerHUD(string userName)
        {
            if (myPlayerName != null)
            {
                myPlayerName.text = $"Name: {userName}";
                myPlayerName.color = GetPlayerColor(myPlayerNumber);
            }
        }

        private void UpdateOpponentPlayerHUD(string userName)
        {
            if (opponentPlayerName != null)
            {
                opponentPlayerName.text = $"Name: {userName}";
                opponentPlayerName.color = GetPlayerColor(opponentPlayerNumber);
            }
        }
        private void HandleError(string error)
        {
            Debug.LogError($"Failed to fetch player data: {error}");
        }

        private IEnumerator WaitForPlayerNumbers()
        {
            while (PhotonNetwork.PlayerList.All(p => !p.CustomProperties.ContainsKey("PlayerNumber")))
            {
                yield return new WaitForSeconds(0.1f);
            }

            var myTank = gameManager.m_Tanks.FirstOrDefault(t => t.m_Instance != null && t.m_Instance.GetComponent<PhotonView>().IsMine);
            if (myTank != null)
            {
                myPlayerNumber = myTank.m_PlayerNumber;
            }

            var opponentTank = gameManager.m_Tanks.FirstOrDefault(t => t.m_PlayerNumber != myPlayerNumber && t.m_Instance != null);
            if (opponentTank != null)
            {
                opponentPlayerNumber = opponentTank.m_PlayerNumber;
            }

            UpdatePlayerNumbers();
        }
        private void UpdatePlayerNumbers()
        {
            // ??????????????????
            if (myPlayerName != null)
            {
                myPlayerName.text = $"Player {myPlayerNumber}";
                myPlayerName.color = GetPlayerColor(myPlayerNumber);
            }

            // ??????????????????
            if (opponentPlayerName != null)
            {
                opponentPlayerName.text = $"Player {opponentPlayerNumber}";
                opponentPlayerName.color = GetPlayerColor(opponentPlayerNumber);
            }
        }

        private Color GetPlayerColor(int playerNumber)
        {
            if (playerNumber == 1)
                return Color.blue;
            else if (playerNumber == 2)
                return Color.red;
            else
                return Color.white;
        }

        private void OnDisable()
        {
            RenameButton.OnUserNameUpdated -= UpdateMyPlayerHUD;
            if (gameManager != null)
            {
                gameManager.GameStateChanged -= HandleGameStateChanged;
                var localTank = gameManager.m_Tanks.FirstOrDefault(t => t.m_Instance != null && t.m_Instance.GetComponent<PhotonView>().IsMine);
                if (localTank != null)
                {
                    localTank.WeaponStockChanged -= HandleWeaponStockChanged;
                }
            }
        }

        private void HandleGameStateChanged(GameManager.GameState newState)
        {
            bool isGameActive = (newState == GameManager.GameState.RoundPlaying);
            playerStockArea.gameObject.SetActive(isGameActive);
        }
        private void HandleWeaponStockChanged(int playerNumber, string weaponName, int currentStock)
        {
            playerStockArea.UpdatePlayerStockArea(weaponName, currentStock);
        }
    }
}
