using UnityEngine;
using Photon.Pun;

public class MiniMapMineIcon : MonoBehaviourPun
{
    [SerializeField] private GameObject skullPrefab;
    private float heightOffset = 1.0f;
    private GameObject skullInstance;
    private void Start()
    {
        // 自分が設置した地雷の場合のみSkullを生成
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

        // レイヤーを設定
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
        // オブジェクトとその子オブジェクトにレイヤーを設定
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
        // 地雷が削除された場合、Skullも削除
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
