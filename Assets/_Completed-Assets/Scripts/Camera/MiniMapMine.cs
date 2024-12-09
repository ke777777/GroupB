using UnityEngine;
using Photon.Pun;

public class MiniMapMineIcon : MonoBehaviourPun
{
    [SerializeField] private GameObject skullIconPrefab; // ミニマップ用髑髏アイコンのプレハブ
    [SerializeField] private RectTransform miniMapTransform; // ミニマップのUIキャンバスのRectTransform
    [SerializeField] private Camera miniMapCamera; // ミニマップ用カメラ
    private GameObject skullIconInstance; // 髑髏アイコンのインスタンス
    private void Start()
    {
        // MiniMapTransformを自動取得
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

        // MiniMapCameraを自動取得
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
        // 地雷のワールド座標をスクリーン座標に変換
        Vector3 screenPosition = miniMapCamera.WorldToScreenPoint(transform.position);

        // スクリーン座標をミニマップのローカル座標に変換
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            miniMapTransform,
            screenPosition,
            miniMapCamera,
            out Vector2 localPosition
        );

        // 髑髏アイコンの位置を更新
        skullIconInstance.GetComponent<RectTransform>().anchoredPosition = localPosition;
    }

    private void OnDestroy()
    {
        // 地雷が削除された場合、ミニマップのアイコンも削除
        if (skullIconInstance != null)
        {
            Destroy(skullIconInstance);
        }
    }
}
