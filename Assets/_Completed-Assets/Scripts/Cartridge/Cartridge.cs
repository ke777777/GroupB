using UnityEngine;
using System.Collections;
using Photon.Pun;
using Complete;

namespace Complete
{
    public class Cartridge : MonoBehaviourPun
    {
        public float blinkDuration = 3.0f; // 明滅する合計時間
        public float blinkInterval = 0.2f; // 明滅の間隔

        [SerializeField] public CartridgeData cartridgeData;

        private Renderer cartridgeRenderer;

        private void Start()
        {
            // Rendererコンポーネントを取得
            cartridgeRenderer = GetComponent<Renderer>();

            // 明滅を開始
            StartCoroutine(BlinkAndDestroy());
        }

        private IEnumerator BlinkAndDestroy()
        {
            float elapsedTime = 0f;

            // 明滅の合計時間が経過するまでループ
            while (elapsedTime < blinkDuration)
            {
                // Rendererの有効・無効化を切り替え
                cartridgeRenderer.enabled = !cartridgeRenderer.enabled;

                // 次の明滅まで待機
                yield return new WaitForSeconds(blinkInterval);

                // 経過時間を更新
                elapsedTime += blinkInterval;
            }

            // カートリッジを消滅
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