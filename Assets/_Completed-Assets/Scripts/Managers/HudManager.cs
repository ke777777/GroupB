using UnityEngine;
using System.Collections;
using System.Linq;
using Photon.Pun;
namespace Complete
{
    public class HudManager : MonoBehaviour
    {
        [SerializeField] private PlayerStockArea playerStockArea;
        [SerializeField] private GameManager gameManager;

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
                if (localTank != null)
                {
                    localTank.WeaponStockChanged += HandleWeaponStockChanged;
                    // TankShooting‚©‚çŒ»İ‚Ì•ŠíŠ”‚ğæ“¾‚µ‚ÄUI‚ğXV
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
