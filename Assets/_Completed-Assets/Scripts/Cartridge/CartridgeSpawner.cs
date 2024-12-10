using System.Collections;
using UnityEngine;

public class CartridgeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cartridgePrefab; // Cartridgeプレハブ
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(50f, 0f, 50f); // 生成範囲
    [SerializeField] private float spawnInterval = 5f; // 生成間隔

    private Coroutine spawnRoutine;

    private void Start()
    {
        // GameManagerのイベントを監視
        Complete.GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDestroy()
    {
        if (Complete.GameManager.Instance != null)
        {
            Complete.GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        }
    }

    private void HandleGameStateChanged(Complete.GameManager.GameState newState)
    {
        if (newState == Complete.GameManager.GameState.RoundPlaying)
        {
            // ゲームプレイ中にコルーチンを開始
            spawnRoutine = StartCoroutine(SpawnRoutine());
        }
        else
        {
            // ゲームがプレイ中でなくなったらコルーチンを停止
            if (spawnRoutine != null)
            {
                StopCoroutine(spawnRoutine);
                spawnRoutine = null;
            }
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnCartridge();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnCartridge()
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            1.0f, // 地面の高さ
            Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
        );

        Instantiate(cartridgePrefab, randomPosition, Quaternion.identity);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.5f); // スポーン位置に緑の球を描画
    }

}
