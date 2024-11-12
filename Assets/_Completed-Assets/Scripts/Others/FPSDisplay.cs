using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    public Text FPSText;
    public Text minFPSText;

    private float minFPS = float.MaxValue;
    private float elapsedTime = 0.0f;
    private int frameCount = 0;
    private float timeSinceLastUpdate = 0.0f;

    void Update()
    {
        frameCount++;
        elapsedTime += Time.deltaTime;
        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate >= 0.5f)
        {
            float fps = frameCount / timeSinceLastUpdate;
            FPSText.text = $"FPS: {fps:F1}";

            if (elapsedTime > 2.0f && fps < minFPS) //�N�����オ0�ɋ߂�����2�b�ォ��
                minFPS = fps;

            minFPSText.text = elapsedTime > 2.0f ? $"Min FPS: {minFPS:F1}" : "Min FPS: N/A";

            frameCount = 0;
            timeSinceLastUpdate = 0.0f;
        }
    }
}

