using UnityEngine;
using UnityEngine.UI;

public class AmmoHUD : MonoBehaviour
{
    public Image ammoIcon; // �A�C�R����Image
    public Sprite ammoIcon1; // �X�g�b�N��1�p
    public Sprite ammoIcon10; // �X�g�b�N��10�p

    public void UpdateAmmoIcon(int currentAmmo)
    {
        // �����ŃX�g�b�N���ɉ����ăA�C�R����ύX
        if (currentAmmo >= 10)
        {
            ammoIcon.sprite = ammoIcon10;
        }
        else
        {
            ammoIcon.sprite = ammoIcon1;
        }
    }
}

