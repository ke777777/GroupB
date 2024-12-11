using UnityEngine;
using Photon.Pun;

public class MiniMapFollowPlayer : MonoBehaviour
{
    private Transform playerTransform; // �v���C���[��Transform
    private bool isPlayerFound = false; // �v���C���[�������������ǂ����̃t���O


    private void Start()
    {
        StartCoroutine(FindLocalPlayer());
    }

    private void Update()
    {
        // �v���C���[�����������ꍇ�ɒǔ����������s
        if (isPlayerFound && playerTransform != null)
        {
            Vector3 newPosition = playerTransform.position;
            newPosition.y = transform.position.y; // �������Œ�
            transform.position = newPosition;
        }
    }

    private System.Collections.IEnumerator FindLocalPlayer()
    {
        while (!isPlayerFound)
        {
            // �V�[�����̂��ׂẴv���C���[���擾
            Complete.TankMovement[] players = FindObjectsOfType<Complete.TankMovement>();

            foreach (Complete.TankMovement player in players)
            {
                PhotonView photonView = player.GetComponent<PhotonView>();

                // PhotonView��null�łȂ����m�F
                if (photonView == null)
                {
                    Debug.LogWarning("Player does not have a PhotonView component.");
                    continue;
                }

                // �����̃v���C���[���ǂ����m�F
                if (photonView.IsMine)
                {
                    playerTransform = player.transform;
                    isPlayerFound = true; // �v���C���[����������t���O��ݒ�
                    yield break;
                }
            }

            yield return new WaitForSeconds(0.5f);             // �v���C���[��������Ȃ������ꍇ�A�Ď��s
        }

        if (!isPlayerFound)
        {
            Debug.LogError("Failed to find local player for MiniMap. Check player spawning and Photon setup.");
        }
    }

}
