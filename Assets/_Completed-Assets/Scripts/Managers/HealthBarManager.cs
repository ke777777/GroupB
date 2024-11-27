using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
            // �^���N�����������̂�҂�
            while (gameManager == null || gameManager.m_Tanks == null || gameManager.m_Tanks.Count == 0)
            {
                Debug.LogWarning("Waiting for GameManager and Tanks to be initialized...");
                yield return new WaitForSeconds(0.5f); // �����ҋ@���čă`�F�b�N
            }

            // �e�^���N�̃C���X�^���X�����������̂�҂�
            while (gameManager.m_Tanks.Exists(tank => tank.m_Instance == null))
            {
                Debug.LogWarning("Waiting for all Tank instances to be initialized...");
                yield return new WaitForSeconds(0.5f);
            }

            // �^���N�̌��N��Ԃ������N
            foreach (var tank in gameManager.m_Tanks)
            {
                if (tank.m_Instance == null)
                {
                    Debug.LogWarning($"Tank {tank.m_PlayerNumber} instance is null.");
                    continue; // �^���N�C���X�^���X���Ȃ��ꍇ�̓X�L�b�v
                }

                var tankHealth = tank.m_Instance.GetComponent<TankHealth>();
                if (tankHealth != null)
                {
                    // �C�x���g���T�u�X�N���C�u
                    tankHealth.OnHealthChanged += HandleHealthChanged;
                    HandleHealthChanged(tankHealth.CurrentHealth, tankHealth.StartingHealth, tank.m_PlayerNumber);

                    Debug.Log($"Linked TankHealth for Tank {tank.m_PlayerNumber}.");
                }
                else
                {
                    Debug.LogWarning($"TankHealth component not found on Tank {tank.m_PlayerNumber}.");
                }
            }
        }


        private void HandleHealthChanged(float currentHealth, float maxHealth, int playerNumber)
        {
            float healthPercentage = currentHealth / maxHealth;

            if (playerNumber == 1 && player1HealthSlider != null)
            {
                player1HealthSlider.value = currentHealth;

                if (player1FillImage != null)
                {
                    // �̗͊����ɉ����ĐF��ω�������
                    player1FillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, healthPercentage);
                }
            }
            else if (playerNumber == 2 && player2HealthSlider != null)
            {
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
