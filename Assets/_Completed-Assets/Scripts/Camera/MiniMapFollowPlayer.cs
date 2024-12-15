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
                    yield break;
                }
            }

            yield return new WaitForSeconds(0.5f);             // プレイヤーが見つからなかった場合、再試行
        }

        if (!isPlayerFound)
        {
            Debug.LogError("Failed to find local player for MiniMap. Check player spawning and Photon setup.");
        }
    }

}
