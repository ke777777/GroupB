using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 10;
    public float lifetime = 30f;
    private float blinkStartTime = 25f;

    private void Start()
    {
        Invoke("DestroyPickup", lifetime);
    }

    private void Update()
    {
        if (Time.time > lifetime - blinkStartTime)
        {
            // �_�ł��鏈��
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // ��Ԃ��G����
        {
            TankAmmoManager ammoManager = other.GetComponent<TankAmmoManager>();
            if (ammoManager != null)
            {
                ammoManager.AddAmmo(ammoAmount);
                Destroy(gameObject); // �J�[�g���b�W������
            }
        }
    }

    private void DestroyPickup()
    {
        Destroy(gameObject); // ��莞�Ԍ�Ɏ�������
    }
}
