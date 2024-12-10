using UnityEngine;
using UnityEngine.UI;

public class PlayerStockArea : MonoBehaviour
{
    [SerializeField] private Image[] singleShells; // Shell1, Shell2, ..., Shell10
    [SerializeField] private Image[] groupedShells; // Shells10, Shells20, ..., Shells40


    private void Start()
    {
        InitializeHUD(10); // 初期状態を設定（10ストックでスタート）
    }

    /// <summary>
    /// HUDを初期化し、10ストックの表示を設定します。
    /// </summary>
    /// <param name="initialStock">初期の砲弾ストック数</param>
    private void InitializeHUD(int initialStock)
    {
        // 全ての個別シェルを非表示
        foreach (var shell in singleShells)
        {
            shell.gameObject.SetActive(false);
        }

        // 全てのグループ化シェルを非表示
        for (int i = 0; i < groupedShells.Length; i++)
        {
            groupedShells[i].gameObject.SetActive(i == (initialStock / 10 - 1)); // 初期状態のグループだけを表示
        }
    }

    /// <summary>
    /// 砲弾ストック数を受け取り、HUDの表示を更新します。
    /// </summary>
    /// <param name="stockCount">現在の砲弾ストック数</param>
    public void UpdatePlayerStockArea(int stockCount)
    {
        // 個別のShellを制御
        for (int i = 0; i < singleShells.Length; i++)
        {
            singleShells[i].gameObject.SetActive(i < stockCount % 10); // 余りで表示を判定
        }

        // グループ化されたShellを制御
        for (int i = 0; i < groupedShells.Length; i++)
        {
            groupedShells[i].gameObject.SetActive(i < stockCount / 10); // 商で表示を判定
        }
    }
}
