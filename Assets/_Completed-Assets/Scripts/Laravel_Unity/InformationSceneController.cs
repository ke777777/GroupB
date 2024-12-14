using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class InformationSceneController : MonoBehaviour
{
    public Text noticeText; // 通知を表示するTextコンポーネントを設定
    public GameObject noticePopup; // ポップアップオブジェクト
    public Text popupTitle; // ポップアップ内のタイトル表示用Text
    public Text popupContent; // ポップアップ内のコンテンツ表示用Text
    public Button okButton; // ポップアップ内のOKボタン

    private string apiUrl = "http://localhost/api/notice"; // APIのエンドポイントURL

    void Start()
    {
        // シーンがロードされたときに通知を取得
        StartCoroutine(GetNotices());
    }

    IEnumerator GetNotices()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
                noticeText.text = "Failed to load notifications.";
            }
            else
            {
                string json = request.downloadHandler.text;
                // Debug.Log("Response: " + json);

                // JSONをデコードして処理
                List<Notice> notices = JsonUtility.FromJson<NoticeListWrapper>("{\"notices\":" + json + "}").notices;
                DisplayNotices(notices);
            }
        }
    }


    void DisplayNotices(List<Notice> notices)
    {
        // 元のテキストコンポーネントを使用して通知を表示
        string displayText = "";

        // 列の開始位置を指定
        int titleColumnWidth = 25;

        // 列ヘッダーを作成
        displayText += CreateRow("Title", titleColumnWidth);
        displayText += new string('-', titleColumnWidth) + "\n";

        float startY = 90f; // 初期Y座標
        float rowHeight = 25f; // 各行の高さ

        foreach (Notice notice in notices)
        {
            displayText += CreateRow(
                TruncateString(notice.title, titleColumnWidth - 2),
                titleColumnWidth
            );

            // タイトルテキストをクリックできるように設定
            GameObject titleButtonObject = new GameObject("TitleButton");
            titleButtonObject.transform.SetParent(noticeText.transform, false);
            // Buttonを生成し、テキストをその子として追加
            Button titleButton = titleButtonObject.AddComponent<Button>();
            Text titleText = titleButtonObject.AddComponent<Text>();
            titleText.text = notice.title;
            titleText.font = noticeText.font;
            // titleText.color = Color.blue; // 色を青にしてクリック可能に見せる
            titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, 0f); // 完全透明
            titleText.alignment = TextAnchor.MiddleLeft;
            titleText.raycastTarget = true; // レイキャストを有効にしてクリックを検知
            // RectTransform設定
            RectTransform rectTransform = titleButtonObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(627, 25); // 必要に応じて調整
            rectTransform.anchoredPosition = new Vector2(0, startY); // 個別の位置を計算
            startY -= rowHeight; // 次の行のY座標を計算
            // クリックイベントを設定
            titleButton.onClick.AddListener(() => OpenPopup(notice, notices));
 
        }

        if (string.IsNullOrEmpty(displayText))
        {
            displayText = "No notices available.";
        }

        noticeText.text = displayText;
    }

    void OpenPopup(Notice notice, List<Notice> allInquiries)
    {
        popupTitle.text = notice.title; // タイトルを表示
        popupContent.text = notice.content;
        noticePopup.SetActive(true); // ポップアップを表示
    }

    public void OnOKButtonPressed()
    {
        noticePopup.SetActive(false);
    }


    // 長い文字列を切り詰めて「...」を追加
    string TruncateString(string value, int maxLength)
    {
        if (value.Length > maxLength)
        {
            return value.Substring(0, maxLength - 3) + "...";
        }
        return value;
    }

    string CreateRow(string title, int titleWidth)
    {
        return title.PadRight(titleWidth) + "\n";
    }

    [System.Serializable]
    public class Notice
    {
        public int id;
        public string title;
        public string content;
    }

    [System.Serializable]
    public class NoticeListWrapper
    {
        public List<Notice> notices;
    }
}
