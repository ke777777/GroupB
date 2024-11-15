using UnityEngine;

public class MiniMapFollowPlayer : MonoBehaviour
{
    public int targetPlayerNumber = 1; // �ǐՂ���v���C���[�̔ԍ�
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
                    Debug.Log($"Player {targetPlayerNumber} found and assigned to MiniMap Camera.");
                    yield break;
                }
            }

            Debug.LogWarning($"Player with number {targetPlayerNumber} not found. Retrying...");
            yield return new WaitForSeconds(0.5f); // 0.5�b�ҋ@���čĎ��s
        }
    }

    private void LateUpdate()
    {
        if (playerTransform != null)
        {
            Vector3 newPosition = playerTransform.position;
            newPosition.y = transform.position.y; // �J�����̍������ێ�
            transform.position = newPosition;
        }
    }
}
