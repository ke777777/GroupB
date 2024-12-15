using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstLoginText : MonoBehaviour
{
    [SerializeField]
    private Text resultText;  // Textコンポーネントの参照

    private const string UserIdKey = "user_id";

    // Start is called before the first frame update
    void Start()
    {
        // PlayerPrefsにUserIdが保存されているか確認し、結果を表示
        DisplayPlayerPrefsStatus();
    }

    // PlayerPrefsにUserIdが保存されているかチェックして表示するメソッド
    private void DisplayPlayerPrefsStatus()
    {
        // PlayerPrefsにUserIdKeyが保存されているかチェック
        if (PlayerPrefs.HasKey(UserIdKey))
        {
            // 保存されていれば0を表示
            int userId = PlayerPrefs.GetInt(UserIdKey);
            resultText.text = "ID: " + userId;
        }
        else
        {
            // 保存されていなければ1を表示
            resultText.text = "First Login";
        }
    }
}
