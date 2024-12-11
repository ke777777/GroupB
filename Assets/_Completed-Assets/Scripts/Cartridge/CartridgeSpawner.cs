using UnityEngine;
using System.Collections;
using Photon.Pun;

namespace Complete
{
    public class CartridgeSpawner : MonoBehaviour
    {
        [SerializeField] private CartridgeData cartridgeData;
        private GameManager gameManager;
        private Coroutine spawnCoroutine;
        [SerializeField] private LayerMask groundLayer; // 地面を判定するレイヤー


        public void SpawnCartridge(CartridgeData data)
        {
            if (data.cartridgePrefab == null)
            {
                Debug.LogError("cartridgePrefabがnullです。Instantiateできません。");
                return;
            }

            Vector3 spawnPosition = FindFlatPosition();

            /* Vector3 randomPosition = new Vector3(
                Random.Range(-40f, 40f), // X座標の範囲
                1f,                      // Y座標（固定）
                Random.Range(-40f, 40f)  // Z座標の範囲
            );
            */

            GameObject prefab = Resources.Load<GameObject>(data.cartridgePrefab.name);
            if (prefab != null)
            {
                PhotonNetwork.Instantiate(prefab.name, spawnPosition, Quaternion.identity);
            }
            else
            {
                Debug.LogError($"Prefab '{data.cartridgePrefab.name}' not found in Resources.");
            }
        }


        private void HandleGameStateChanged(GameManager.GameState newState)
        {
            if (newState == GameManager.GameState.RoundPlaying)
            {
                if (spawnCoroutine == null)
                {
                    spawnCoroutine = StartCoroutine(SpawnRoutine(cartridgeData));
                }
            }
            else
            {
                if (spawnCoroutine != null)
                {
                    StopCoroutine(spawnCoroutine);
                    spawnCoroutine = null;
                }
            }
        }

        private Vector3 FindFlatPosition()
        {
            const int maxAttempts = 10; // 位置を探す試行回数の上限
            for (int i = 0; i < maxAttempts; i++)
            {
                // ランダムなX, Z座標を取得
                float randomX = Random.Range(-40f, 40f);
                float randomZ = Random.Range(-40f, 40f);
                Vector3 groundCheckPosition = new Vector3(randomX, 50f, randomZ);

                // 下方向にレイキャストして地面を探す
                if (Physics.Raycast(groundCheckPosition, Vector3.down, out RaycastHit hitInfo, Mathf.Infinity, groundLayer))
                {
                    // 地面オブジェクトのチェック
                    if (hitInfo.collider.CompareTag("Ground"))
                    {
                        Vector3 spawnPosition = hitInfo.point;
                        spawnPosition.y = 1f; // カートリッジをy=1に設定

                        // 上方向にレイキャストして障害物がないことを確認
                        if (!Physics.Raycast(spawnPosition + Vector3.up * 0.5f, Vector3.up, 1f))
                        {
                            return spawnPosition;
                        }
                    }
                }
            }
            return Vector3.zero; // 有効な位置が見つからない場合
        }

        private IEnumerator SpawnRoutine(CartridgeData data)
        {
            while (true)
            {
                SpawnCartridge(data);
                yield return new WaitForSeconds(data.spawnFrequency);
            }
        }

        private void Start()
        {
            gameManager = FindObjectOfType<GameManager>();

            if (gameManager != null)
            {
                gameManager.GameStateChanged += HandleGameStateChanged;
            }
        }

        private void OnDestroy()
        {
            if (gameManager != null)
            {
                gameManager.GameStateChanged -= HandleGameStateChanged;
            }
        }
    }
}
