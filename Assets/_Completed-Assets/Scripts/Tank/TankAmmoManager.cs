using UnityEngine;
using UnityEngine.UI; // HUD�\���p

public class TankAmmoManager : MonoBehaviour
{
    public int maxAmmo = 50;
    public int currentAmmo = 10;
    public Text ammoText; // HUD�Ƃ��ĕ\������e�L�X�g

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
            // �C�e�𔭎˂��鏈��
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
