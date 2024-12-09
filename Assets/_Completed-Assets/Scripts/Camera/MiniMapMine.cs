using UnityEngine;
using Photon.Pun;

public class MiniMapMine : MonoBehaviourPun
{
    [SerializeField] private GameObject skullIconPrefab;
    [SerializeField] private RectTransform miniMapTransform;
    [SerializeField] private Transform miniMapCameraTransform;
    private GameObject skullIconInstance;

    private void Start()
    {
        if (miniMapTransform == null)
        {
            // �^�O "MiniMapCanvas" ���~�j�}�b�v�p�� Canvas �ɐݒ肵�Ă���
            GameObject miniMapCanvas = GameObject.FindWithTag("MiniMapCanvas");

            if (miniMapCanvas != null)
            {
                miniMapTransform = miniMapCanvas.GetComponent<RectTransform>();
            }
            else
            {
                Debug.LogError("MiniMapCanvas not found! Please set the tag 'MiniMapCanvas' to the MiniMap Canvas object.");
            }
        }

        if (miniMapCameraTransform == null)
        {
            // �^�O "MiniMapCamera" ���~�j�}�b�v�J�����ɐݒ肵�Ă���
            miniMapCameraTransform = GameObject.FindWithTag("MiniMapCamera")?.transform;

            if (miniMapCameraTransform == null)
            {
                Debug.LogError("MiniMapCamera not found! Please set the tag 'MiniMapCamera' to the MiniMap Camera object.");
            }
        }

        if (photonView.IsMine)
        {
            AddSkullToMiniMap();
        }
    }

    private void Update()
    {
        if (skullIconInstance != null && miniMapCameraTransform != null)
        {
            UpdateIconPosition();
        }
    }

    private void AddSkullToMiniMap()
    {
        // �~�j�}�b�v���鐃A�C�R����ǉ�
        skullIconInstance = Instantiate(skullIconPrefab, miniMapTransform);

        // �����ʒu��ݒ�
        UpdateIconPosition();
    }

    private void UpdateIconPosition()
    {
        // �n���̃��[���h���W���~�j�}�b�v�̃��[�J�����W�ɕϊ�
        Vector2 miniMapPosition = WorldToMiniMapPosition(transform.position);
        skullIconInstance.GetComponent<RectTransform>().anchoredPosition = miniMapPosition;
    }

    private Vector2 WorldToMiniMapPosition(Vector3 worldPosition)
    {
        // ���[���h���W���~�j�}�b�v�̃J�����r���[���Ƀ}�b�s���O
        Vector3 relativePosition = worldPosition - miniMapCameraTransform.position;

        float scale = 1f / miniMapCameraTransform.localScale.x;

        return new Vector2(relativePosition.x * scale, relativePosition.z * scale);
    }

    private void OnDestroy()
    {
        // �n�����폜���ꂽ�ꍇ�A�~�j�}�b�v�̃A�C�R�����폜
        if (skullIconInstance != null)
        {
            Destroy(skullIconInstance);
        }
    }
}
