using UnityEngine;
using UnityEngine.UI;

public class PlayerStockArea : MonoBehaviour
{
    [SerializeField] private Image[] shellImage = new Image[10]; // 1���̖C�e�A�C�R��
    [SerializeField] private Image[] shellImages = new Image[4]; // 10���̖C�e�A�C�R��
    [SerializeField] private Image[] mineImages = new Image[3];  // �n���A�C�R��

    /*
        [SerializeField] private GridLayoutGroup shellGridLayout; // 1���C�e�A�C�R���̃��C�A�E�g�����p
        [SerializeField] private GridLayoutGroup tenShellGridLayout; // 10���C�e�A�C�R���̃��C�A�E�g�����p
        [SerializeField] private GridLayoutGroup mineGridLayout;  // �n���A�C�R���̃��C�A�E�g�����p
        [SerializeField] private Vector2 baseResolution = new Vector2(1920, 1080); // ��𑜓x
        [SerializeField] private float scaleFactor = 1.0f; // �X�P�[�������p�̔{��
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
        AdjustIconSize(); // ���t���[���A��ʃT�C�Y�ɉ����ăA�C�R���̃T�C�Y�ƊԊu�𒲐�
    }
    private void AdjustLayout()
    {
        // Stock1Area������
        stock1Area.anchoredPosition = new Vector2(-300, 0);

        // Stock10Area�𒆉��ɔz�u
        stock10Area.anchoredPosition = new Vector2(0, 0);

        // MineArea���E��
        mineArea.anchoredPosition = new Vector2(300, 0);
    }
    private void AdjustIconSize()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // �X�N���[���𑜓x�ɉ������X�P�[���v�Z
        float scaleWidth = screenWidth / baseResolution.x;
        float scaleHeight = screenHeight / baseResolution.y;
        float scale = Mathf.Min(scaleWidth, scaleHeight) * scaleFactor;
       // scale = Mathf.Max(scale, 0.93f); // �S�̂̃X�P�[���ŏ��l
        // 1���C�e�A�C�R���̃T�C�Y�ƊԊu�𒲐�
        if (shellGridLayout != null)
        {
            shellGridLayout.cellSize = new Vector2(30, 60) * scale;
            shellGridLayout.spacing = new Vector2(5, 5) * scale;
            shellGridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            shellGridLayout.constraintCount = 10; // 1�s��10���ׂ�
        }

        // 10���C�e�A�C�R���̃T�C�Y�ƊԊu�𒲐�
        if (tenShellGridLayout != null)
        {
            tenShellGridLayout.cellSize = new Vector2(30, 60) * scale;
            tenShellGridLayout.spacing = new Vector2(5, 5) * scale;
            tenShellGridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            tenShellGridLayout.constraintCount = 4; // 1�s��4���ׂ�
        }

        // �n���A�C�R���̃T�C�Y�ƊԊu�𒲐�
        if (mineGridLayout != null)
        {
            mineGridLayout.cellSize = new Vector2(50, 60) * scale;
            mineGridLayout.spacing = new Vector2(5, 5) * scale;
            mineGridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            mineGridLayout.constraintCount = 3; // 1�s��3���ׂ�
        }
    } */
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

    // �C�e�X�g�b�N��UI���X�V
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

        // 10���A�C�R���̕\�����X�V
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

        // 1���A�C�R���̕\�����X�V
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
