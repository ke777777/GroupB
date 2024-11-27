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

        public void SpawnCartridge(CartridgeData data)
        {
            if (data.cartridgePrefab == null)
            {
                Debug.LogError("cartridgePrefab��null�ł��BInstantiate�ł��܂���B");
                return;
            }

            Vector3 randomPosition = new Vector3(
                Random.Range(-40f, 40f), // X���W�͈̔�
                1f,                      // Y���W�i�Œ�j
                Random.Range(-40f, 40f)  // Z���W�͈̔�
            );

            string prefabName = data.cartridgePrefab.name;

            // �v���n�u����CustomPrefabPool�ɓo�^����Ă��邩�m�F
            CustomPrefabPool customPool = PhotonNetwork.PrefabPool as CustomPrefabPool;
            if (customPool == null)
            {
                Debug.LogError("PhotonNetwork.PrefabPool is not a CustomPrefabPool.");
                return;
            }

            if (!customPool.ContainsPrefab(prefabName))
            {
                Debug.LogError($"Prefab with name '{prefabName}' not found in CustomPrefabPool.");
                return;
            }

            GameObject cartridge = PhotonNetwork.Instantiate(prefabName, randomPosition, Quaternion.identity);
            if (cartridge != null)
            {
                Debug.Log($"Instantiated cartridge prefab '{prefabName}' at position {randomPosition}");
            }
            else
            {
                Debug.LogError($"Failed to instantiate cartridge prefab '{prefabName}'");
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

        // ����I��SpawnCartridge���Ăяo���R���[�`��
        private IEnumerator SpawnRoutine(CartridgeData data)
        {
            while (true)
            {
                SpawnCartridge(data);
                yield return new WaitForSeconds(data.spawnFrequency); // CartridgeData����p�x���擾
            }
        }

        private void Start()
        {
            gameManager = FindObjectOfType<GameManager>();
            if (cartridgeData == null || cartridgeData.cartridgePrefab == null)
            {
                Debug.LogError("cartridgeData�܂���cartridgePrefab���ݒ肳��Ă��܂���B�����𒆒f���܂��B");
                return;
            }

            if (gameManager != null)
            {
                gameManager.GameStateChanged += HandleGameStateChanged;
            }
            else
            {
                Debug.LogError("GameManager��������܂���B");
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
