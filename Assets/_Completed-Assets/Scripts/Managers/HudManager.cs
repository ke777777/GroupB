using UnityEngine;

namespace Complete
{
    public class HudManager : MonoBehaviour
    {
        [SerializeField] private PlayerStockArea player1StockArea;
        [SerializeField] private PlayerStockArea player2StockArea;
        [SerializeField] private GameManager gameManager;

        private void OnEnable()
        {
            if (gameManager != null)
            {
                gameManager.GameStateChanged += HandleGameStateChanged;

                foreach (var tank in gameManager.m_Tanks)
                {
                    tank.WeaponStockChanged += HandleWeaponStockChanged;
                }
            }
        }

        private void OnDisable()
        {
            if (gameManager != null)
            {
                gameManager.GameStateChanged -= HandleGameStateChanged;

                foreach (var tank in gameManager.m_Tanks)
                {
                    tank.WeaponStockChanged -= HandleWeaponStockChanged;
                }
            }
        }

        private void HandleGameStateChanged(GameManager.GameState newState)
        {
            bool isGameActive = (newState == GameManager.GameState.RoundPlaying);

            player1StockArea.gameObject.SetActive(isGameActive);
            player2StockArea.gameObject.SetActive(isGameActive);
        }

        private void HandleWeaponStockChanged(int playerNumber, string weaponName, int currentStock)
        {
            if (playerNumber == 1)
            {
                player1StockArea.UpdatePlayerStockArea(weaponName, currentStock);
            }
            else if (playerNumber == 2)
            {
                player2StockArea.UpdatePlayerStockArea(weaponName, currentStock);
            }
        }
    }
}
