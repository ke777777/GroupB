using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
namespace Complete
{
    public class CountWinsManager : MonoBehaviour
    {
        public static CountWinsManager Instance { get; private set; }

        [SerializeField] private GameManager gameManager;
        [SerializeField] private Image[] player1WinStars;
        [SerializeField] private Image[] player2WinStars;

        private void Awake()
        {
            // シングルトンの設定
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            InitializeStars(player1WinStars);
            InitializeStars(player2WinStars);
            StartCoroutine(WaitForGameManagerAndSubscribe());
        }
        private IEnumerator WaitForGameManagerAndSubscribe()
        {
            while (gameManager == null || gameManager.m_Tanks == null || gameManager.m_Tanks.Count == 0)
            {
                Debug.Log("Waiting for GameManager and Tanks to be initialized...");
                yield return new WaitForSeconds(0.5f);
            }

            // 初期状態の勝利数を表示
            UpdateWinStars();
        }

        private void InitializeStars(Image[] starImages)
        {
            foreach (var star in starImages)
            {
                if (star != null)
                {
                    star.gameObject.SetActive(false);
                }
            }
        }

        public void UpdateWinStars()
        {
            if (gameManager == null || gameManager.m_Tanks == null || gameManager.m_Tanks.Count < 2)
            {
                Debug.LogWarning("GameManager or Tanks are not properly initialized.");
                return;
            }

            var tank1 = gameManager.m_Tanks.FirstOrDefault(t => t.m_PlayerNumber == 1);
            var tank2 = gameManager.m_Tanks.FirstOrDefault(t => t.m_PlayerNumber == 2);

            int player1Wins = tank1 != null ? tank1.m_Wins : 0;
            int player2Wins = tank2 != null ? tank2.m_Wins : 0;

            Debug.Log($"Updating win stars: Player1 Wins = {player1Wins}, Player2 Wins = {player2Wins}");
            UpdateStars(player1WinStars, Mathf.Min(player1Wins, player1WinStars.Length));
            UpdateStars(player2WinStars, Mathf.Min(player2Wins, player2WinStars.Length));
        }

        private void UpdateStars(Image[] starImages, int winCount)
        {
            for (int i = 0; i < starImages.Length; i++)
            {
                if (starImages[i] != null)
                {
                    starImages[i].gameObject.SetActive(i < winCount);
                }
                else
                {
                    Debug.LogWarning($"Star {i + 1} is null in the array.");
                }
            }
        }
    }
}
