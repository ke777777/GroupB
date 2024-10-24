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
            // 点滅する処理
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 戦車が触れると
        {
            TankAmmoManager ammoManager = other.GetComponent<TankAmmoManager>();
            if (ammoManager != null)
            {
                ammoManager.AddAmmo(ammoAmount);
                Destroy(gameObject); // カートリッジを消す
            }
        }
    }

    private void DestroyPickup()
    {
        Destroy(gameObject); // 一定時間後に自動消滅
    }
}
