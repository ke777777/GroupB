using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        // 初期設定でマスターの名前を設定
        PhotonNetwork.NickName = "Master";  // 部屋を作成する前に名前を設定
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("サーバーに接続中...");
    }

    public override void OnConnectedToMaster()
    {
        // マスターサーバーに接続後、部屋を作成
        PhotonNetwork.CreateRoom("TanksGameRoom", new RoomOptions() { MaxPlayers = 2 }, TypedLobby.Default);
        Debug.Log("部屋作成中...");
    }

    // 部屋に参加または作成後に呼ばれる
    public override void OnJoinedRoom()
    {
        Debug.Log("部屋に参加しました");

        // マスタークライアントのIDと名前を表示
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"マスタークライアントのID: {PhotonNetwork.LocalPlayer.UserId}, マスタークライアントの名前: {PhotonNetwork.LocalPlayer.NickName}");
        }
        else
        {
            Debug.Log($"現在のプレイヤーはマスタークライアントではありません。");
        }
    }

    // 部屋から退出した場合に呼ばれる
    public override void OnLeftRoom()
    {
        Debug.Log("部屋から退出しました。");
    }
}