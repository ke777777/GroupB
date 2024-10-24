using UnityEngine;
using UnityEngine.UI;

public class AmmoHUD : MonoBehaviour
{
    public Image ammoIcon; // アイコンのImage
    public Sprite ammoIcon1; // ストック数1用
    public Sprite ammoIcon10; // ストック数10用

    public void UpdateAmmoIcon(int currentAmmo)
    {
        // ここでストック数に応じてアイコンを変更
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

