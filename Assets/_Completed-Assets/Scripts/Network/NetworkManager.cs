using UnityEngine;
using Photon.Pun; // Photon Unity Networking API
using Photon.Realtime; // Realtime関連の名前空間

public class NetworkManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        // Photonサーバーに接続
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Connecting to Photon...");
    }

    // マスターサーバーに接続成功時に呼ばれる
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        // ロビーに参加
        PhotonNetwork.JoinLobby();
    }

    // ロビーに参加成功時に呼ばれる
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
    }

    // 接続失敗時のコールバック
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected: {cause}");
    }
}
