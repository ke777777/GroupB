using UnityEngine;
using System.Collections;
using Photon.Pun;

namespace Complete
{
    public class CartridgeSpawner : MonoBehaviour
    {
        [SerializeField] private CartridgeData cartridgeData;
        private GameManager gameManager;
        private Coroutine spawnCoroutine;
        [SerializeField] private LayerMask groundLayer; // �n�ʂ𔻒肷�郌�C���[


        public void SpawnCartridge(CartridgeData data)
        {
            if (data.cartridgePrefab == null)
            {
                Debug.LogError("cartridgePrefab��null�ł��BInstantiate�ł��܂���B");
                return;
            }

            Vector3 spawnPosition = FindFlatPosition();

            /* Vector3 randomPosition = new Vector3(
                Random.Range(-40f, 40f), // X���W�͈̔�
                1f,                      // Y���W�i�Œ�j
                Random.Range(-40f, 40f)  // Z���W�͈̔�
            );
            */

            GameObject prefab = Resources.Load<GameObject>(data.cartridgePrefab.name);
            if (prefab != null)
            {
                PhotonNetwork.Instantiate(prefab.name, spawnPosition, Quaternion.identity);
            }
            else
            {
                Debug.LogError($"Prefab '{data.cartridgePrefab.name}' not found in Resources.");
            }
        }


        private void HandleGameStateChanged(GameManager.GameState newState)
        {
            if (newState == GameManager.GameState.RoundPlaying)
            {
                if (spawnCoroutine == null)
                {
                    spawnCoroutine = StartCoroutine(SpawnRoutine(cartridgeData));
                }
            }
            else
            {
                if (spawnCoroutine != null)
                {
                    StopCoroutine(spawnCoroutine);
                    spawnCoroutine = null;
                }
            }
        }

        private Vector3 FindFlatPosition()
        {
            // �����_����X, Z���W���擾
            float randomX = Random.Range(-40f, 40f);
            float randomZ = Random.Range(-40f, 40f);

            Vector3 randomPosition = new Vector3(randomX, 50f, randomZ); // Y�������ʒu����n�߂�
            Ray ray = new Ray(randomPosition, Vector3.down); // �������Ƀ��C���΂�

            // ���C�L���X�g�Œn�ʂ̈ʒu��T��
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, groundLayer))
            {
                // �q�b�g�����n�`�̖@�����擾���ĉ�]�𒲐�
                Vector3 flatPosition = hitInfo.point;
                Vector3 normal = hitInfo.normal;

                // �n�`�����R�łȂ��ꍇ�̓X�L�b�v�i�@����������ɋ߂��ꍇ�̂ݐ����j
                if (Vector3.Angle(normal, Vector3.up) > 10f) // ���R�ł���p�x�̂������l�i10�x�ȓ��j
                {
                    return Vector3.zero; // �s�K�؂Ȉʒu
                }

                return flatPosition;
            }

            return Vector3.zero; // �n�`��������Ȃ���Ζ����Ȓl��Ԃ�
        }
        private IEnumerator SpawnRoutine(CartridgeData data)
        {
            while (true)
            {
                SpawnCartridge(data);
                yield return new WaitForSeconds(data.spawnFrequency);
            }
        }

        private void Start()
        {
            gameManager = FindObjectOfType<GameManager>();

            if (gameManager != null)
            {
                gameManager.GameStateChanged += HandleGameStateChanged;
            }
        }

        private void OnDestroy()
        {
            if (gameManager != null)
            {
                gameManager.GameStateChanged -= HandleGameStateChanged;
            }
        }
    }
}
