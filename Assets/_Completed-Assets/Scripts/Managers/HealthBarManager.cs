using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using Photon.Pun;

namespace Complete
{
    public class HealthBarManager : MonoBehaviour
    {
        [SerializeField] private Slider player1HealthSlider; // プレイヤー1の体力バー
        [SerializeField] private Image player1FillImage;     // プレイヤー1の体力バーのFill Image
        [SerializeField] private Slider player2HealthSlider; // プレイヤー2の体力バー
        [SerializeField] private Image player2FillImage;     // プレイヤー2の体力バーのFill Image
        [SerializeField] private Color fullHealthColor = Color.green; // 体力最大時の色
        [SerializeField] private Color zeroHealthColor = Color.red;   // 体力最小時の色
        [SerializeField] private GameManager gameManager;    // ゲームマネージャーの参照

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
            // GameManager の初期化待ち
            while (gameManager == null || gameManager.m_Tanks == null || gameManager.m_Tanks.Count == 0)
            {
                Debug.Log("Waiting for GameManager and Tanks to be initialized...");
                yield return new WaitForSeconds(0.5f); // 次のフレームまで待機
            }

            // 各タンクのインスタンス生成待ち
            bool allInstancesInitialized = false;
            while (!allInstancesInitialized)
            {
                allInstancesInitialized = true;
                foreach (var tank in gameManager.m_Tanks)
                {
                    if (tank.m_Instance == null)
                    {
                        Debug.Log($"Tank {tank.m_PlayerNumber} instance is not initialized yet. Retrying...");
                        allInstancesInitialized = false;
                        break;
                    }
                }
                yield return new WaitForSeconds(0.5f);
            }

            Debug.Log("All Tank instances have been initialized. Linking health...");

            // 各タンクのヘルス状態をリンク
            foreach (var tank in gameManager.m_Tanks)
            {
                if (tank.m_Instance == null)
                {
                    Debug.LogError($"Tank {tank.m_PlayerNumber} instance is null. Skipping...");
                    continue; // インスタンスが存在しない場合はスキップ
                }

                var tankHealth = tank.m_Instance.GetComponent<TankHealth>();
                if (tankHealth == null)
                {
                    Debug.LogError($"TankHealth component not found on Tank {tank.m_PlayerNumber}. Skipping...");
                    continue; // TankHealth コンポーネントが見つからない場合はスキップ
                }

                // ヘルス変更イベントにサブスクライブ
                tankHealth.OnHealthChanged += HandleHealthChanged;

                // 初期状態を処理
                HandleHealthChanged(tankHealth.CurrentHealth, tankHealth.StartingHealth, tank.m_PlayerNumber);

                Debug.Log($"Linked health for Tank {tank.m_PlayerNumber}.");
            }

            Debug.Log("Health linking completed for all Tanks.");
        }


        private void HandleHealthChanged(float currentHealth, float maxHealth, int playerNumber)
        {
            float healthPercentage = currentHealth / maxHealth;

            if (playerNumber == 1 && player1HealthSlider != null)
            {

                player1HealthSlider.maxValue = maxHealth;
                player1HealthSlider.value = currentHealth;

                if (player1FillImage != null)
                {
                    // 体力割合に応じて色を変化させる
                    player1FillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, healthPercentage);
                }
            }
            else if (playerNumber == 2 && player2HealthSlider != null)
            {
                player2HealthSlider.maxValue = maxHealth;
                player2HealthSlider.value = currentHealth;

                if (player2FillImage != null)
                {
                    // 体力割合に応じて色を変化させる
                    player2FillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, healthPercentage);
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
                        tankHealth.OnHealthChanged -= HandleHealthChanged;
                    }
                }
            }
        }
    }
}
