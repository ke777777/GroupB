using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserIDManager : MonoBehaviour
{
    public InputField userIDInputField; // UserID入力欄
    public Button saveButton;          // 保存ボタン

    void Start()
    {
        // // 既にUserIDが登録済みの場合、直接ホーム画面に遷移
        // if (PlayerPrefs.HasKey("UserID"))
        // {
        //     SceneManager.LoadScene("HomeScene"); // ホーム画面に遷移
        //     return;
        // }

        saveButton.onClick.AddListener(SaveUserID);
    }

    void SaveUserID()
    {
        string userID = userIDInputField.text.Trim();
        if (!string.IsNullOrEmpty(userID))
        {
            PlayerPrefs.SetString("UserID", userID);

            // 登録後にホーム画面に遷移
            SceneManager.LoadScene(SceneNames.HomeScene);
        }
    }
}
