using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic; // �ǉ��FDictionary���g�p���邽��
using Photon.Pun;

namespace Complete
{
    public class HealthBarManager : MonoBehaviour
    {
        [SerializeField] private Slider myHealthSlider; // �����̗̑̓o�[
        [SerializeField] private Image myFillImage;     // �����̗̑̓o�[��Fill Image
        [SerializeField] private Slider opponentHealthSlider; // ����̗̑̓o�[
        [SerializeField] private Image opponentFillImage;     // ����̗̑̓o�[��Fill Image
        [SerializeField] private Color fullHealthColor = Color.green; // �̗͍ő厞�̐F
        [SerializeField] private Color zeroHealthColor = Color.red;   // �̗͍ŏ����̐F
        [SerializeField] private GameManager gameManager;    // �Q�[���}�l�[�W���[�̎Q��
        private int myPlayerNumber;
        private int opponentPlayerNumber;

        // �ǉ��F�C�x���g�n���h����ێ����鎫��
        private Dictionary<TankHealth, TankHealth.OnHealthChangedDelegate> healthChangedHandlers = new Dictionary<TankHealth, TankHealth.OnHealthChangedDelegate>();

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
            // �����̃^���N��������
            TankManager myTank = null;
            while (myTank == null)
            {
                myTank = gameManager.m_Tanks.FirstOrDefault(t => t.m_Instance != null && t.m_Instance.GetComponent<PhotonView>().IsMine);
                if (myTank == null)
                {
                    yield return new WaitForSeconds(0.5f);
                }
            }

            myPlayerNumber = myTank.m_PlayerNumber;

            // ����̃^���N��������
            TankManager opponentTank = gameManager.m_Tanks.FirstOrDefault(t => t.m_PlayerNumber != myPlayerNumber);

            if (opponentTank != null)
            {
                opponentPlayerNumber = opponentTank.m_PlayerNumber;
            }
            else
            {
                Debug.LogError("Opponent tank not found.");
                yield break;
            }

            // �����Ƒ���̃^���N�̃w���X�������N
            LinkHealth(myTank, myHealthSlider, myFillImage);
            LinkHealth(opponentTank, opponentHealthSlider, opponentFillImage);
        }

        private void LinkHealth(TankManager tankManager, Slider healthSlider, Image fillImage)
        {
            if (tankManager.m_Instance == null)
            {
                Debug.LogError($"Tank {tankManager.m_PlayerNumber} instance is null.");
                return;
            }

            var tankHealth = tankManager.m_Instance.GetComponent<TankHealth>();
            if (tankHealth == null)
            {
                Debug.LogError($"TankHealth component not found on Tank {tankManager.m_PlayerNumber}.");
                return;
            }

            // �C�x���g�n���h�����쐬���ĕێ�
            TankHealth.OnHealthChangedDelegate handler = (currentHealth, maxHealth, playerNumber) =>
            {
                HandleHealthChanged(currentHealth, maxHealth, playerNumber, healthSlider, fillImage);
            };

            // �C�x���g�Ƀn���h����o�^
            tankHealth.OnHealthChanged += handler;

            // �n���h���������ɕێ�
            healthChangedHandlers[tankHealth] = handler;

            // ������Ԃ�����
            HandleHealthChanged(tankHealth.CurrentHealth, tankHealth.StartingHealth, tankManager.m_PlayerNumber, healthSlider, fillImage);
        }

        private void HandleHealthChanged(float currentHealth, float maxHealth, int playerNumber, Slider healthSlider, Image fillImage)
        {
            float healthPercentage = currentHealth / maxHealth;

            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;

                if (fillImage != null)
                {
                    fillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, healthPercentage);
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
                        // �C�x���g�n���h��������
                        if (healthChangedHandlers.ContainsKey(tankHealth))
                        {
                            tankHealth.OnHealthChanged -= healthChangedHandlers[tankHealth];
                            healthChangedHandlers.Remove(tankHealth);
                        }

                    }
                }
            }
        }
    }
}
