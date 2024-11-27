using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // ロビー関連
    private bool isReady = false;        // 自分の準備完了状態
    private bool opponentReady = false; // 対戦相手の準備完了状態

    // スタンプ関連
    public GameObject stampPrefab;       // スタンプ用プレハブ
    public Sprite[] stamps;             // スタンプ画像リスト（6個の画像をInspectorで設定）
    public Canvas canvas;               // スタンプを表示するキャンバス
    public Transform[] stampPositions;  // 各スタンプボタンに対応する表示位置
    public Button[] stampButtons;       // スタンプボタンリスト

    private GameObject currentStamp;    // 現在表示中のスタンプ
    private bool isStampActive = false; // 現在スタンプがアクティブかどうか（連打防止用）

    void Start()
    {
        Debug.Log("NetworkManager Start");

        // Photonサーバーに接続
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Connecting to Photon...");
    }

    // マスターサーバーへの接続成功時
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        PhotonNetwork.JoinLobby();
    }

    // ロビーへの参加成功時
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        CreateOrJoinRoom();
    }

    // ルーム作成または参加
    public void CreateOrJoinRoom()
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 2 // 対戦プレイヤーは2人まで
        };
        PhotonNetwork.JoinOrCreateRoom("VersusRoom", roomOptions, TypedLobby.Default);
    }

    // ルームへの参加成功時
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
    }

    // 対戦相手がルームに参加した時
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} has joined the room");
    }

    // READYボタン押下時
    public void ReadyButtonPressed()
    {
        Debug.Log("ReadyButtonPressed called");

        // `photonView`の確認
        if (photonView == null)
        {
            Debug.LogError("photonView is null. Ensure NetworkManager has a PhotonView component.");
            return;
        }

        if (isReady)
        {
            Debug.LogWarning("You are already ready.");
            return;
        }

        isReady = true;
        Debug.Log("You are now ready.");
        UpdateReadyStatusUI(true); // UIの更新
        photonView.RPC("SetOpponentReady", RpcTarget.Others); // 相手に通知
        CheckReadyState();
    }

    // 対戦相手が準備完了した時の通知
    [PunRPC]
    public void SetOpponentReady()
    {
        opponentReady = true;
        Debug.Log("Opponent is now ready.");
        UpdateReadyStatusUI(false); // UIの更新
        CheckReadyState();
    }

    // 自分と対戦相手が両方準備完了かチェック
    private void CheckReadyState()
    {
        if (isReady && opponentReady)
        {
            Debug.Log("Both players are ready. Starting the match...");
            PhotonNetwork.LoadLevel(SceneNames.CompleteGameScene); // シーン遷移
        }
    }

    // 準備完了状態をUIに反映
    private void UpdateReadyStatusUI(bool isSelf)
    {
        if (isSelf)
        {
            Debug.Log("Updating UI: You are ready.");
            // 自分のUI更新処理
        }
        else
        {
            Debug.Log("Updating UI: Opponent is ready.");
            // 相手のUI更新処理
        }
    }

    // スタンプボタンが押されたとき
    public void OnStampButtonClicked(int stampIndex)
    {
        Debug.Log($"OnStampButtonClicked called with index: {stampIndex}");

        if (stampPrefab == null)
        {
            Debug.LogError("stampPrefab is null. Please assign it in the Inspector.");
            return;
        }

        if (canvas == null)
        {
            Debug.LogError("canvas is null. Please assign it in the Inspector.");
            return;
        }

        if (stampPositions == null || stampPositions.Length <= stampIndex || stampPositions[stampIndex] == null)
        {
            Debug.LogError($"Invalid stampPositions or stampIndex: {stampIndex}");
            return;
        }

        if (photonView == null)
        {
            Debug.LogError("photonView is null. Ensure NetworkManager has a PhotonView component.");
            return;
        }

        Debug.Log($"Sending stamp index: {stampIndex}");

        // 自分の画面にも即時反映
        ReceiveStamp(stampIndex, PhotonNetwork.NickName);

        // 他のプレイヤーにも送信
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("ReceiveStamp", RpcTarget.Others, stampIndex, PhotonNetwork.NickName);
        }
        else
        {
            Debug.LogWarning("Cannot send RPC. Player is not in a room.");
        }
    }

    // スタンプ受信処理
    [PunRPC]
    public void ReceiveStamp(int stampIndex, string senderName)
    {
        Debug.Log($"ReceiveStamp called with index: {stampIndex}, sender: {senderName}");

        if (stampPrefab == null || canvas == null || stampPositions == null || stamps == null)
        {
            Debug.LogError("Stamp setup is incomplete. Check Inspector settings.");
            return;
        }

        isStampActive = true;

        if (currentStamp != null)
        {
            Destroy(currentStamp);
        }

        Transform targetPosition = stampPositions[stampIndex];
        currentStamp = Instantiate(stampPrefab, canvas.transform);
        RectTransform stampRectTransform = currentStamp.GetComponent<RectTransform>();
        stampRectTransform.position = targetPosition.position;

        Image stampImage = currentStamp.GetComponent<Image>();
        stampImage.sprite = stamps[stampIndex];
        stampImage.color = stampButtons[stampIndex].GetComponent<Image>().color;

        Debug.Log($"{senderName} sent a stamp.");
        StartCoroutine(FadeOutStamp(currentStamp));
    }

    private IEnumerator FadeOutStamp(GameObject stamp)
    {
        CanvasGroup stampCanvasGroup = stamp.GetComponent<CanvasGroup>();
        if (stampCanvasGroup == null)
        {
            stampCanvasGroup = stamp.AddComponent<CanvasGroup>();
        }

        yield return new WaitForSeconds(5f);

        float fadeDuration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            if (stamp == null)
            {
                isStampActive = false;
                yield break;
            }

            stampCanvasGroup.alpha = 1 - (elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (currentStamp == stamp)
        {
            Destroy(currentStamp);
            currentStamp = null;
            isStampActive = false;
        }
    }
}
