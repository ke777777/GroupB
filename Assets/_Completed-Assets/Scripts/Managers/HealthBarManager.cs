using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic; // 追加：Dictionaryを使用するため
using Photon.Pun;

namespace Complete
{
    public class HealthBarManager : MonoBehaviour
    {
        [SerializeField] private Slider myHealthSlider; // 自分の体力バー
        [SerializeField] private Image myFillImage;     // 自分の体力バーのFill Image
        [SerializeField] private Slider opponentHealthSlider; // 相手の体力バー
        [SerializeField] private Image opponentFillImage;     // 相手の体力バーのFill Image
        [SerializeField] private Color fullHealthColor = Color.green; // 体力最大時の色
        [SerializeField] private Color zeroHealthColor = Color.red;   // 体力最小時の色
        [SerializeField] private GameManager gameManager;    // ゲームマネージャーの参照
        private int myPlayerNumber;
        private int opponentPlayerNumber;

        // 追加：イベントハンドラを保持する辞書
        private Dictionary<TankHealth, TankHealth.OnHealthChangedDelegate> healthChangedHandlers = new Dictionary<TankHealth, TankHealth.OnHealthChangedDelegate>();

        private void OnEnable()
        {
            if (gameManager == null || gameManager.m_Tanks == null || gameManager.m_Tanks.Count == 0)
            {
                Debug.LogError("GameManager or tanks data is not properly initialized.");
                return;
            }

            StartCoroutine(WaitForTanksAndLinkHealth());
        }

        private IEnumerator WaitForTanksAndLinkHealth()
        {
            // 自分のタンクを見つける
            TankManager myTank = null;
            while (myTank == null)
            {
                myTank = gameManager.m_Tanks.FirstOrDefault(t => t.m_Instance != null && t.m_Instance.GetComponent<PhotonView>().IsMine);
                if (myTank == null)
                {
                    yield return new WaitForSeconds(0.5f);
                }
            }

            myPlayerNumber = myTank.m_PlayerNumber;

            // 相手のタンクを見つける
            TankManager opponentTank = gameManager.m_Tanks.FirstOrDefault(t => t.m_PlayerNumber != myPlayerNumber);

            if (opponentTank != null)
            {
                opponentPlayerNumber = opponentTank.m_PlayerNumber;
            }
            else
            {
                Debug.LogError("Opponent tank not found.");
                yield break;
            }

            // 自分と相手のタンクのヘルスをリンク
            LinkHealth(myTank, myHealthSlider, myFillImage);
            LinkHealth(opponentTank, opponentHealthSlider, opponentFillImage);
        }

        private void LinkHealth(TankManager tankManager, Slider healthSlider, Image fillImage)
        {
            if (tankManager.m_Instance == null)
            {
                Debug.LogError($"Tank {tankManager.m_PlayerNumber} instance is null.");
                return;
            }

            var tankHealth = tankManager.m_Instance.GetComponent<TankHealth>();
            if (tankHealth == null)
            {
                Debug.LogError($"TankHealth component not found on Tank {tankManager.m_PlayerNumber}.");
                return;
            }

            // イベントハンドラを作成して保持
            TankHealth.OnHealthChangedDelegate handler = (currentHealth, maxHealth, playerNumber) =>
            {
                HandleHealthChanged(currentHealth, maxHealth, playerNumber, healthSlider, fillImage);
            };

            // イベントにハンドラを登録
            tankHealth.OnHealthChanged += handler;

            // ハンドラを辞書に保持
            healthChangedHandlers[tankHealth] = handler;

            // 初期状態を処理
            HandleHealthChanged(tankHealth.CurrentHealth, tankHealth.StartingHealth, tankManager.m_PlayerNumber, healthSlider, fillImage);
        }

        private void HandleHealthChanged(float currentHealth, float maxHealth, int playerNumber, Slider healthSlider, Image fillImage)
        {
            float healthPercentage = currentHealth / maxHealth;

            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;

                if (fillImage != null)
                {
                    fillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, healthPercentage);
                }
            }
        }

        private void OnDisable()
        {
            if (gameManager != null)
            {
                foreach (var tank in gameManager.m_Tanks)
                {
                    if (tank.m_Instance == null)
                        continue;

                    var tankHealth = tank.m_Instance.GetComponent<TankHealth>();
                    if (tankHealth != null)
                    {
                        // イベントハンドラを解除
                        if (healthChangedHandlers.ContainsKey(tankHealth))
                        {
                            tankHealth.OnHealthChanged -= healthChangedHandlers[tankHealth];
                            healthChangedHandlers.Remove(tankHealth);
                        }

                    }
                }
            }
        }
    }
}
