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
            // �򋗗��Q�[�W�𔽕�������
            power = Mathf.PingPong(Time.time * maxPower, maxPower - minPower) + minPower;
            // HUD�ɔ򋗗��Q�[�W��\�����鏈��
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
        // �򋗗��Ɋ�Â��Ĕ��˂���
    }
}

