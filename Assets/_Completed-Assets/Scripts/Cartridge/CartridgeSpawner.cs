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

        public void SpawnCartridge(CartridgeData data)
        {
            if (data.cartridgePrefab == null)
            {
                Debug.LogError("cartridgePrefabがnullです。Instantiateできません。");
                return;
            }

            Vector3 randomPosition = new Vector3(
                Random.Range(-40f, 40f), // X座標の範囲
                1f,                      // Y座標（固定）
                Random.Range(-40f, 40f)  // Z座標の範囲
            );

            string prefabName = data.cartridgePrefab.name;

            // プレハブ名がCustomPrefabPoolに登録されているか確認
            CustomPrefabPool customPool = PhotonNetwork.PrefabPool as CustomPrefabPool;
            if (customPool == null)
            {
                Debug.LogError("PhotonNetwork.PrefabPool is not a CustomPrefabPool.");
                return;
            }

            if (!customPool.ContainsPrefab(prefabName))
            {
                Debug.LogError($"Prefab with name '{prefabName}' not found in CustomPrefabPool.");
                return;
            }

            GameObject cartridge = PhotonNetwork.Instantiate(prefabName, randomPosition, Quaternion.identity);
            if (cartridge != null)
            {
                Debug.Log($"Instantiated cartridge prefab '{prefabName}' at position {randomPosition}");
            }
            else
            {
                Debug.LogError($"Failed to instantiate cartridge prefab '{prefabName}'");
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

        // 定期的にSpawnCartridgeを呼び出すコルーチン
        private IEnumerator SpawnRoutine(CartridgeData data)
        {
            while (true)
            {
                SpawnCartridge(data);
                yield return new WaitForSeconds(data.spawnFrequency); // CartridgeDataから頻度を取得
            }
        }

        private void Start()
        {
            gameManager = FindObjectOfType<GameManager>();
            if (cartridgeData == null || cartridgeData.cartridgePrefab == null)
            {
                Debug.LogError("cartridgeDataまたはcartridgePrefabが設定されていません。処理を中断します。");
                return;
            }

            if (gameManager != null)
            {
                gameManager.GameStateChanged += HandleGameStateChanged;
            }
            else
            {
                Debug.LogError("GameManagerが見つかりません。");
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
