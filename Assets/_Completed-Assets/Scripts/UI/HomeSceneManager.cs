using UnityEngine;
using UnityEngine.UI;

public class HomeSceneManager : MonoBehaviour
{
    public Text userIDText; // UserIDを表示するTextコンポーネント

    void Start()
    {
        // 登録済みUserIDを取得
        string userID = PlayerPrefs.GetString("UserID", "Unknown User");
        userIDText.text = "User ID: " + userID;
    }
}
