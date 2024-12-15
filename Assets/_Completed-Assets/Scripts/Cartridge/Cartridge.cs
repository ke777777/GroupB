using UnityEngine;
using System.Collections;
using Photon.Pun;
using Complete;

namespace Complete
{
    public class Cartridge : MonoBehaviourPun
    {
        public float blinkDuration = 3.0f; // ���ł��鍇�v����
        public float blinkInterval = 0.2f; // ���ł̊Ԋu

        [SerializeField] public CartridgeData cartridgeData;

        private Renderer cartridgeRenderer;

        private void Start()
        {
            // Renderer�R���|�[�l���g���擾
            cartridgeRenderer = GetComponent<Renderer>();

            // ���ł��J�n
            StartCoroutine(BlinkAndDestroy());
        }

        private IEnumerator BlinkAndDestroy()
        {
            float elapsedTime = 0f;

            // ���ł̍��v���Ԃ��o�߂���܂Ń��[�v
            while (elapsedTime < blinkDuration)
            {
                // Renderer�̗L���E��������؂�ւ�
                cartridgeRenderer.enabled = !cartridgeRenderer.enabled;

                // ���̖��ł܂őҋ@
                yield return new WaitForSeconds(blinkInterval);

                // �o�ߎ��Ԃ��X�V
                elapsedTime += blinkInterval;
            }

            // �J�[�g���b�W������
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                TankShooting tankShooting = other.GetComponent<TankShooting>();
                if (tankShooting != null && tankShooting.photonView.IsMine)
                {
                    tankShooting.GainingWeaponNumber(cartridgeData.weaponType);
                    photonView.RPC(nameof(DestroyCartridge), RpcTarget.MasterClient);
                }
            }
        }

        [PunRPC]
        private void DestroyCartridge()
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}