using UnityEngine;
using UnityEngine.UI;

public class PlayerStockArea : MonoBehaviour
{
    [SerializeField] private Image[] shellImage = new Image[10]; // 砲弾のストック用画像（１発）
    [SerializeField] private Image[] shellImages = new Image[4]; // 砲弾のストック用画像（１０発）
    [SerializeField] private Image[] mineImages = new Image[3];   // 地雷のストック用画像

    // UpdatePlayerStockAreaメソッド: 武器名とストック数に基づいてUIを更新
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
            Debug.LogWarning("未知の武器タイプ: " + weaponName);
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
                Debug.LogWarning("shellImagesの要素がnullです。");
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
                Debug.LogWarning("shellImageの要素がnullです。");
            }
        }
    }


    // 地雷ストックのUIを更新
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
                Debug.LogWarning("mineImagesの要素がnullです。");
            }
        }
    }
}
