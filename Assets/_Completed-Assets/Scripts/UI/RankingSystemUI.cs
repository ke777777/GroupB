using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;  // Newtonsoft.Jsonを使用する

public class RankingSystemUI : MonoBehaviour
{
    // サーバーのURL（サンプルとして指定）
    private string serverURL = "http://localhost:8080/update_game_count";

    // ユーザーIDとカラム名（例として固定値）
    private string user_id1 = "4523";
    private string column_name1 = "n_win";
    private string user_id2 = "4512";
    private string column_name2 = "n_win";
    private const string UserIdKey = "user_id";

    // UIの設定
    [SerializeField] private Text rankTextPrefab; // ランキングを表示するTextプレハブ
    [SerializeField] private RectTransform contentPanel; // ContentのRectTransform
    [SerializeField] private Text clientRankText; // クライアントのランキングを表示するText

    // フラグ
    private int pFlag1 = 0;
    private int pFlag2 = 0;
    private int clientFlag = 0; // クライアントのフラグ

    void Start()
    {
        // Unityの実行時にリクエストを送信
        UpdateGameCount(user_id1, column_name1, user_id2, column_name2);
    }

    // ゲームカウントを更新するリクエストを送信するメソッド
    private void UpdateGameCount(string user_id1, string column_name1, string user_id2, string column_name2)
    {
        // サーバーに送信するURLを作成
        string url = $"{serverURL}?user_id1={user_id1}&column_name1={column_name1}&user_id2={user_id2}&column_name2={column_name2}";

        // リクエストを送信するコルーチンを開始
        StartCoroutine(SendRequest(url));
    }

    // HTTPリクエストを送信し、レスポンスを処理するコルーチン
    private IEnumerator SendRequest(string url)
    {
        // UnityWebRequestでGETリクエストを作成
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            // リクエストを送信し、レスポンスを待機
            yield return www.SendWebRequest();

            // エラーチェック
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Request failed: " + www.error);
            }
            else
            {
                // レスポンスを取得
                string response = www.downloadHandler.text;

                // サーバーから返されたレスポンスを処理
                HandleServerResponse(response);
            }
        }
    }

    // サーバーから返されたJSONレスポンスを解析してデバッグ表示するメソッド
    private void HandleServerResponse(string response)
    {
        // サーバーから返されたJSONデータをそのまま表示（デバッグ用）
        Debug.Log("Received JSON: " + response);

        try
        {
            // JSONを手動で解析して必要な部分（top_10_ranking）を取り出す
            var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);

            // p_flag1とp_flag2を取得
            pFlag1 = jsonResponse.ContainsKey("p_flag1") ? int.Parse(jsonResponse["p_flag1"].ToString()) : 0;
            pFlag2 = jsonResponse.ContainsKey("p_flag2") ? int.Parse(jsonResponse["p_flag2"].ToString()) : 0;

            // "top_10_ranking"を取得してリスト形式にキャスト
            var top10Ranking = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonResponse["p_top_10_ranking"].ToString());

            // ランキングをUIに更新
            UpdateRanking(top10Ranking);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error parsing response: " + e.Message);
        }
    }

    // サーバーから受け取ったランキングデータを基にランキングを更新
    private void UpdateRanking(List<Dictionary<string, object>> top10Ranking)
    {
        // 現在のContent内のTextだけを削除（Vertical Layout Groupは削除しない）
        foreach (Transform child in contentPanel)
        {
            if (child.GetComponent<Text>() != null) // Textのオブジェクトだけ削除
            {
                Destroy(child.gameObject);
            }
        }

        // クライアントのuser_idを取得（PlayerPrefsに保存されている）
        int UserId = PlayerPrefs.GetInt(UserIdKey);
        string clientUserId = UserId.ToString();
        Debug.Log(clientUserId);
        
        // ランキングの表示
        foreach (var user in top10Ranking)
        {
            // 新しいTextオブジェクトを作成
            Text newRankText = Instantiate(rankTextPrefab, contentPanel);

            // サーバーから受け取ったデータを表示
            string userName = user["user_name"].ToString();
            int nWin = int.Parse(user["n_win"].ToString());
            int nLoss = user.ContainsKey("n_loss") && user["n_loss"] != null ? int.Parse(user["n_loss"].ToString()) : 0;
            float winRate = float.Parse(user["win_rate"].ToString());
            int userRank = int.Parse(user["user_rank"].ToString()); // user_rankを取り出す
            string userId = user["user_id"].ToString();

            // クライアントのuser_idと一致する場合はclientFlagを立てる
            if (userId == clientUserId)
            {
                clientFlag = 1; // クライアントのフラグを立てる
            }

            // テキストを設定
            newRankText.text = userRank + ". " + userName + ": Wins " + nWin + ", Losses " + nLoss + ", Win Rate " + winRate.ToString("F2");

            // Textのオーバーフロー設定
            newRankText.horizontalOverflow = HorizontalWrapMode.Overflow;
            newRankText.verticalOverflow = VerticalWrapMode.Overflow;
            newRankText.resizeTextForBestFit = true;  // フォントサイズを調整
        }

        // クライアントのフラグが0なら「You are out of the ranking」を表示、そうでなければクライアントのランキングを表示
        if (clientFlag == 0)
        {
            clientRankText.text = "You are out of the ranking";
        }
        else
        {
            // クライアントのランキング情報を表示
            var clientData = top10Ranking.Find(user => user["user_id"].ToString() == clientUserId);
            
            if (clientData != null)
            {
                // クライアントの情報を取得
                int nWin = int.Parse(clientData["n_win"].ToString());
                int nLoss = clientData.ContainsKey("n_loss") && clientData["n_loss"] != null ? int.Parse(clientData["n_loss"].ToString()) : 0;
                float winRate = float.Parse(clientData["win_rate"].ToString());
                int userRank = int.Parse(clientData["user_rank"].ToString());

                // クライアントのランキングを表示
                clientRankText.text = $"Your Rank: {userRank}, Wins: {nWin}, Losses: {nLoss}, Win Rate: {winRate:F2}";
                
                // pFlag1が0の場合、「rank up」を表示し、その部分だけ色を変更
                if (pFlag1 == 1)
                {
                    // "rank up"部分を赤色に変える（色の指定はお好みで調整）
                    clientRankText.text += " <color=#FF0000>(Rank Up)</color>";
                }
            }
        }

        // ContentPanelの高さを更新
        UpdateContentPanelHeight(top10Ranking.Count);
    }

    // ContentPanelの高さを、表示するTextの高さに基づいて設定
    private void UpdateContentPanelHeight(int itemCount)
    {
        // rankTextPrefabの高さを取得
        float rankTextHeight = rankTextPrefab.rectTransform.rect.height;

        // Contentの高さを、各ランキング項目の高さ * アイテム数に設定
        float newHeight = rankTextHeight * itemCount;

        // RectTransformの高さを変更
        contentPanel.sizeDelta = new Vector2(contentPanel.sizeDelta.x, newHeight);
    }
}