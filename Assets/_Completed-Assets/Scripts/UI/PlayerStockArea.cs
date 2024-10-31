using UnityEngine;
using UnityEngine.UI;

public class PlayerStockArea : MonoBehaviour
{
    [SerializeField] private Image[] shellImages = new Image[10];
    [SerializeField] private Image[] shellsImages = new Image[4];


    public void UpdatePlayerStockArea(int stockCount)
    {
      // �X�g�b�N����0�̏ꍇ�̓��ʂȏ���
        if (stockCount == 0)
        {
            // ���ׂẴV�F���摜���\���ɂ���
            foreach (var shellImage in shellImages)
            {
                shellImage.gameObject.SetActive(false);
            }
            foreach (var shellsImage in shellsImages)
            {
                shellsImage.gameObject.SetActive(false);
            }
            return;
        }

        // 1�̈ʂ�10�̈ʂ��v�Z
        int tensPlace = (stockCount - 1) / 10;  // 10�Ŋ��������i0����n�܂�j
        int onesPlace = stockCount - (tensPlace * 10);  // 1�̈ʁi1����10�j

        // 1�̈ʂ̃V�F���摜�̕\������
        for (int i = 0; i < shellImages.Length; i++)
        {
            shellImages[i].gameObject.SetActive(i < onesPlace);
        }

        // 10�̈ʂ̃V�F���摜�̕\������
        for (int i = 0; i < shellsImages.Length; i++)
        {
            shellsImages[i].gameObject.SetActive(i < tensPlace);
        }
    }

}
