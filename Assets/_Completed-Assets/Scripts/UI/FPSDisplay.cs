using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    public Text fpsText; // UIのテキストオブジェクトを格納
    private float deltaTime;
    private float minFps = float.MaxValue; // 初期値を最大値に設定
    private float timer = 0f; // タイマー用の変数
    private bool recordFps = false; // FPSを記録するかどうかのフラグ

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f; // 平均フレーム時間計算

        // タイマーを更新
        timer += Time.deltaTime;

        // 1秒経過後にFPSの記録を開始
        if (timer > 5f)
        {
            recordFps = true;
        }

        // FPSを計算
        float fps = 1.0f / deltaTime;

        // 最低FPSを更新（記録が開始されている場合）
        if (recordFps && fps < minFps)
        {
            minFps = fps;
        }

        // 表示するテキストを更新
        fpsText.text = $"FPS: {Mathf.Ceil(fps).ToString()}  Min FPS: {(recordFps ? Mathf.Ceil(minFps).ToString() : "N/A")}";
    }
}
