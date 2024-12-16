using UnityEngine;
using System.Collections;
using Photon.Pun;
using Complete;

namespace Complete
{
    public class Cartridge : MonoBehaviourPun
    {
        public float blinkDuration = 3.0f; // ????????
        public float blinkInterval = 0.2f; // ?????

        [SerializeField] public CartridgeData cartridgeData;

        private Renderer cartridgeRenderer;

        private void Start()
        {
            // Renderer??????????
            cartridgeRenderer = GetComponent<Renderer>();

            // ?????
            StartCoroutine(BlinkAndDestroy());
        }

        private IEnumerator BlinkAndDestroy()
        {
            float elapsedTime = 0f;

            // ?????????????????
            while (elapsedTime < blinkDuration)
            {
                // Renderer????????????
                cartridgeRenderer.enabled = !cartridgeRenderer.enabled;

                // ????????
                yield return new WaitForSeconds(blinkInterval);

                // ???????
                elapsedTime += blinkInterval;
            }

            // ?????????
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