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
        Debug.Log("Start method called."); // デバッグログで確認
        Debug.Log($"Assigned Button: {exitButton.name}"); // ボタン名を確認
        exitButton.onClick.AddListener(OnClicked);
    }
    private void OnClicked()
    {
        Debug.Log("Exit Lobby Button Clicked"); // デバッグログを追加
        SceneManager.LoadScene(SceneNames.HomeScene);
    }
}

