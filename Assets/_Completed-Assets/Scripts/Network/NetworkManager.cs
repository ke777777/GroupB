using UnityEngine;
using Photon.Pun; // Photon Unity Networking API
using Photon.Realtime; // Realtime�֘A�̖��O���

public class NetworkManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        // Photon�T�[�o�[�ɐڑ�
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Connecting to Photon...");
    }

    // �}�X�^�[�T�[�o�[�ɐڑ��������ɌĂ΂��
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        // ���r�[�ɎQ��
        PhotonNetwork.JoinLobby();
    }

    // ���r�[�ɎQ���������ɌĂ΂��
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
    }

    // �ڑ����s���̃R�[���o�b�N
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected: {cause}");
    }
}
