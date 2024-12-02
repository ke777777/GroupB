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
        [SerializeField] private Text myPlayerName;
        [SerializeField] private Text opponentPlayerName;
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
            while (true)
            {
                var localTank = gameManager.m_Tanks.FirstOrDefault(t => t.m_Instance != null && t.m_Instance.GetComponent<PhotonView>().IsMine);
                if (localTank != null && localTank.m_PlayerNumber > 0)
                {
                    myPlayerNumber = localTank.m_PlayerNumber;
                    localTank.WeaponStockChanged += HandleWeaponStockChanged;

                    // 相手のタンクを取得
                    var opponentTank = gameManager.m_Tanks.FirstOrDefault(t => t.m_PlayerNumber != myPlayerNumber && t.m_Instance != null);
                    if (opponentTank != null)
                    {
                        opponentPlayerNumber = opponentTank.m_PlayerNumber;
                    }

                    // プレイヤーナンバーをHUDに表示
                    UpdatePlayerNumbers();

                    // TankShootingから現在の武器所持数を取得してUIを更新
                    var tankShooting = localTank.m_Instance.GetComponent<TankShooting>();
                    if (tankShooting != null)
                    {
                        var weaponStocks = tankShooting.GetWeaponStocks();
                        foreach (var weapon in weaponStocks)
                        {
                            HandleWeaponStockChanged(localTank.m_PlayerNumber, weapon.Key, weapon.Value.CurrentWeaponNumber);
                        }
                    }

                    break;
                }

                yield return new WaitForSeconds(0.5f);
            }
        }

        private void UpdatePlayerNumbers()
        {
            // 自分のプレイヤーナンバーを左側に表示
            if (myPlayerName != null)
            {
                myPlayerName.text = $"Player {myPlayerNumber}";
                myPlayerName.color = GetPlayerColor(myPlayerNumber);
            }

            // 相手のプレイヤーナンバーを右側に表示
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
