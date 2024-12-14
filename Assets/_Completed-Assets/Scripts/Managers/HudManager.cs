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
        [SerializeField] private MySQLRequest mySQLRequest; // MySQLRequest���Q��
        [SerializeField] private Text myPlayerName;
        [SerializeField] private Text opponentPlayerName;
        [SerializeField] private Text myPlayerScoreText; // �����̃X�R�A�\���p
        [SerializeField] private Text opponentPlayerScoreText; // ����̃X�R�A�\���p
        private int myPlayerNumber;
        private int opponentPlayerNumber;

        private void OnEnable()
        {
            if (gameManager != null)
            {
                gameManager.GameStateChanged += HandleGameStateChanged;

                StartCoroutine(SubscribeToLocalTank());
            }
        }
        private IEnumerator SubscribeToLocalTank()
        {
            // ���[�J���^���N�̏���������������܂ő҂�
            TankManager myTank = null;
            while (myTank == null || myTank.m_Instance == null)
            {
                myTank = gameManager.m_Tanks.FirstOrDefault(t => t.m_Instance != null && t.m_Instance.GetComponent<PhotonView>().IsMine);
                yield return new WaitForSeconds(0.1f);
            }

            // ���[�J���v���C���[�ԍ����擾
            myPlayerNumber = myTank.m_PlayerNumber;

            // ����^���N�������������܂ő҂�
            TankManager opponentTank = null;
            while (opponentTank == null || opponentTank.m_Instance == null || opponentTank.m_PlayerNumber == 0)
            {
                opponentTank = gameManager.m_Tanks.FirstOrDefault(t => t.m_PlayerNumber != myPlayerNumber && t.m_Instance != null);
                yield return new WaitForSeconds(0.1f);
            }

            opponentPlayerNumber = opponentTank.m_PlayerNumber;

            // �v���C���[�f�[�^��HUD�ɕ\��
            if (mySQLRequest != null)
            {
                // �����̃v���C���[�f�[�^���擾
                mySQLRequest.GetPlayerData(myPlayerNumber, UpdateMyPlayerHUD, HandleError);

                // ����̃v���C���[�f�[�^���擾
                mySQLRequest.GetPlayerData(opponentPlayerNumber, UpdateOpponentPlayerHUD, HandleError);
            }


            // �v���C���[�i���o�[��HUD�ɕ\��
            //UpdatePlayerNumbers();

            myTank.WeaponStockChanged += HandleWeaponStockChanged;

            // TankShooting���猻�݂̕��폊�������擾����UI���X�V
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

        private void UpdateMyPlayerHUD(PlayerData data)
        {
            if (myPlayerName != null)
            {
                myPlayerName.text = $"Name: {data.user_name}";
                myPlayerName.color = GetPlayerColor(myPlayerNumber);

            }

            if (myPlayerScoreText != null)
            {
                myPlayerScoreText.text = $"Wins: {data.n_win} | Losses: {data.n_loss}";
            }
        }
        private void UpdateOpponentPlayerHUD(PlayerData data)
        {
            if (opponentPlayerName != null)
            {
                opponentPlayerName.text = $"Name: {data.user_name}";
            }

            if (opponentPlayerScoreText != null)
            {
                opponentPlayerScoreText.text = $"Wins: {data.n_win} | Losses: {data.n_loss}";
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
            // �����̃v���C���[�i���o�[�������ɕ\��
            if (myPlayerName != null)
            {
                myPlayerName.text = $"Player {myPlayerNumber}";
                myPlayerName.color = GetPlayerColor(myPlayerNumber);
            }

            // ����̃v���C���[�i���o�[���E���ɕ\��
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
