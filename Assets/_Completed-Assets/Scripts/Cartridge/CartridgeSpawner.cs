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

            GameObject prefab = Resources.Load<GameObject>(data.cartridgePrefab.name);

            if (prefab != null)
            {
                GameObject cartridge = PhotonNetwork.Instantiate(prefab.name, randomPosition, Quaternion.identity);
                /*if (cartridge != null)
                {
                    Debug.Log($"Instantiated cartridge prefab '{prefab.name}' at position {randomPosition}");
                }
                else
                {
                    Debug.LogError($"Failed to instantiate cartridge prefab '{prefab.name}'");
                }*/
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
