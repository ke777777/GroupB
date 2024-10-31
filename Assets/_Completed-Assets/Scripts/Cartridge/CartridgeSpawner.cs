using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace Complete
{
    public class CartridgeSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject shellCartridgePrefab;
        private GameManager gameManager;
        private Coroutine spawnCoroutine;

        // �����_���Ȉʒu�Ƀv���n�u�𐶐����郁�\�b�h
        public void SpawnCartridge()
        {
            if (shellCartridgePrefab == null)
            {
                Debug.LogError("shellCartridgePrefab��null�ł��BInstantiate�ł��܂���B");
                return;
            }

            Vector3 randomPosition = new Vector3(
                Random.Range(-10f, 10f),  // X���W�͈̔�
                1f,                       // Y���W�i�Œ�j
                Random.Range(-10f, 10f)   // Z���W�͈̔�
            );
            Instantiate(shellCartridgePrefab, randomPosition, Quaternion.identity);
        }

        private void HandleGameStateChanged(GameManager.GameState newState)
        {
            if (newState == GameManager.GameState.RoundPlaying)
            {
                if (spawnCoroutine == null)
                {
                    spawnCoroutine = StartCoroutine(SpawnRoutine());
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
        private IEnumerator SpawnRoutine()
        {
            while (true)
            {
                SpawnCartridge(); // Cartridge�𐶐�
                yield return new WaitForSeconds(3f);  // ��莞�ԑҋ@�i3�b�Ԋu�Ő����j
            }
        }

        private void Start()
        {
            // GameManager�I�u�W�F�N�g���擾
            gameManager = FindObjectOfType<GameManager>();

            gameManager.GameStateChanged += HandleGameStateChanged;
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
