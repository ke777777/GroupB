using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    public Text fpsText;  // FPSを表示するTextコンポーネント

    private float deltaTime = 0.0f;

    void Update()
    {
        // フレーム間の経過時間を計測
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        // FPSを計算
        float fps = 1.0f / deltaTime;

        // テキストにFPSを表示
        fpsText.text = $"FPS: {fps:F1}";
    }
}
