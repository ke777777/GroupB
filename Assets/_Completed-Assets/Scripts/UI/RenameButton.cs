using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;  // UnityWebRequestを使用するために追加

public class RenameButton : MonoBehaviour
{
    [SerializeField]
    private Button reNameButton;  // リネームボタン

    // ダイアログのPanelオブジェクト
    [SerializeField]
    private GameObject dialogPanel;  // ダイアログパネル

    [SerializeField]
    private Button cancelButton;  // キャンセルボタン

    [SerializeField]
    private InputField nameInputField;  // 名前入力欄（InputField）

    [SerializeField]
    private Button okButton;  // OKボタン

    [SerializeField]
    private GameObject errorText;  // エラーメッセージ用のTextコンポーネント

    // UI表示用のTextコンポーネント
    [SerializeField]
    private Text userIdText;  // ユーザーIDを表示するText
    [SerializeField]
    private Text userNameText;  // ユーザー名を表示するText

    private const string UserIdKey = "user_id";  // PlayerPrefsのキー
    private const string UserNameKey = "user_name";
    private const string ServerUrl = "http://localhost:8080";  // サーバーのURL（ローカル環境など）

    private void Start()
    {
        // Renameボタンがクリックされた時にOnClickedメソッドを呼び出す
        reNameButton.onClick.AddListener(OnClicked);

        // CancelボタンがクリックされたときにCloseDialogメソッドを呼び出す
        cancelButton.onClick.AddListener(CloseDialog);

        // OKボタンがクリックされたときにShowEnteredNameメソッドを呼び出す
        okButton.onClick.AddListener(ShowEnteredName);

        // UIにユーザーIDとユーザー名を表示
        DisplayUserInfo();
    }

    // ユーザーIDと名前を画面に表示
    private void DisplayUserInfo()
    {
        // PlayerPrefsからuser_idとuser_nameを取得
        int userId = PlayerPrefs.GetInt(UserIdKey);
        string userName = PlayerPrefs.GetString(UserNameKey);

        // 取得したユーザーIDと名前をTextコンポーネントに表示
        userIdText.text = "User ID: " + userId;
        userNameText.text = "User Name: " + userName;
    }

    // Renameボタンがクリックされたときに呼ばれるメソッド
    private void OnClicked()
    {
        dialogPanel.SetActive(true);  // ダイアログを表示
    }

    // Cancelボタンがクリックされたときに呼ばれるメソッド
    private void CloseDialog()
    {
        dialogPanel.SetActive(false);  // ダイアログを非表示にする
    }

    // OKボタンがクリックされたときに呼ばれるメソッド
    private void ShowEnteredName()
    {
        string enteredName = nameInputField.text;  // 入力されたテキストを取得
        int userId = PlayerPrefs.GetInt(UserIdKey);  // PlayerPrefsからuser_idを取得
        
        // 名前が制限を満たしているかチェック
        if (enteredName.Length >= 3 && enteredName.Length <= 15)
        {
            // サーバーへユーザー名更新リクエストを送信
            StartCoroutine(UpdateUserName(userId, enteredName));
        }
        else
        {
            errorText.SetActive(true);  // エラーメッセージを表示
        }
    }

    private IEnumerator UpdateUserName(int userId, string userName)
    {
        // サーバーのURLを使用して、GETリクエストのURLを作成
        string url = $"{ServerUrl}/update_column_value?user_id={userId}&column_name={UserNameKey}&new_value={userName}";
        
        // GETリクエストを送信
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // リクエストを送信し、サーバーの応答を待つ
            yield return webRequest.SendWebRequest();

            // リクエストが失敗した場合のエラーチェック
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                // エラーが発生した場合の処理
                Debug.LogError("Error updating user name: " + webRequest.error);
            }
            else
            {
                // サーバーからの応答が成功の場合
                string response = webRequest.downloadHandler.text;
                Debug.Log("Server Response: " + response);
                
                // 名前が更新された場合
                Debug.Log("User name updated successfully!");
                    
                // PlayerPrefsの名前を更新
                PlayerPrefs.SetString(UserNameKey, userName);
                PlayerPrefs.Save();
                DisplayUserInfo();
            }
        }
    }
}