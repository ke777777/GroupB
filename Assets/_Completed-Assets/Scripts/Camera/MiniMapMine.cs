using UnityEngine;
using Photon.Pun;

public class MiniMapMineIcon : MonoBehaviourPun
{
    [SerializeField] private GameObject skullPrefab;
    private float heightOffset = 1.0f;
    private GameObject skullInstance;
    private void Start()
    {
        // �������ݒu�����n���̏ꍇ�̂�Skull�𐶐�
        if (photonView.IsMine)
        {
            CreateSkullOnMiniMap();
        }
    }

    private void CreateSkullOnMiniMap()
    {
        skullInstance = Instantiate(skullPrefab);
        Vector3 adjustedPosition = transform.position;
        adjustedPosition.y += heightOffset;
        skullInstance.transform.position = adjustedPosition;

        // ���C���[��ݒ�
        int miniMapLayer = LayerMask.NameToLayer("MiniMap");
        if (miniMapLayer == -1)
        {
            Debug.LogError("MiniMapOnly layer not found.");
            return;
        }
        SetLayerRecursively(skullInstance, miniMapLayer);
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        // �I�u�W�F�N�g�Ƃ��̎q�I�u�W�F�N�g�Ƀ��C���[��ݒ�
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            child.gameObject.layer = layer;
        }
    }
    /*private void Update()
    {
        UpdateIconVisibility();
    }


    private void UpdateIconVisibility()
    {
        if (skullInstance == null || Camera.main == null) return;

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(skullInstance.transform.position);

        bool isOutsideView = screenPosition.z < 0 ||
                             screenPosition.x < 0 || screenPosition.x > Screen.width ||
                             screenPosition.y < 0 || screenPosition.y > Screen.height;

        skullInstance.SetActive(!isOutsideView);
    }
    */

    private void OnDestroy()
    {
        // �n�����폜���ꂽ�ꍇ�ASkull���폜
        if (skullInstance != null)
        {
            Destroy(skullInstance);
        }
    }
    private void RemoveSkullFromMiniMap()
    {
        if (skullInstance != null)
        {
            Destroy(skullInstance);
        }
    }
}
