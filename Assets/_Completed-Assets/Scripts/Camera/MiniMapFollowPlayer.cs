using UnityEngine;

public class MiniMapFollowPlayer : MonoBehaviour
{
    public int targetPlayerNumber = 1; // 追跡するプレイヤーの番号
    private Transform playerTransform; // プレイヤーのTransform
    private bool isPlayerFound = false; // プレイヤーが見つかったかどうかのフラグ
    public float retryInterval = 0.5f; // プレイヤーを再検索する間隔

    private void Start()
    {
        StartCoroutine(FindPlayer());
    }

    private void Update()
    {
        // プレイヤーが見つかった場合に追尾処理を実行
        if (isPlayerFound && playerTransform != null)
        {
            Vector3 newPosition = playerTransform.position;
            newPosition.y = transform.position.y; // 高さを固定
            transform.position = newPosition;
        }
    }

    private System.Collections.IEnumerator FindPlayer()
    {
        while (!isPlayerFound)
        {
            Complete.TankMovement[] players = FindObjectsOfType<Complete.TankMovement>();

            foreach (Complete.TankMovement player in players)
            {
                if (player.m_PlayerNumber == targetPlayerNumber)
                {
                    playerTransform = player.transform;
                    isPlayerFound = true; // プレイヤーを見つけたらフラグを設定
                    Debug.Log($"Player {targetPlayerNumber} found.");
                    yield break;
                }
                else
                {
                    Debug.LogWarning($"Player number mismatch: Expected {targetPlayerNumber}, but found {player.m_PlayerNumber}");
                }
            }

            // プレイヤーが見つからなかった場合、再試行
            Debug.LogWarning($"Player with number {targetPlayerNumber} not found. Retrying...");
            yield return new WaitForSeconds(retryInterval);
        }
    }
}
