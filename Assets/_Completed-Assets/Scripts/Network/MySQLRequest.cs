using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;


public class MySQLRequest : MonoBehaviour
{
    [SerializeField] private string baseUrl = "http://localhost:8080"; // サーバーURLとポートを設定

    // ゲーム結果更新API呼び出し
    public void UpdateGameCount(int userId1, string columnName1, int userId2, string columnName2)
    {
        string url = $"{baseUrl}/update_game_count?user_id1={userId1}&column_name1={columnName1}&user_id2={userId2}&column_name2={columnName2}";
        StartCoroutine(SendRequest(url));
    }

    // プレイヤーデータ取得API呼び出し
    public void GetPlayerData(int userId, System.Action<string> onSuccess, System.Action<string> onError = null)
    {
        string url = $"{baseUrl}/get_column_value?user_id={userId}&column_name=user_name";
        StartCoroutine(SendPlayerDataRequest(url, onSuccess, onError));
    }

    private IEnumerator SendRequest(string url)
    {
        Debug.Log($"Sending request to: {url}");
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Request Successful: {request.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"Request Failed: {request.error}");
            }
        }
    }

    private IEnumerator SendPlayerDataRequest(string url, System.Action<string> onSuccess, System.Action<string> onError)
    {
        Debug.Log($"Sending request to: {url}");
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Request Successful. Raw Response: {request.downloadHandler.text}");
                try
                {
                    // サーバーからのレスポンスをそのまま返す
                    onSuccess?.Invoke(request.downloadHandler.text);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to parse response: {e.Message}");
                    onError?.Invoke("Failed to parse response");
                }
            }
            else
            {
                Debug.LogError($"Request Failed: {request.error}");
                onError?.Invoke(request.error);
            }
        }
    }
}
