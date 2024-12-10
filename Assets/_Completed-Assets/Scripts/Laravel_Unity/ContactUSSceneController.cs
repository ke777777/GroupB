using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class ContactUSSceneController : MonoBehaviour
{
    public Text inquiryText; // 通知を表示するTextコンポーネントを設定

    private string apiUrl = "http://localhost/api/inquiry"; // APIのエンドポイントURL

    void Start()
    {
        // シーンがロードされたときに通知を取得
        StartCoroutine(GetInquiries());
    }

    IEnumerator GetInquiries()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
                inquiryText.text = "Failed to load notifications.";
            }
            else
            {
                string json = request.downloadHandler.text;
                // Debug.Log("Response: " + json);

                // JSONをデコードして処理
                List<Inquiry> inquiries = JsonUtility.FromJson<InquiryListWrapper>("{\"inquiries\":" + json + "}").inquiries;

                // 自分のUserIDを取得
                string userID = PlayerPrefs.GetString("UserID", "Unknown User");

                // 自分宛の通知を抽出
                List<Inquiry> filteredInquiries = inquiries.FindAll(inquiry => inquiry.receiver_id == userID | inquiry.sender_id == userID);
                // List<Inquiry> filteredInquiries2 = inquiries.FindAll(inquiry => inquiry.sender_id == userID);

                // 通知を表示
                DisplayInquiries(filteredInquiries);
                // DisplayInquiries(filteredInquiries2);
            }
        }
    }
    void DisplayInquiries(List<Inquiry> inquiries)
    {
        string displayText = "";

        // 列の開始位置を指定
        int idColumnWidth = 8;    // ID列の幅
        int titleColumnWidth = 25; // Title列の幅
        int contentColumnWidth = 50; // Content列の幅

        // 列ヘッダーを作成
        displayText += CreateRow("ID", "Title", "Content", idColumnWidth, titleColumnWidth, contentColumnWidth);
        displayText += new string('-', idColumnWidth + titleColumnWidth + contentColumnWidth) + "\n"; // 区切り線

        foreach (Inquiry inquiry in inquiries)
        {
            displayText += CreateRow(
                inquiry.id.ToString(),
                TruncateString(inquiry.title, titleColumnWidth - 2), // 列幅-2で余白を確保
                TruncateString(inquiry.content, contentColumnWidth - 2),
                idColumnWidth,
                titleColumnWidth,
                contentColumnWidth
            );
        }

        if (string.IsNullOrEmpty(displayText))
        {
            displayText = "No inquiries available.";
        }

        inquiryText.text = displayText;
    }

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
    public class Inquiry
    {
        public int id;
        public string title;
        public string content;
        public string receiver_id;
        public string sender_id;
    }

    [System.Serializable]
    public class InquiryListWrapper
    {
        public List<Inquiry> inquiries;
    }
}
