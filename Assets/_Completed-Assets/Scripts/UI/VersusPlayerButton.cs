using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // ボタンのUIを扱うため
using UnityEngine.SceneManagement; // シーン遷移のため

public class VersusPlayerButton : MonoBehaviour
{
    [SerializeField] 
    private Button versusPlayerButton; // Buttonコンポーネントを参照するための変数

    private void Start()
    {
        // ボタンがクリックされたときにOnClickedメソッドを登録
        versusPlayerButton.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        // ホーム画面へ遷移
        SceneManager.LoadScene(SceneNames.CompleteGame);
    }
}
