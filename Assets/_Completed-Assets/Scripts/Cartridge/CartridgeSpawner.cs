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
            const int maxAttempts = 10; // �ʒu��T�����s�񐔂̏��
            for (int i = 0; i < maxAttempts; i++)
            {
                // �����_����X, Z���W���擾
                float randomX = Random.Range(-40f, 40f);
                float randomZ = Random.Range(-40f, 40f);
                Vector3 groundCheckPosition = new Vector3(randomX, 50f, randomZ);

                // �������Ƀ��C�L���X�g���Ēn�ʂ�T��
                if (Physics.Raycast(groundCheckPosition, Vector3.down, out RaycastHit hitInfo, Mathf.Infinity, groundLayer))
                {
                    // �n�ʃI�u�W�F�N�g�̃`�F�b�N
                    if (hitInfo.collider.CompareTag("Ground"))
                    {
                        Vector3 spawnPosition = hitInfo.point;
                        spawnPosition.y = 1f; // �J�[�g���b�W��y=1�ɐݒ�

                        // ������Ƀ��C�L���X�g���ď�Q�����Ȃ����Ƃ��m�F
                        if (!Physics.Raycast(spawnPosition + Vector3.up * 0.5f, Vector3.up, 1f))
                        {
                            return spawnPosition;
                        }
                    }
                }
            }
            return Vector3.zero; // �L���Ȉʒu��������Ȃ��ꍇ
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
