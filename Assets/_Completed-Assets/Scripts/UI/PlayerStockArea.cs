using UnityEngine;
using UnityEngine.UI;

public class PlayerStockArea : MonoBehaviour
{
    [SerializeField] private Image[] shellImages = new Image[10];
    [SerializeField] private Image[] shellsImages = new Image[4];


    public void UpdatePlayerStockArea(int stockCount)
    {
      // ストック数が0の場合の特別な処理
        if (stockCount == 0)
        {
            // すべてのシェル画像を非表示にする
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

        // 1の位と10の位を計算
        int tensPlace = (stockCount - 1) / 10;  // 10で割った商（0から始まる）
        int onesPlace = stockCount - (tensPlace * 10);  // 1の位（1から10）

        // 1の位のシェル画像の表示制御
        for (int i = 0; i < shellImages.Length; i++)
        {
            shellImages[i].gameObject.SetActive(i < onesPlace);
        }

        // 10の位のシェル画像の表示制御
        for (int i = 0; i < shellsImages.Length; i++)
        {
            shellsImages[i].gameObject.SetActive(i < tensPlace);
        }
    }

}
