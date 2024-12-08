using UnityEngine;
using Photon.Pun;

public class MiniMapFollowPlayer : MonoBehaviour
{
    private Transform playerTransform; // プレイヤーのTransform
    private bool isPlayerFound = false; // プレイヤーが見つかったかどうかのフラグ


    private void Start()
    {
        StartCoroutine(FindLocalPlayer());
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

    private System.Collections.IEnumerator FindLocalPlayer()
    {
        while (!isPlayerFound)
        {
            // シーン内のすべてのプレイヤーを取得
            Complete.TankMovement[] players = FindObjectsOfType<Complete.TankMovement>();

            foreach (Complete.TankMovement player in players)
            {
                PhotonView photonView = player.GetComponent<PhotonView>();

                // PhotonViewがnullでないか確認
                if (photonView == null)
                {
                    Debug.LogWarning("Player does not have a PhotonView component.");
                    continue;
                }

                // 自分のプレイヤーかどうか確認
                if (photonView.IsMine)
                {
                    playerTransform = player.transform;
                    isPlayerFound = true; // プレイヤーを見つけたらフラグを設定
                    Debug.Log("Local player found for MiniMap.");
                    yield break;
                }
            }

            // プレイヤーが見つからなかった場合、再試行
            Debug.LogWarning("Local player not found. Retrying...");
            yield return new WaitForSeconds(0.5f);
        }

        // 再試行が終了してもプレイヤーが見つからなかった場合のフォールバック
        if (!isPlayerFound)
        {
            Debug.LogError("Failed to find local player for MiniMap. Check player spawning and Photon setup.");
        }
    }

}
