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
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            if (data.cartridgePrefab == null)
            {
                Debug.LogError("cartridgePrefab?null???Instantiate??????");
                return;
            }

            Vector3 randomPosition = new Vector3(
                Random.Range(-40f, 40f), // X?????
                1f,                      // Y??????
                Random.Range(-40f, 40f)  // Z?????
            );

            GameObject prefab = Resources.Load<GameObject>(data.cartridgePrefab.name);

            if (prefab != null)
            {
                GameObject cartridge = PhotonNetwork.Instantiate(prefab.name, randomPosition, Quaternion.identity);
                /*if (cartridge != null)
                {
                    Debug.Log($"Instantiated cartridge prefab '{prefab.name}' at position {randomPosition}");
                }
                else
                {
                    Debug.LogError($"Failed to instantiate cartridge prefab '{prefab.name}'");
                }*/
            }
            else
            {
                Debug.LogError($"Prefab '{data.cartridgePrefab.name}' not found in Resources.");
            }
        }

        private void HandleGameStateChanged(GameManager.GameState newState)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

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
            if (!PhotonNetwork.IsMasterClient)
            {
                enabled = false; // ???????????????????????
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