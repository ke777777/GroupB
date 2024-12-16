using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class StartButton : MonoBehaviour
{
    [SerializeField]
    private Button startButton;

    [SerializeField]
    private GameObject warningText; // アカウント停止メッセージを表示するテキストオブジェクト

    private const string UserIdKey = "user_id";
    private const string UserNameKey = "user_name";
    private const string DefaultUserName = "NoName";
    private const string ServerUrl = "http://localhost:8080"; // サーバーのURL

    private void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        // PlayerPrefsにUserIDが保存されているか確認
        if (UserUtils.GetUserId() != 0)
        {
            int userId = UserUtils.GetUserId();
            string userName = UserUtils.GetUserName();

            Debug.Log($"Existing User: {userName} (ID: {userId})");

            // サーバーにユーザー情報が存在するか確認
            StartCoroutine(CheckUserExists(userId, userName));
            StartCoroutine(GetDeleteFlag(userId)); // delete_flagの値を確認
        }
        else
        {
            // UserIDが未保存の場合、新規ユーザーIDを生成
            int newUserId = GenerateRandomUserId();
            string newUserName = DefaultUserName;

            // サーバーに保存し、結果を確認
            StartCoroutine(CheckAndSaveUserId(newUserId, newUserName));
        }
    }

    private int GenerateRandomUserId()
    {
        return Random.Range(1000, 10000); // 4桁のランダムな数字を生成
    }

    private void SaveUserId(int userId, string userName)
    {
        PlayerPrefs.SetInt(UserIdKey, userId);
        PlayerPrefs.SetString(UserNameKey, userName);
        PlayerPrefs.Save();
    }

    private void DeleteUserId()
    {
        PlayerPrefs.DeleteKey(UserIdKey);
        PlayerPrefs.DeleteKey(UserNameKey);
    }

    private IEnumerator CheckAndSaveUserId(int userId, string userName)
    {
        bool isUserIdUnique = false;

        while (!isUserIdUnique)
        {
            string url = $"{ServerUrl}/add_id?user_id={userId}&user_name={userName}";

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    string response = webRequest.downloadHandler.text;

                    if (response.Contains("User ID already exists."))
                    {
                        userId = GenerateRandomUserId();
                    }
                    else
                    {
                        isUserIdUnique = true;
                        SaveUserId(userId, userName);
                        SceneManager.LoadScene("HomeScene");
                    }
                }
                else
                {
                    Debug.LogError($"Failed to save user ID: {webRequest.error}");
                    yield break;
                }
            }
        }
    }

    private IEnumerator CheckUserExists(int userId, string userName)
    {
        string url = $"{ServerUrl}/check_user_exists?user_id={userId}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string response = webRequest.downloadHandler.text;

                if (response.Contains("User not found"))
                {
                    Debug.Log("User not found. Deleting local data...");
                    DeleteUserId();
                    SceneManager.LoadScene("TitleScene");
                }
                else if (response.Contains("User found"))
                {
                    Debug.Log("User exists. Continuing...");
                }
                else
                {
                    Debug.LogError("Unexpected server response: " + response);
                }
            }
            else
            {
                Debug.LogError("Failed to check user existence: " + webRequest.error);
            }
        }
    }

    private IEnumerator GetDeleteFlag(int userId)
    {
        string url = $"{ServerUrl}/get_column_value?user_id={userId}&column_name=delete_flag";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string response = webRequest.downloadHandler.text;

                if (response == "0")
                {
                    SceneManager.LoadScene("HomeScene");
                }
                else if (response == "1")
                {
                    ShowAccountSuspendedMessage();
                }
                else
                {
                    Debug.LogError($"Unexpected delete_flag value: {response}");
                }
            }
            else
            {
                Debug.LogError("Failed to fetch delete_flag: " + webRequest.error);
            }
        }
    }

    private void ShowAccountSuspendedMessage()
    {
        if (warningText != null)
        {
            warningText.SetActive(true);
        }
        else
        {
            Debug.LogError("WarningText object is not assigned.");
        }
    }
}
