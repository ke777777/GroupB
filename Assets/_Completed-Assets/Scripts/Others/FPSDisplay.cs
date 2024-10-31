using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    public Text fpsText;  // FPS��\������Text�R���|�[�l���g

    private float deltaTime = 0.0f;

    void Update()
    {
        // �t���[���Ԃ̌o�ߎ��Ԃ��v��
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        // FPS���v�Z
        float fps = 1.0f / deltaTime;

        // �e�L�X�g��FPS��\��
        fpsText.text = $"FPS: {fps:F1}";
    }
}
