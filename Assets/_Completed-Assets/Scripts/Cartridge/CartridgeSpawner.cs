using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace Complete
{
    public class CartridgeSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject shellCartridgePrefab;
        private GameManager gameManager;
        private Coroutine spawnCoroutine;

        // ランダムな位置にプレハブを生成するメソッド
        public void SpawnCartridge()
        {
            if (shellCartridgePrefab == null)
            {
                Debug.LogError("shellCartridgePrefabがnullです。Instantiateできません。");
                return;
            }

            Vector3 randomPosition = new Vector3(
                Random.Range(-10f, 10f),  // X座標の範囲
                1f,                       // Y座標（固定）
                Random.Range(-10f, 10f)   // Z座標の範囲
            );
            Instantiate(shellCartridgePrefab, randomPosition, Quaternion.identity);
        }

        private void HandleGameStateChanged(GameManager.GameState newState)
        {
            if (newState == GameManager.GameState.RoundPlaying)
            {
                if (spawnCoroutine == null)
                {
                    spawnCoroutine = StartCoroutine(SpawnRoutine());
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
        private IEnumerator SpawnRoutine()
        {
            while (true)
            {
                SpawnCartridge(); // Cartridgeを生成
                yield return new WaitForSeconds(3f);  // 一定時間待機（3秒間隔で生成）
            }
        }

        private void Start()
        {
            // GameManagerオブジェクトを取得
            gameManager = FindObjectOfType<GameManager>();

            gameManager.GameStateChanged += HandleGameStateChanged;
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
