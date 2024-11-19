using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ExitLobby : MonoBehaviour
{
   [SerializeField]private Button exitButton;

    void Start()
    {
        if (exitButton == null)
        {
            Debug.LogError("exitButton is not assigned in the Inspector!");
            return;
        }
        Debug.Log("Start method called."); // �f�o�b�O���O�Ŋm�F
        Debug.Log($"Assigned Button: {exitButton.name}"); // �{�^�������m�F
        exitButton.onClick.AddListener(OnClicked);
    }
    private void OnClicked()
    {
        Debug.Log("Exit Lobby Button Clicked"); // �f�o�b�O���O��ǉ�
        SceneManager.LoadScene(SceneNames.HomeScene);
    }
}

