using UnityEngine;

public class TankTPSCamera : MonoBehaviour
{
    public Transform turretTransform; // �C����Transform
    public Vector3 offset = new Vector3(0, 5, -7); // �J�����̃I�t�Z�b�g�i�΂ߌ��̈ʒu�j
    public float smoothSpeed = 0.125f; // �J�����̒Ǐ]���x

    private void LateUpdate()
    {
        if (turretTransform != null)
        {
            // �^�[�Q�b�g�ʒu�Ɋ�Â��ăJ�����̈ʒu���v�Z
            Vector3 desiredPosition = turretTransform.position + turretTransform.rotation * offset;

            // �X���[�Y�ɃJ�����ʒu����
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            // �J��������ɖC���������悤��
            transform.LookAt(turretTransform);
        }
    }
}
