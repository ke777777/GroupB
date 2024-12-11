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
            // ランダムなX, Z座標を取得
            float randomX = Random.Range(-40f, 40f);
            float randomZ = Random.Range(-40f, 40f);

            Vector3 randomPosition = new Vector3(randomX, 50f, randomZ); // Yを高い位置から始める
            Ray ray = new Ray(randomPosition, Vector3.down); // 下方向にレイを飛ばす

            // レイキャストで地面の位置を探す
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, groundLayer))
            {
                // ヒットした地形の法線を取得して回転を調整
                Vector3 flatPosition = hitInfo.point;
                Vector3 normal = hitInfo.normal;

                // 地形が平坦でない場合はスキップ（法線が上向きに近い場合のみ生成）
                if (Vector3.Angle(normal, Vector3.up) > 10f) // 平坦である角度のしきい値（10度以内）
                {
                    return Vector3.zero; // 不適切な位置
                }

                return flatPosition;
            }

            return Vector3.zero; // 地形が見つからなければ無効な値を返す
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
