using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // ���r�[�֘A
    private bool isReady = false;        // �����̏����������
    private bool opponentReady = false; // �ΐ푊��̏����������

    // �X�^���v�֘A
    public GameObject stampPrefab;       // �X�^���v�p�v���n�u
    public Sprite[] stamps;             // �X�^���v�摜���X�g�i6�̉摜��Inspector�Őݒ�j
    public Canvas canvas;               // �X�^���v��\������L�����o�X
    public Transform[] stampPositions;  // �e�X�^���v�{�^���ɑΉ�����\���ʒu
    public Button[] stampButtons;       // �X�^���v�{�^�����X�g

    private GameObject currentStamp;    // ���ݕ\�����̃X�^���v
    private bool isStampActive = false; // ���݃X�^���v���A�N�e�B�u���ǂ����i�A�Ŗh�~�p�j

    void Start()
    {
        Debug.Log("NetworkManager Start");

        // Photon�T�[�o�[�ɐڑ�
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Connecting to Photon...");
    }

    // �}�X�^�[�T�[�o�[�ւ̐ڑ�������
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        PhotonNetwork.JoinLobby();
    }

    // ���r�[�ւ̎Q��������
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        CreateOrJoinRoom();
    }

    // ���[���쐬�܂��͎Q��
    public void CreateOrJoinRoom()
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 2 // �ΐ�v���C���[��2�l�܂�
        };
        PhotonNetwork.JoinOrCreateRoom("VersusRoom", roomOptions, TypedLobby.Default);
    }

    // ���[���ւ̎Q��������
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
    }

    // �ΐ푊�肪���[���ɎQ��������
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} has joined the room");
    }

    // READY�{�^��������
    public void ReadyButtonPressed()
    {
        Debug.Log("ReadyButtonPressed called");

        // `photonView`�̊m�F
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
        UpdateReadyStatusUI(true); // UI�̍X�V
        photonView.RPC("SetOpponentReady", RpcTarget.Others); // ����ɒʒm
        CheckReadyState();
    }

    // �ΐ푊�肪���������������̒ʒm
    [PunRPC]
    public void SetOpponentReady()
    {
        opponentReady = true;
        Debug.Log("Opponent is now ready.");
        UpdateReadyStatusUI(false); // UI�̍X�V
        CheckReadyState();
    }

    // �����Ƒΐ푊�肪���������������`�F�b�N
    private void CheckReadyState()
    {
        if (isReady && opponentReady)
        {
            Debug.Log("Both players are ready. Starting the match...");
            PhotonNetwork.LoadLevel(SceneNames.CompleteGameScene); // �V�[���J��
        }
    }

    // ����������Ԃ�UI�ɔ��f
    private void UpdateReadyStatusUI(bool isSelf)
    {
        if (isSelf)
        {
            Debug.Log("Updating UI: You are ready.");
            // ������UI�X�V����
        }
        else
        {
            Debug.Log("Updating UI: Opponent is ready.");
            // �����UI�X�V����
        }
    }

    // �X�^���v�{�^���������ꂽ�Ƃ�
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

        // �����̉�ʂɂ��������f
        ReceiveStamp(stampIndex, PhotonNetwork.NickName);

        // ���̃v���C���[�ɂ����M
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("ReceiveStamp", RpcTarget.Others, stampIndex, PhotonNetwork.NickName);
        }
        else
        {
            Debug.LogWarning("Cannot send RPC. Player is not in a room.");
        }
    }

    // �X�^���v��M����
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
