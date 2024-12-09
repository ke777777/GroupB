using UnityEngine;
using Photon.Pun;

public class MiniMapMineIcon : MonoBehaviourPun
{
    [SerializeField] private GameObject skullIconPrefab; // �~�j�}�b�v�p�鐃A�C�R���̃v���n�u
    [SerializeField] private RectTransform miniMapTransform; // �~�j�}�b�v��UI�L�����o�X��RectTransform
    [SerializeField] private Camera miniMapCamera; // �~�j�}�b�v�p�J����
    private GameObject skullIconInstance; // �鐃A�C�R���̃C���X�^���X
    private void Start()
    {
        // MiniMapTransform�������擾
        Canvas miniMapCanvas = FindObjectOfType<Canvas>();
        if (miniMapCanvas != null)
        {
            miniMapTransform = miniMapCanvas.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogError("MiniMap Canvas not found!");
            return;
        }

        // MiniMapCamera�������擾
        miniMapCamera = GameObject.FindWithTag("MiniMapCamera")?.GetComponent<Camera>();
        if (miniMapCamera == null)
        {
            Debug.LogError("MiniMap Camera not found!");
            return;
        }

        if (photonView.IsMine)
        {
            AddSkullToMiniMap();
        }
    }
    private void Update()
    {
        if (skullIconInstance != null && miniMapCamera != null)
        {
            UpdateIconPosition();
        }
    }

    private void AddSkullToMiniMap()
    {
        skullIconInstance = Instantiate(skullIconPrefab, miniMapTransform);

        skullIconInstance.layer = LayerMask.NameToLayer("MiniMap");

        UpdateIconPosition();
    }
    private void UpdateIconPosition()
    {
        // �n���̃��[���h���W���X�N���[�����W�ɕϊ�
        Vector3 screenPosition = miniMapCamera.WorldToScreenPoint(transform.position);

        // �X�N���[�����W���~�j�}�b�v�̃��[�J�����W�ɕϊ�
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            miniMapTransform,
            screenPosition,
            miniMapCamera,
            out Vector2 localPosition
        );

        // �鐃A�C�R���̈ʒu���X�V
        skullIconInstance.GetComponent<RectTransform>().anchoredPosition = localPosition;
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
