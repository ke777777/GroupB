using UnityEngine;
using UnityEngine.UI;
using System.Collections;
namespace Complete
{
    public class CountWins : MonoBehaviour
    {
        // シングルトンのインスタンス
        public static CountWins Instance { get; private set; }

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

            if (gameManager == null || gameManager.m_Tanks.Count < 2)
            {
                return;
            }

            int player1Wins = gameManager.m_Tanks[0].m_Wins;
            int player2Wins = gameManager.m_Tanks[1].m_Wins;
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
