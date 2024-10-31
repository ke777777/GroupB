using UnityEngine;
using Complete;
namespace Complete
{
    public class HudManager : MonoBehaviour
    {
        [SerializeField] private PlayerStockArea player1StockArea;
        [SerializeField] private PlayerStockArea player2StockArea;
        [SerializeField] private GameManager gameManager; // GameManager�̎Q��

        private void OnEnable()
        {
            if (gameManager != null)
            {
                gameManager.GameStateChanged += HandleGameStateChanged;

                foreach (var tank in gameManager.m_Tanks) //�C�x���g��o�^
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

                foreach (var tank in gameManager.m_Tanks)//�C�x���g������
                {
                    tank.cannonballStockChanged -= HandleWeaponStockChanged;
                }
            }
        }

        private void HandleGameStateChanged(GameManager.GameState newState)
        {
            bool isGameActive = (newState == GameManager.GameState.RoundPlaying);

            // Player1��Player2��HUD�\���𐧌�
            player1StockArea.gameObject.SetActive(isGameActive);
            player2StockArea.gameObject.SetActive(isGameActive);
        }
        private void HandleWeaponStockChanged(int playerNumber, int currentStock)
        {
            if (playerNumber == 1) // �v���C���[1�̑Ή�����PlayerStockArea��UpdatePlayerStockArea���Ăяo��
            {
                player1StockArea.UpdatePlayerStockArea(currentStock);
            }
            else if (playerNumber == 2) // �v���C���[2�̑Ή�����PlayerStockArea��UpdatePlayerStockArea���Ăяo��
            {
                player2StockArea.UpdatePlayerStockArea(currentStock);
            }
        }
    }
}
