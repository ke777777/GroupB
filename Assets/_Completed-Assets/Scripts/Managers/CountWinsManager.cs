using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using Photon.Pun;
namespace Complete
{
    public class CountWinsManager : MonoBehaviour
    {
        public static CountWinsManager Instance { get; private set; }

        [SerializeField] private GameManager gameManager;
        [SerializeField] private Image[] myWinStars;
        [SerializeField] private Image[] opponentWinStars;
        private int myPlayerNumber;
        private int opponentPlayerNumber;

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
            InitializeStars(myWinStars);
            InitializeStars(opponentWinStars);
            StartCoroutine(WaitForGameManagerAndSetup());
        }
        private IEnumerator WaitForGameManagerAndSetup()
        {
            while (gameManager == null || gameManager.m_Tanks == null || gameManager.m_Tanks.Count == 0)
            {
                Debug.Log("Waiting for GameManager and Tanks to be initialized...");
                yield return new WaitForSeconds(0.5f);
            }

            // 自分と相手のプレイヤーナンバーを取得
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
            if (gameManager == null || gameManager.m_Tanks == null)
            {
                Debug.LogWarning("GameManager or Tanks are not properly initialized.");
                return;
            }

            var myTank = gameManager.m_Tanks.FirstOrDefault(t => t.m_PlayerNumber == myPlayerNumber);
            var opponentTank = gameManager.m_Tanks.FirstOrDefault(t => t.m_PlayerNumber != myPlayerNumber);

            int myWins = myTank != null ? myTank.m_Wins : 0;
            int opponentWins = opponentTank != null ? opponentTank.m_Wins : 0;

            Debug.Log($"Updating win stars: My Wins = {myWins}, Opponent Wins = {opponentWins}");
            UpdateStars(myWinStars, Mathf.Min(myWins, myWinStars.Length));
            UpdateStars(opponentWinStars, Mathf.Min(opponentWins, opponentWinStars.Length));
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
