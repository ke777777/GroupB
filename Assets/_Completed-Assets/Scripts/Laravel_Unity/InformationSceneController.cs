using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class InformationSceneController : MonoBehaviour
{
    public Text noticeText; // 通知を表示するTextコンポーネントを設定

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
        // 通知をTextコンポーネントに表示
        string displayText = "";

        // 列の開始位置を指定
        int idColumnWidth = 8;    // ID列の幅
        int titleColumnWidth = 25; // Title列の幅
        int contentColumnWidth = 50; // Content列の幅

        // 列ヘッダーを作成
        displayText += CreateRow("ID", "Title", "Content", idColumnWidth, titleColumnWidth, contentColumnWidth);
        displayText += new string('-', idColumnWidth + titleColumnWidth + contentColumnWidth) + "\n"; // 区切り線

        foreach (Notice notice in notices)
        {
            displayText += CreateRow(
                notice.id.ToString(),
                TruncateString(notice.title, titleColumnWidth - 2), // 列幅-2で余白を確保
                TruncateString(notice.content, contentColumnWidth - 2),
                idColumnWidth,
                titleColumnWidth,
                contentColumnWidth
            );
        }

        if (string.IsNullOrEmpty(displayText))
        {
            displayText = "No notifications available.";
        }

        noticeText.text = displayText;
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

    // 各列のデータを整形して行を作成
    string CreateRow(string id, string title, string content, int idWidth, int titleWidth, int contentWidth)
    {
        return id.PadRight(idWidth) + title.PadRight(titleWidth) + content.PadRight(contentWidth) + "\n";
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
