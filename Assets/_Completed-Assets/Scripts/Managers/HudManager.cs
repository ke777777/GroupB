using UnityEngine;
using Complete;
namespace Complete
{
    public class HudManager : MonoBehaviour
    {
        [SerializeField] private PlayerStockArea player1StockArea;
        [SerializeField] private PlayerStockArea player2StockArea;
        [SerializeField] private GameManager gameManager; // GameManagerの参照

        private void OnEnable()
        {
            if (gameManager != null)
            {
                gameManager.GameStateChanged += HandleGameStateChanged;

                foreach (var tank in gameManager.m_Tanks) //イベントを登録
                {
                    tank.cannonballStockChanged+= HandleWeaponStockChanged;
                }
            }
        }

        private void OnDisable()
        {
            if (gameManager != null)
            {
                gameManager.GameStateChanged -= HandleGameStateChanged;

                foreach (var tank in gameManager.m_Tanks)//イベントを解除
                {
                    tank.cannonballStockChanged -= HandleWeaponStockChanged;
                }
            }
        }

        private void HandleGameStateChanged(GameManager.GameState newState)
        {
            bool isGameActive = (newState == GameManager.GameState.RoundPlaying);

            // Player1とPlayer2のHUD表示を制御
            player1StockArea.gameObject.SetActive(isGameActive);
            player2StockArea.gameObject.SetActive(isGameActive);
        }
        private void HandleWeaponStockChanged(int playerNumber, int currentStock)
        {
            if (playerNumber == 1) // プレイヤー1の対応するPlayerStockAreaのUpdatePlayerStockAreaを呼び出す
            {
                player1StockArea.UpdatePlayerStockArea(currentStock);
            }
            else if (playerNumber == 2) // プレイヤー2の対応するPlayerStockAreaのUpdatePlayerStockAreaを呼び出す
            {
                player2StockArea.UpdatePlayerStockArea(currentStock);
            }
        }
    }
}
