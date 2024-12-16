using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class StartButton : MonoBehaviour
{
    [SerializeField]
    private Button startButton;

    [SerializeField]
    private GameObject warningText;  // アカウント停止メッセージを表示するテキストオブジェクト

    private const string UserIdKey = "user_id";
    private const string UserNameKey = "user_name";
    private const string DefaultUserName = "NoName";
    private const string ServerUrl = "http://localhost:8080"; // サーバーのURL

    private void Start()
    {
        // ボタンがクリックされたら実行
        startButton.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        // PlayerPrefsからuser_idとuser_nameを取得
        if (!PlayerPrefs.HasKey(UserIdKey))
        {
            int userId = GenerateRandomUserId();
            string userName = DefaultUserName;

            // user_idがデータベースに既に存在しないか確認
            StartCoroutine(CheckAndSaveUserId(userId, userName));
        }
        else
        {
            // user_idとuser_nameがすでにPlayerPrefsに保存されている場合
            string userName = PlayerPrefs.GetString(UserNameKey);
            int userId = PlayerPrefs.GetInt(UserIdKey);
            Debug.Log(userName);
            Debug.Log(userId);

            // ユーザー情報がサーバー側に存在するか確認
            StartCoroutine(CheckUserExists(userId, userName));

            // ここでdelete_flagの値を取得する新しいコルーチンを呼び出す
            StartCoroutine(GetDeleteFlag(userId));
        }
    }

    private int GenerateRandomUserId()
    {
        return Random.Range(1000, 10000); // 4桁のランダムな数字を生成
    }

    private void SaveUserId(int userId, string userName)
    {
        // PlayerPrefsにuser_idとuser_nameを保存
        PlayerPrefs.SetInt(UserIdKey, userId);
        PlayerPrefs.SetString(UserNameKey, userName);
        PlayerPrefs.Save();
    }

    private void deleteUserId(int userId, string userName)
    {
        // PlayerPrefsにuser_idとuser_nameを削除
        PlayerPrefs.DeleteKey(UserIdKey);
        PlayerPrefs.DeleteKey(UserNameKey);
    }

    private IEnumerator CheckAndSaveUserId(int userId, string userName)
    {
        bool isUserIdUnique = false;

        while (!isUserIdUnique)
        {
            // サーバーへのリクエストURLを作成
            string url = $"{ServerUrl}/add_id?user_id={userId}&user_name={userName}";

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // リクエストを送信
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success || (webRequest.downloadHandler.text).Contains("User ID already exists."))
                {
                    // サーバーからのレスポンスを確認
                    string response = webRequest.downloadHandler.text;

                    if (response.Contains("User ID already exists."))
                    {
                        // 同じuser_idが既に存在する場合は新しいuser_idを生成し直す
                        userId = GenerateRandomUserId();
                    }
                    else
                    {
                        // user_idがユニークだった場合
                        isUserIdUnique = true;
                        SaveUserId(userId, userName);
                    }
                }
                else
                {
                    // エラーが発生した場合
                    Debug.LogError("Web request failed: " + webRequest.error);
                    yield break;
                }
            }
        }
        // Sceneを読み込む
        SceneManager.LoadScene("HomeScene");
    }

    private IEnumerator CheckUserExists(int userId, string userName)
    {
        // サーバーへのリクエストURLを作成
        string url = $"{ServerUrl}/check_user_exists?user_id={userId}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // リクエストを送信
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success || (webRequest.downloadHandler.text).Contains("User not found"))
            {
                // サーバーからのレスポンスを確認
                string response = webRequest.downloadHandler.text;
                // "User not found" メッセージが返ってきた場合、削除されたとみなす
                if (response.Contains("User not found"))
                {
                    Debug.Log("User not found, deleting...");
                    // ユーザーが削除されていた場合、PlayerPrefsからも削除
                    deleteUserId(userId, userName);
                    // 再度、最初の画面に戻す
                    SceneManager.LoadScene("TitleScene");
                }
                else if (response.Contains("User found"))
                {
                    Debug.Log("User found, continuing...");
                    // ユーザーが存在する場合、そのまま続行
                    //SceneManager.LoadScene("HomeScene");
                }
                else
                {
                    // 想定外のレスポンスの場合
                    Debug.LogError("Unexpected server response: " + response);
                    yield break;
                }
            }
            else
            {
                // エラーが発生した場合
                Debug.LogError("Web request failed: " + webRequest.error);
                yield break;
            }
        }
    }

    private IEnumerator GetDeleteFlag(int userId)
    {
        // サーバーへのリクエストURLを作成
        string url = $"{ServerUrl}/get_column_value?user_id={userId}&column_name=delete_flag";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // リクエストを送信
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // サーバーからのレスポンスを確認
                string response = webRequest.downloadHandler.text;

                // レスポンスの値を表示
                Debug.Log("Delete flag value: " + response);

                // サーバーから返ってきたレスポンスがそのままdelete_flagの値
                if (response == "0")
                {
                    // delete_flagが0の場合はHomeSceneに遷移
                    SceneManager.LoadScene("HomeScene");
                }
                else if (response == "1")
                {
                    // delete_flagが1の場合は警告メッセージを表示
                    ShowAccountSuspendedMessage();
                }
                else
                {
                    Debug.LogError("Unexpected delete flag value: " + response);
                }
            }
            else
            {
                // エラーが発生した場合
                Debug.LogError("Web request failed: " + webRequest.error);
            }
        }
    }

    private void ShowAccountSuspendedMessage()
    {
        if (warningText != null)
        {
            warningText.SetActive(true);  // メッセージを表示
        }
        else
        {
            Debug.LogError("Warning Text is not assigned!");
        }
    }
}