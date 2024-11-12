using UnityEngine;
using UnityEngine.UI;

public class PlayerStockArea : MonoBehaviour
{
    [SerializeField] private Image[] shellImage = new Image[10]; // �C�e�̃X�g�b�N�p�摜�i�P���j
    [SerializeField] private Image[] shellImages = new Image[4]; // �C�e�̃X�g�b�N�p�摜�i�P�O���j
    [SerializeField] private Image[] mineImages = new Image[3];   // �n���̃X�g�b�N�p�摜

    // UpdatePlayerStockArea���\�b�h: ���햼�ƃX�g�b�N���Ɋ�Â���UI���X�V
    public void UpdatePlayerStockArea(string weaponName, int currentStock)
    {
        if (weaponName == "Shell")
        {
            UpdateShellStock(currentStock);
        }
        else if (weaponName == "Mine")
        {
            UpdateMineStock(currentStock);
        }
        else
        {
            Debug.LogWarning("���m�̕���^�C�v: " + weaponName);
        }
    }
    private void UpdateShellStock(int stockCount)
    {
        int tensPlace = 0;
        int onesPlace = 0;

        if (stockCount <= 10)
        {
        onesPlace = stockCount;
        }
        else
        {
            tensPlace = (stockCount - 1) / 10;
            onesPlace = stockCount - tensPlace * 10;
        }

        for (int i = 0; i < shellImages.Length; i++)
        {
            if (shellImages[i] != null)
            {
                shellImages[i].gameObject.SetActive(i < tensPlace);
            }
            else
            {
                Debug.LogWarning("shellImages�̗v�f��null�ł��B");
            }
        }

        for (int i = 0; i < shellImage.Length; i++)
        {
            if (shellImage[i] != null)
            {
                shellImage[i].gameObject.SetActive(i < onesPlace);
            }
            else
            {
                Debug.LogWarning("shellImage�̗v�f��null�ł��B");
            }
        }
    }


    // �n���X�g�b�N��UI���X�V
    private void UpdateMineStock(int stockCount)
    {
        for (int i = 0; i < mineImages.Length; i++)
        {
            if (mineImages[i] != null)
            {
                mineImages[i].gameObject.SetActive(i < stockCount);
            }
            else
            {
                Debug.LogWarning("mineImages�̗v�f��null�ł��B");
            }
        }
    }
}
