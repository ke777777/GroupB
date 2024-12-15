using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StampManager : MonoBehaviour
{
    public GameObject stampPrefab;       // スタンプ用プレハブ
    public Sprite[] stamps;             // スタンプ画像リスト（6個の画像をInspectorで設定）
    public Canvas canvas;               // スタンプを表示するキャンバス
    public Transform[] stampPositions;  // 各スタンプボタンに対応する表示位置
    public Button[] stampButtons;       // スタンプボタンリスト

    private GameObject currentStamp;    // 現在表示中のスタンプ

    public void OnStampButtonClicked(int stampIndex)
    {
        // 前のスタンプを削除
        if (currentStamp != null)
        {
            Destroy(currentStamp);
        }

        // 指定された固定位置を取得
        Transform targetPosition = stampPositions[stampIndex];

        // ボタンの色を取得
        Color buttonColor = stampButtons[stampIndex].GetComponent<Image>().color;

        // スタンプを生成して設定
        currentStamp = Instantiate(stampPrefab, canvas.transform); // Canvas の子に配置
        RectTransform stampRectTransform = currentStamp.GetComponent<RectTransform>();

        // スタンプの位置を固定位置に設定
        stampRectTransform.position = targetPosition.position;

        // スタンプ画像と色を設定
        Image stampImage = currentStamp.GetComponent<Image>();
        stampImage.sprite = stamps[stampIndex];
        stampImage.color = buttonColor;

        // スタンプを一定時間後にフェードアウトさせる
        StartCoroutine(FadeOutStamp(currentStamp));
    }

    private IEnumerator FadeOutStamp(GameObject stamp)
    {
        CanvasGroup stampCanvasGroup = stamp.GetComponent<CanvasGroup>();
        if (stampCanvasGroup == null)
        {
            stampCanvasGroup = stamp.AddComponent<CanvasGroup>();
        }

        yield return new WaitForSeconds(5f); // 5秒間待機

        float fadeDuration = 1f; // フェードアウト時間
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            if (stamp == null)
            {
                yield break;
            }

            stampCanvasGroup.alpha = 1 - (elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 完全に透明になったら削除
        if (currentStamp == stamp) // 表示中のスタンプが一致していれば削除
        {
            Destroy(currentStamp);
            currentStamp = null;
        }
    }
}