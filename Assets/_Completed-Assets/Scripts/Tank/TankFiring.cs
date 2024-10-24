using UnityEngine;

public class TankFiring : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float minPower = 10f;
    public float maxPower = 50f;
    private float power;
    private bool chargingPower = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            chargingPower = true;
        }

        if (chargingPower)
        {
            // 飛距離ゲージを反復させる
            power = Mathf.PingPong(Time.time * maxPower, maxPower - minPower) + minPower;
            // HUDに飛距離ゲージを表示する処理
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            FireProjectile();
            chargingPower = false;
        }
    }

    private void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = firePoint.forward * power;
        // 飛距離に基づいて発射する
    }
}

