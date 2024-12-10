using UnityEngine;
using UnityEngine.UI;

public class UserRegistration : MonoBehaviour
{
    public Text userNameText;        // UserNameを表示するテキスト
    public GameObject popupPanel;    // 入力ポップアップのパネル
    public InputField nameInputField; // ユーザー名入力欄

    private string defaultName = "No Name"; // 初期値

    void Start()
    {
        // 保存された名前をロード
        string savedName = PlayerPrefs.GetString("UserName", defaultName);
        userNameText.text = "UserName: " + savedName;

        // ポップアップを非表示に
        popupPanel.SetActive(false);
    }

    // ユーザー登録ボタンが押された時の処理
    public void OnUserRegistrationButtonPressed()
    {
        // 入力フィールドをクリア
        nameInputField.text = "";

        // ポップアップを表示
        popupPanel.SetActive(true);
    }

    // OKボタンが押された時の処理
    public void OnOKButtonPressed()
    {
        // 入力が空白でない場合のみ反映
        if (!string.IsNullOrEmpty(nameInputField.text))
        {
            string newName = nameInputField.text;
            userNameText.text = "UserName: " + newName;

            // 名前を保存
            PlayerPrefs.SetString("UserName", newName);
            PlayerPrefs.Save();
        }

        // ポップアップを非表示に
        popupPanel.SetActive(false);
    }

    // キャンセルボタンが押された時の処理
    public void OnCancelButtonPressed()
    {
        // ポップアップを非表示に
        popupPanel.SetActive(false);
    }
}
