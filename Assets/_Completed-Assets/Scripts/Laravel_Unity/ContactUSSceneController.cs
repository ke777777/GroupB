using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class ContactUSSceneController : MonoBehaviour
{
    public Text inquiryText; // 通知を表示するTextコンポーネント
    public GameObject inquiryPopup; // ポップアップオブジェクト
    public Text popupTitle; // ポップアップ内のタイトル表示用Text
    public Text popupContent; // ポップアップ内のコンテンツ表示用Text
    public InputField replyInputField; // 返信用の入力フィールド
    public Button okButton; // ポップアップ内のOKボタン
    public Button closeButton; // ポップアップ内の閉じるボタン
    public GameObject successMessage; // Successメッセージのポップアップ

    private string apiUrl = "http://localhost/api/inquiry"; // APIのエンドポイントURL
    private string replyUrl = "http://localhost/api/reply"; // APIのエンドポイントreplyURL
    public string csrfTokenUrl = "http://localhost/csrf-token"; // CSRFトークンを取得するエンドポイント
    private Inquiry selectedInquiry; // 現在選択されている通知
    private string csrfToken;

    void Start()
    {
        StartCoroutine(GetCsrfToken());

        // シーンがロードされたときに通知を取得
        StartCoroutine(GetInquiries());

        // ボタンイベントの設定
        okButton.onClick.AddListener(SendReply);
        closeButton.onClick.AddListener(ClosePopup);
    }

    private IEnumerator GetCsrfToken()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(csrfTokenUrl))
        {
            request.SetRequestHeader("Accept", "application/json");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<CsrfResponse>(request.downloadHandler.text);
                csrfToken = response.token;
            }
            else
            {
                Debug.LogError("Failed to get CSRF token: " + request.error);
            }
        }
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

                // JSONをデコードして処理
                List<Inquiry> inquiries = JsonUtility.FromJson<InquiryListWrapper>("{\"inquiries\":" + json + "}").inquiries;

                // 自分のUserIDを取得
                string userID = PlayerPrefs.GetString("UserID", "Unknown User");

                // 自分宛の通知を抽出
                List<Inquiry> filteredInquiries = inquiries.FindAll(inquiry => inquiry.receiver_id == userID || inquiry.sender_id == userID);

                // 通知を表示
                DisplayInquiries(filteredInquiries);
            }
        }
    }

    void DisplayInquiries(List<Inquiry> inquiries)
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

        HashSet<int> displayedInquiryIds = new HashSet<int>(); // 表示済みのinquiry_idを記録するためのセット

        foreach (Inquiry inquiry in inquiries)
        {
            // inquiry_idがすでに表示されたことがあるかチェック
            if (!displayedInquiryIds.Contains(inquiry.inquiry_id))
            {
                // 新しいinquiry_idなら表示
                displayText += CreateRow(
                    TruncateString(inquiry.title, titleColumnWidth - 2),
                    titleColumnWidth
                );

                // タイトルテキストをクリックできるように設定
                GameObject titleButtonObject = new GameObject("TitleButton");
                titleButtonObject.transform.SetParent(inquiryText.transform, false);

                // Buttonを生成し、テキストをその子として追加
                Button titleButton = titleButtonObject.AddComponent<Button>();
                Text titleText = titleButtonObject.AddComponent<Text>();
                titleText.text = inquiry.title;
                titleText.font = inquiryText.font;
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
                titleButton.onClick.AddListener(() => OpenPopup(inquiry, inquiries));

                // 表示済みのinquiry_idをセットに追加
                displayedInquiryIds.Add(inquiry.inquiry_id);
            }
        }

        if (string.IsNullOrEmpty(displayText))
        {
            displayText = "No inquiries available.";
        }

        inquiryText.text = displayText;
    }

    void OpenPopup(Inquiry inquiry, List<Inquiry> allInquiries)
    {
        selectedInquiry = inquiry; // 現在の通知を保存
        popupTitle.text = inquiry.title; // タイトルを表示

        // 同じinquiry_idを持つ全ての通知を取得
        List<Inquiry> relatedInquiries = allInquiries.FindAll(i => i.inquiry_id == inquiry.inquiry_id);

        // コンテンツをまとめて表示
        string contentText = "";
        foreach (var related in relatedInquiries)
        {
            if (related.sender_id == "999999") // 運営の返信
            {
                contentText += $"Operator: {related.content}\n";
            }
            else // ユーザーの送信
            {
                contentText += $"You: {related.content}\n";
            }
        }

        popupContent.text = contentText; // まとめた内容を表示
        replyInputField.text = ""; // 入力フィールドをリセット
        inquiryPopup.SetActive(true); // ポップアップを表示
    }


    void ClosePopup()
    {
        inquiryPopup.SetActive(false); // ポップアップを非表示
    }


    public void SendReply()
    {
        if (string.IsNullOrEmpty(csrfToken))
        {
            Debug.LogError("CSRF token not available.");
            return;
        }

        // 選択された問い合わせのIDを取得
        int inquiryId = selectedInquiry.inquiry_id;
        string content = replyInputField.text; // 入力された内容を取得
        string userID = PlayerPrefs.GetString("UserID", "Unknown User");
        string senderId = userID;
        string receiverId = "999999"; // 自動的に運営のIDを設定

        // inquiryId を引数として PostReply に渡す
        StartCoroutine(PostReply(inquiryId, senderId, receiverId, content));
    }

    IEnumerator PostReply(int inquiryId, string senderId, string receiverId, string content)
    {
        WWWForm form = new WWWForm();
        form.AddField("inquiry_id", inquiryId); // inquiry_id を送信
        form.AddField("content", content); // 本文のみを送信
        form.AddField("sender_id", senderId); // 送信者ID
        form.AddField("receiver_id", receiverId); // 受信者ID
        form.AddField("_token", csrfToken); // CSRFトークンを追加

        using (UnityWebRequest request = UnityWebRequest.Post(replyUrl, form))
        {
            request.SetRequestHeader("Accept", "application/json");
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
                Debug.LogError("Response code: " + request.responseCode);
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
            else
            {
                var response = request.downloadHandler.text;
                Debug.Log("Response: " + response);
                InquiryResponse inquiryResponse = JsonUtility.FromJson<InquiryResponse>(response);

                if (inquiryResponse != null)
                {
                    Debug.Log("Inquiry ID: " + inquiryResponse.id);
                    Debug.Log("Inquiry sent successfully: " + inquiryResponse.message);

                    // 成功メッセージを表示
                    successMessage.SetActive(true);

                    // 入力フィールドをクリア
                    replyInputField.text = "";
                }
            }
        }
    }

    public void OnOKButtonPressed()
    {
        successMessage.SetActive(false);
    }


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

    public class CsrfResponse
    {
        public string token;
    }

    [System.Serializable]
    public class Inquiry
    {
        public int id;
        public int inquiry_id;
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

    public class InquiryResponse
    {
        public int id;
        public string message;
    }
}
