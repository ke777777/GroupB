using UnityEngine;

public class MiniMapFollowPlayer : MonoBehaviour
{
    public int targetPlayerNumber = 1; // 追跡するプレイヤーの番号
    private Transform playerTransform;

    private void Start()
    {
        StartCoroutine(FindPlayer());
    }

    private System.Collections.IEnumerator FindPlayer()
    {
        while (playerTransform == null)
        {
            Complete.TankMovement[] players = FindObjectsOfType<Complete.TankMovement>();
            foreach (Complete.TankMovement player in players)
            {
                if (player.m_PlayerNumber == targetPlayerNumber)
                {
                    playerTransform = player.transform;
                    yield break;
                }
            }

            Debug.LogWarning($"Player with number {targetPlayerNumber} not found. Retrying...");
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void LateUpdate()
    {
        if (playerTransform != null)
        {
            Vector3 newPosition = playerTransform.position;
            newPosition.y = transform.position.y; // カメラの高さを維持
            transform.position = newPosition;
        }
    }
}
