using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class PlayerStockArea : MonoBehaviour
    {
        [SerializeField] private Image[] shells; // Shell1, Shell2, ..., Shell10
        [SerializeField] private Image[] shells10; // Shells10, Shells20, Shells30, Shells40

        // 砲弾のストック数を引数に、UIを更新するメソッド
        public void UpdatePlayerStockArea(int stockCount)
        {
            // stockCountが負にならないようにする
            stockCount = Mathf.Max(stockCount, 0);

            // 1の位と10の位を取得
            int onesPlace = stockCount % 10; // 1の位
            int tensPlace = stockCount / 10;  // 10の位

            // 条件に基づいて表示を決定
            if (stockCount == 0)
            {
                // ストック数が0の場合
                for (int i = 0; i < shells.Length; i++)
                {
                    shells[i].gameObject.SetActive(false);
                }
                for (int i = 0; i < shells10.Length; i++)
                {
                    shells10[i].gameObject.SetActive(false);
                }
            }
            else if (onesPlace == 0)
            {
                // 1の位が0の場合
                for (int i = 0; i < shells.Length; i++)
                {
                    shells[i].gameObject.SetActive(i < 10);
                }
                for (int i = 0; i < shells10.Length; i++)
                {
                    shells10[i].gameObject.SetActive(i < tensPlace - 1);
                }
            }
            else
            {
                // 1の位が0でない場合
                for (int i = 0; i < shells.Length; i++)
                {
                    shells[i].gameObject.SetActive(i < onesPlace);
                }
                for (int i = 0; i < shells10.Length; i++)
                {
                    shells10[i].gameObject.SetActive(i < tensPlace);
                }
            }
        }
    }
}