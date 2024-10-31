using UnityEngine;
using System.Collections;

public class Cartridge : MonoBehaviour
{
    public float blinkDuration = 3.0f; // ���ł��鍇�v����
    public float blinkInterval = 0.2f; // ���ł̊Ԋu

    private Renderer cartridgeRenderer;

    private void Start()
    {
        // Renderer�R���|�[�l���g���擾
        cartridgeRenderer = GetComponent<Renderer>();

        // ���ł��J�n
        StartCoroutine(BlinkAndDestroy());
    }

    private IEnumerator BlinkAndDestroy()
    {
        float elapsedTime = 0f;

        // ���ł̍��v���Ԃ��o�߂���܂Ń��[�v
        while (elapsedTime < blinkDuration)
        {
            // Renderer�̗L���E��������؂�ւ�
            cartridgeRenderer.enabled = !cartridgeRenderer.enabled;

            // ���̖��ł܂őҋ@
            yield return new WaitForSeconds(blinkInterval);

            // �o�ߎ��Ԃ��X�V
            elapsedTime += blinkInterval;
        }

        // �J�[�g���b�W������
        Destroy(gameObject);
    }
}
