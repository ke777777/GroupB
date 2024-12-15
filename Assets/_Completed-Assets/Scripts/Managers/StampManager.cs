using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StampManager : MonoBehaviour
{
    public GameObject stampPrefab;       // �X�^���v�p�v���n�u
    public Sprite[] stamps;             // �X�^���v�摜���X�g�i6�̉摜��Inspector�Őݒ�j
    public Canvas canvas;               // �X�^���v��\������L�����o�X
    public Transform[] stampPositions;  // �e�X�^���v�{�^���ɑΉ�����\���ʒu
    public Button[] stampButtons;       // �X�^���v�{�^�����X�g

    private GameObject currentStamp;    // ���ݕ\�����̃X�^���v

    public void OnStampButtonClicked(int stampIndex)
    {
        // �O�̃X�^���v���폜
        if (currentStamp != null)
        {
            Destroy(currentStamp);
        }

        // �w�肳�ꂽ�Œ�ʒu���擾
        Transform targetPosition = stampPositions[stampIndex];

        // �{�^���̐F���擾
        Color buttonColor = stampButtons[stampIndex].GetComponent<Image>().color;

        // �X�^���v�𐶐����Đݒ�
        currentStamp = Instantiate(stampPrefab, canvas.transform); // Canvas �̎q�ɔz�u
        RectTransform stampRectTransform = currentStamp.GetComponent<RectTransform>();

        // �X�^���v�̈ʒu���Œ�ʒu�ɐݒ�
        stampRectTransform.position = targetPosition.position;

        // �X�^���v�摜�ƐF��ݒ�
        Image stampImage = currentStamp.GetComponent<Image>();
        stampImage.sprite = stamps[stampIndex];
        stampImage.color = buttonColor;

        // �X�^���v����莞�Ԍ�Ƀt�F�[�h�A�E�g������
        StartCoroutine(FadeOutStamp(currentStamp));
    }

    private IEnumerator FadeOutStamp(GameObject stamp)
    {
        CanvasGroup stampCanvasGroup = stamp.GetComponent<CanvasGroup>();
        if (stampCanvasGroup == null)
        {
            stampCanvasGroup = stamp.AddComponent<CanvasGroup>();
        }

        yield return new WaitForSeconds(5f); // 5�b�ԑҋ@

        float fadeDuration = 1f; // �t�F�[�h�A�E�g����
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

        // ���S�ɓ����ɂȂ�����폜
        if (currentStamp == stamp) // �\�����̃X�^���v����v���Ă���΍폜
        {
            Destroy(currentStamp);
            currentStamp = null;
        }
    }
}