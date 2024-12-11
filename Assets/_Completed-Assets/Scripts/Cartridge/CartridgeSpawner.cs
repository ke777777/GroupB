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
        private float spawnRange = 40f;
        private float groundCheckHeight = 50f;


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
                GameObject cartridge = PhotonNetwork.Instantiate(prefab.name, spawnPosition, Quaternion.identity);
                Rigidbody rb = cartridge.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = Vector3.zero; // 初期速度をリセット
                    rb.angularVelocity = Vector3.zero; // 回転速度をリセット
                }
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
                float randomX = Random.Range(-spawnRange, spawnRange);
                float randomZ = Random.Range(-spawnRange, spawnRange);
                Vector3 groundCheckPosition = new Vector3(randomX, groundCheckHeight, randomZ);

                // 地面オブジェクトのチェック
                if (Physics.Raycast(groundCheckPosition, Vector3.down, out RaycastHit hitInfo, Mathf.Infinity, groundLayer))
                {
                    // 地面オブジェクトのチェック
                    if (hitInfo.collider.CompareTag("Ground"))
                    {
                        Vector3 spawnPosition = hitInfo.point;
                        spawnPosition.y = 1f; // カートリッジをy=1に固定

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
