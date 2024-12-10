using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class InquiryForm : MonoBehaviour
{
    public string apiUrl = "http://localhost/api/inquiry"; // APIのエンドポイントURL
    public string csrfTokenUrl = "http://localhost/csrf-token"; // CSRFトークンを取得するエンドポイント
    public string getMaxInquiryIdUrl = "http://localhost/api/getMaxInquiryId"; // 最大のinquiry_idを取得するエンドポイント
    public InputField titleInputField; // タイトルを入力するためのInputField
    public InputField contentInputField; // 内容を入力するためのInputField
    public GameObject successMessage; // Successメッセージのポップアップ

    private string csrfToken;
    private int nextInquiryId;

    private void Start()
    {
        StartCoroutine(GetCsrfToken());
        StartCoroutine(GetMaxInquiryId());
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

    private IEnumerator GetMaxInquiryId()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(getMaxInquiryIdUrl))
        {
            request.SetRequestHeader("Accept", "application/json");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<MaxInquiryIdResponse>(request.downloadHandler.text);
                nextInquiryId = response.maxId + 1; // 次のIDを計算
                Debug.Log("Next Inquiry ID: " + nextInquiryId);
            }
            else
            {
                Debug.LogError("Failed to get max inquiry ID: " + request.error);
                nextInquiryId = 1; // エラー時は1から開始する
            }
        }
    }

    public void SendInquiry()
    {
        if (string.IsNullOrEmpty(csrfToken))
        {
            Debug.LogError("CSRF token not available.");
            return;
        }

        if (nextInquiryId <= 0)
        {
            Debug.LogError("Invalid next inquiry ID.");
            return;
        }

        string title = titleInputField.text; // 入力されたタイトルを取得
        string content = contentInputField.text; // 入力された内容を取得
        string userID = PlayerPrefs.GetString("UserID", "Unknown User");
        string senderId = userID;
        string receiverId = "999999"; // 自動的に運営のIDを設定

        StartCoroutine(PostInquiry(nextInquiryId, senderId, receiverId, title, content));
    }

    IEnumerator PostInquiry(int inquiryId, string senderId, string receiverId, string title, string content)
    {
        WWWForm form = new WWWForm();
        form.AddField("inquiry_id", inquiryId); // inquiry_idを追加
        form.AddField("title", title);
        form.AddField("content", content);
        form.AddField("sender_id", senderId);
        form.AddField("receiver_id", receiverId);
        form.AddField("_token", csrfToken); // CSRFトークンを追加

        using (UnityWebRequest request = UnityWebRequest.Post(apiUrl, form))
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
                    titleInputField.text = "";
                    contentInputField.text = "";
                }
            }
        }
    }

    public void OnOKButtonPressed()
    {
        successMessage.SetActive(false);
    }

    [System.Serializable]
    public class CsrfResponse
    {
        public string token;
    }

    [System.Serializable]
    public class InquiryResponse
    {
        public int id;
        public string message;
    }

    [System.Serializable]
    public class MaxInquiryIdResponse
    {
        public int maxId;
    }
}
