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
            // タグ "MiniMapCanvas" をミニマップ用の Canvas に設定しておく
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
            // タグ "MiniMapCamera" をミニマップカメラに設定しておく
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
        // ミニマップに髑髏アイコンを追加
        skullIconInstance = Instantiate(skullIconPrefab, miniMapTransform);

        // 初期位置を設定
        UpdateIconPosition();
    }

    private void UpdateIconPosition()
    {
        // 地雷のワールド座標をミニマップのローカル座標に変換
        Vector2 miniMapPosition = WorldToMiniMapPosition(transform.position);
        skullIconInstance.GetComponent<RectTransform>().anchoredPosition = miniMapPosition;
    }

    private Vector2 WorldToMiniMapPosition(Vector3 worldPosition)
    {
        // ワールド座標をミニマップのカメラビュー内にマッピング
        Vector3 relativePosition = worldPosition - miniMapCameraTransform.position;

        float scale = 1f / miniMapCameraTransform.localScale.x;

        return new Vector2(relativePosition.x * scale, relativePosition.z * scale);
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
