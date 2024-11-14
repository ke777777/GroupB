using UnityEngine;
using UnityEngine.UI;

public class PlayerStockArea : MonoBehaviour
{
    [SerializeField] private Image[] shellImage = new Image[10]; // 1発の砲弾アイコン
    [SerializeField] private Image[] shellImages = new Image[4]; // 10発の砲弾アイコン
    [SerializeField] private Image[] mineImages = new Image[3];  // 地雷アイコン

/*
    [SerializeField] private GridLayoutGroup shellGridLayout; // 1発砲弾アイコンのレイアウト調整用
    [SerializeField] private GridLayoutGroup tenShellGridLayout; // 10発砲弾アイコンのレイアウト調整用
    [SerializeField] private GridLayoutGroup mineGridLayout;  // 地雷アイコンのレイアウト調整用
    [SerializeField] private Vector2 baseResolution = new Vector2(1920, 1080); // 基準解像度
    [SerializeField] private float scaleFactor = 1.0f; // スケール調整用の倍率
    [SerializeField] private RectTransform stock1Area;
    [SerializeField] private RectTransform stock10Area;
    [SerializeField] private RectTransform mineArea;
*/

    /*private void Start()
    {
        AdjustLayout();
        AdjustIconSize();
    }

    private void Update()
    {
        AdjustIconSize(); // 毎フレーム、画面サイズに応じてアイコンのサイズと間隔を調整
    }
    private void AdjustLayout()
    {
        // Stock1Areaを左寄せ
        stock1Area.anchoredPosition = new Vector2(-300, 0);

        // Stock10Areaを中央に配置
        stock10Area.anchoredPosition = new Vector2(0, 0);

        // MineAreaを右寄せ
        mineArea.anchoredPosition = new Vector2(300, 0);
    }
    private void AdjustIconSize()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // スクリーン解像度に応じたスケール計算
        float scaleWidth = screenWidth / baseResolution.x;
        float scaleHeight = screenHeight / baseResolution.y;
        float scale = Mathf.Min(scaleWidth, scaleHeight) * scaleFactor;
       // scale = Mathf.Max(scale, 0.93f); // 全体のスケール最小値
        // 1発砲弾アイコンのサイズと間隔を調整
        if (shellGridLayout != null)
        {
            shellGridLayout.cellSize = new Vector2(30, 60) * scale;
            shellGridLayout.spacing = new Vector2(5, 5) * scale;
            shellGridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            shellGridLayout.constraintCount = 10; // 1行に10個並べる
        }

        // 10発砲弾アイコンのサイズと間隔を調整
        if (tenShellGridLayout != null)
        {
            tenShellGridLayout.cellSize = new Vector2(30, 60) * scale;
            tenShellGridLayout.spacing = new Vector2(5, 5) * scale;
            tenShellGridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            tenShellGridLayout.constraintCount = 4; // 1行に4個並べる
        }

        // 地雷アイコンのサイズと間隔を調整
        if (mineGridLayout != null)
        {
            mineGridLayout.cellSize = new Vector2(50, 60) * scale;
            mineGridLayout.spacing = new Vector2(5, 5) * scale;
            mineGridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            mineGridLayout.constraintCount = 3; // 1行に3個並べる
        }
    } */

    // 武器名とストック数に基づいてUIを更新
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

    // 砲弾ストックのUIを更新
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

        // 10発アイコンの表示を更新
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

        // 1発アイコンの表示を更新
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
