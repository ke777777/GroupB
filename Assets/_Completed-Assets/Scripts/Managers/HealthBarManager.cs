using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using Photon.Pun;

namespace Complete
{
    public class HealthBarManager : MonoBehaviour
    {
        [SerializeField] private Slider player1HealthSlider; // �v���C���[1�̗̑̓o�[
        [SerializeField] private Image player1FillImage;     // �v���C���[1�̗̑̓o�[��Fill Image
        [SerializeField] private Slider player2HealthSlider; // �v���C���[2�̗̑̓o�[
        [SerializeField] private Image player2FillImage;     // �v���C���[2�̗̑̓o�[��Fill Image
        [SerializeField] private Color fullHealthColor = Color.green; // �̗͍ő厞�̐F
        [SerializeField] private Color zeroHealthColor = Color.red;   // �̗͍ŏ����̐F
        [SerializeField] private GameManager gameManager;    // �Q�[���}�l�[�W���[�̎Q��

        private void OnEnable()
        {
            if (gameManager == null || gameManager.m_Tanks == null || gameManager.m_Tanks.Count == 0)
            {
                Debug.LogError("GameManager or tanks data is not properly initialized.");
                return;
            }

            StartCoroutine(WaitForTanksAndLinkHealth());
        }

        private IEnumerator WaitForTanksAndLinkHealth()
        {
            // GameManager �̏������҂�
            while (gameManager == null || gameManager.m_Tanks == null || gameManager.m_Tanks.Count == 0)
            {
                Debug.Log("Waiting for GameManager and Tanks to be initialized...");
                yield return new WaitForSeconds(0.5f); // ���̃t���[���܂őҋ@
            }

            // �e�^���N�̃C���X�^���X�����҂�
            bool allInstancesInitialized = false;
            while (!allInstancesInitialized)
            {
                allInstancesInitialized = true;
                foreach (var tank in gameManager.m_Tanks)
                {
                    if (tank.m_Instance == null)
                    {
                        Debug.Log($"Tank {tank.m_PlayerNumber} instance is not initialized yet. Retrying...");
                        allInstancesInitialized = false;
                        break;
                    }
                }
                yield return new WaitForSeconds(0.5f);
            }

            Debug.Log("All Tank instances have been initialized. Linking health...");

            // �e�^���N�̃w���X��Ԃ������N
            foreach (var tank in gameManager.m_Tanks)
            {
                if (tank.m_Instance == null)
                {
                    Debug.LogError($"Tank {tank.m_PlayerNumber} instance is null. Skipping...");
                    continue; // �C���X�^���X�����݂��Ȃ��ꍇ�̓X�L�b�v
                }

                var tankHealth = tank.m_Instance.GetComponent<TankHealth>();
                if (tankHealth == null)
                {
                    Debug.LogError($"TankHealth component not found on Tank {tank.m_PlayerNumber}. Skipping...");
                    continue; // TankHealth �R���|�[�l���g��������Ȃ��ꍇ�̓X�L�b�v
                }

                // �w���X�ύX�C�x���g�ɃT�u�X�N���C�u
                tankHealth.OnHealthChanged += HandleHealthChanged;

                // ������Ԃ�����
                HandleHealthChanged(tankHealth.CurrentHealth, tankHealth.StartingHealth, tank.m_PlayerNumber);

                Debug.Log($"Linked health for Tank {tank.m_PlayerNumber}.");
            }

            Debug.Log("Health linking completed for all Tanks.");
        }


        private void HandleHealthChanged(float currentHealth, float maxHealth, int playerNumber)
        {
            float healthPercentage = currentHealth / maxHealth;

            if (playerNumber == 1 && player1HealthSlider != null)
            {

                player1HealthSlider.maxValue = maxHealth;
                player1HealthSlider.value = currentHealth;

                if (player1FillImage != null)
                {
                    // �̗͊����ɉ����ĐF��ω�������
                    player1FillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, healthPercentage);
                }
            }
            else if (playerNumber == 2 && player2HealthSlider != null)
            {
                player2HealthSlider.maxValue = maxHealth;
                player2HealthSlider.value = currentHealth;

                if (player2FillImage != null)
                {
                    // �̗͊����ɉ����ĐF��ω�������
                    player2FillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, healthPercentage);
                }
            }
        }

        private void OnDisable()
        {
            if (gameManager != null)
            {
                foreach (var tank in gameManager.m_Tanks)
                {
                    if (tank.m_Instance == null)
                        continue;

                    var tankHealth = tank.m_Instance.GetComponent<TankHealth>();
                    if (tankHealth != null)
                    {
                        tankHealth.OnHealthChanged -= HandleHealthChanged;
                    }
                }
            }
        }
    }
}
