using UnityEngine;
using UnityEngine.UI; // HUD表示用

public class TankAmmoManager : MonoBehaviour
{
    public int maxAmmo = 50;
    public int currentAmmo = 10;
    public Text ammoText; // HUDとして表示するテキスト

    private void Start()
    {
        UpdateAmmoUI();
    }

    public void Fire()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            UpdateAmmoUI();
            // 砲弾を発射する処理
        }
        else
        {
            Debug.Log("No ammo left!");
        }
    }

    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Min(currentAmmo + amount, maxAmmo);
        UpdateAmmoUI();
    }

    private void UpdateAmmoUI()
    {
        ammoText.text = "Ammo: " + currentAmmo.ToString();
    }
}
