using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

namespace Complete
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public int m_NumRoundsToWin = 5;            // The number of rounds a single player has to win to win the game.
        public float m_StartDelay = 3f;             // The delay between the start of RoundStarting and RoundPlaying phases.
        public float m_EndDelay = 3f;               // The delay between the end of RoundPlaying and RoundEnding phases.
        public CameraControl m_CameraControl;       // Reference to the CameraControl script for control during different phases.
        public Text m_MessageText;                  // Reference to the overlay Text to display winning text, etc.
        public GameObject m_TankPrefab;             // Reference to the prefab the players will control.
        // public TankManager[] m_Tanks;               // A collection of managers for enabling and disabling different aspects of the tanks.
        public List<TankManager> m_Tanks; // ????????????????????
        [SerializeField] private GameObject minePrefab;
        [SerializeField] private GameObject completeShellPrefab;
        [SerializeField] private GameObject mineCartridgePrefab;
        [SerializeField] private GameObject shellCartridgePrefab;

        private int m_RoundNumber;                  // Which round the game is currently on.
        private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts.
        private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round or game ends.
        private TankManager m_RoundWinner;          // Reference to the winner of the current round.  Used to make an announcement of who won.
        private TankManager m_GameWinner;           // Reference to the winner of the game.  Used to make an announcement of who won.
        public enum GameState
        {
            RoundStarting, RoundPlaying, RoundEnding
        }
        public GameState CurrentGameState { get; private set; }
        public delegate void OnGameStateChanged(GameState newGameState);
        public event OnGameStateChanged GameStateChanged;

        private void Start()
        {
            m_StartWait = new WaitForSeconds(m_StartDelay);
            m_EndWait = new WaitForSeconds(m_EndDelay);


            if (PhotonNetwork.InRoom)
            {
                InitializeTanks();
                SpawnAllTanks();
                StartCoroutine(FindAndAssignTanks());
                StartCoroutine(GameLoop());
            }
        }

        private void InitializeTanks()
        {
            int playerCount = PhotonNetwork.PlayerList.Length;
            m_Tanks = new List<TankManager>();

            for (int i = 0; i < playerCount; i++)
            {
                TankManager tankManager = new TankManager
                {
                    m_PlayerNumber = i + 1,
                    m_PlayerColor = GetPlayerColor(i + 1),
                    m_SpawnPoint = GetSpawnPoint(i + 1)
                };
                m_Tanks.Add(tankManager);
            }

            Debug.Log($"Initialized {playerCount} TankManagers.");
        }

        private Color GetPlayerColor(int playerNumber)
        {
            if (playerNumber == 1)
                return Color.blue;
            else if (playerNumber == 2)
                return Color.red;
            else
                return Color.white;
        }

        private Transform GetSpawnPoint(int playerNumber)
        {
            string spawnPointName = "SpawnPoint" + playerNumber;
            GameObject spawnPointObj = GameObject.Find(spawnPointName);
            if (spawnPointObj != null)
                return spawnPointObj.transform;
            else
            {
                Debug.LogError($"SpawnPoint {spawnPointName} not found.");
                return null;
            }
        }
        private void SpawnAllTanks()
        {
            if (!PhotonNetwork.IsConnected)
            {
                Debug.LogError("Not connected to Photon");
                return;
            }
            if (PhotonNetwork.IsMasterClient)
            {
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    int playerIndex = player.ActorNumber - 1;

                    if (playerIndex >= 0 && playerIndex < m_Tanks.Count)
                    {
                        Transform spawnTransform = m_Tanks[playerIndex].m_SpawnPoint;

                        if (spawnTransform == null)
                        {
                            Debug.LogError($"SpawnPoint for player {player.ActorNumber} is null.");
                            continue;
                        }

                        object[] initData = new object[] { player.ActorNumber };

                        GameObject tank = PhotonNetwork.Instantiate("CompleteTank", spawnTransform.position, spawnTransform.rotation, 0, initData);

                        if (tank != null)
                        {
                            m_Tanks[playerIndex].m_Instance = tank;
                            m_Tanks[playerIndex].Setup();
                            photonView.RPC(nameof(SyncTankSetup), RpcTarget.Others, player.ActorNumber, spawnTransform.position, spawnTransform.rotation);
                            Debug.Log($"Spawned Tank for Player {player.ActorNumber}");
                        }
                        else
                        {
                            Debug.LogError($"Failed to instantiate CompleteTank for player {player.ActorNumber}");
                        }
                    }
                    else
                    {
                        Debug.LogError($"PlayerIndex {playerIndex} is out of bounds. m_Tanks.Count = {m_Tanks.Count}");
                    }
                }
            }
        }
        [PunRPC]
        private void SyncTankSetup(int playerNumber, Vector3 position, Quaternion rotation)
        {
            int playerIndex = playerNumber - 1;

            if (playerIndex >= 0 && playerIndex < m_Tanks.Count)
            {
                if (m_Tanks[playerIndex].m_Instance == null)
                {
                    object[] initData = { playerNumber };
                    GameObject tank = PhotonNetwork.Instantiate("CompleteTank", position, rotation, 0, initData);

                    if (tank != null)
                    {
                        m_Tanks[playerIndex].m_Instance = tank;
                        m_Tanks[playerIndex].Setup();
                        Debug.Log($"�������ꂽ�v���C���[ {playerNumber} �̃^���N��ݒ肵�܂����B");
                    }
                }
            }
            else
            {
                Debug.LogError($"�����Ώۂ̃v���C���[�C���f�b�N�X {playerIndex} ���͈͊O�ł��B");
            }
        }
        private IEnumerator FindAndAssignTanks()
        {
            while (true)
            {
                GameObject[] tanks = GameObject.FindGameObjectsWithTag("Player");

                foreach (GameObject tank in tanks)
                {
                    PhotonView photonView = tank.GetComponent<PhotonView>();
                    if (photonView != null && photonView.InstantiationData != null && photonView.InstantiationData.Length > 0)
                    {
                        int playerNumber = (int)photonView.InstantiationData[0];
                        int playerIndex = playerNumber - 1;

                        if (playerIndex >= 0 && playerIndex < m_Tanks.Count && m_Tanks[playerIndex].m_Instance == null)
                        {
                            m_Tanks[playerIndex].m_Instance = tank;
                            m_Tanks[playerIndex].Setup();
                            Debug.Log($"Assigned Tank to Player {playerNumber}");
                        }
                    }
                }

                if (m_Tanks.TrueForAll(t => t.m_Instance != null))
                {
                    break;
                }

                yield return new WaitForSeconds(0.5f);
            }

            SetCameraTargets();
        }




        private void SetCameraTargets()
        {
            Transform[] targets = new Transform[m_Tanks.Count];
            for (int i = 0; i < m_Tanks.Count; i++)
            {
                if (m_Tanks[i].m_Instance != null)
                {
                    targets[i] = m_Tanks[i].m_Instance.transform;
                }
            }

            m_CameraControl.m_Targets = targets;
        }


        private void SetGameState(GameState newState)
        {
            if (CurrentGameState == newState)
                return;

            CurrentGameState = newState;

            if (GameStateChanged != null)
            {
                GameStateChanged.Invoke(newState);
            }

            if (PhotonNetwork.IsMasterClient)
            {
                if (photonView != null)
                {
                    photonView.RPC(nameof(SetGameStateRPC), RpcTarget.Others, newState);
                }
                else
                {
                    Debug.LogError("photonView is null in SetGameState.");
                }
            }
        }

        [PunRPC]
        private void SetGameStateRPC(GameState newState)
        {
            CurrentGameState = newState;
            GameStateChanged?.Invoke(newState);
        }

        // This is called from start and will run each phase of the game one after another.
        private IEnumerator GameLoop()
        {
            // Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
            yield return StartCoroutine(RoundStarting());

            // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
            yield return StartCoroutine(RoundPlaying());

            // Once execution has returned here, run the 'RoundEnding' coroutine, again don't return until it's finished.
            yield return StartCoroutine(RoundEnding());

            // This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found.
            if (m_GameWinner != null)
            {
                // If there is a game winner, restart the level.
                // SceneManager.LoadScene(SceneNames.TitleScene);
                PhotonNetwork.LoadLevel(SceneNames.TitleScene);
            }
            else
            {
                // If there isn't a winner yet, restart this coroutine so the loop continues.
                // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
                StartCoroutine(GameLoop());
            }
        }


        private IEnumerator RoundStarting()
        {
            SetGameState(GameState.RoundStarting);
            // As soon as the round starts reset the tanks and make sure they can't move.
            ResetAllTanks();
            DisableTankControl();

            // Snap the camera's zoom and position to something appropriate for the reset tanks.
            m_CameraControl.Move();

            // Increment the round number and display text showing the players what round it is.
            m_RoundNumber++;
            m_MessageText.text = "ROUND " + m_RoundNumber;

            // Wait for the specified length of time until yielding control back to the game loop.
            yield return m_StartWait;
        }


        private IEnumerator RoundPlaying()
        {
            SetGameState(GameState.RoundPlaying);
            // As soon as the round begins playing let the players control the tanks.
            EnableTankControl();

            // Clear the text from the screen.
            m_MessageText.text = string.Empty;

            // While there is not one tank left...
            while (!OneTankLeft())
            {
                // ... return on the next frame.
                yield return null;
            }
        }


        private IEnumerator RoundEnding()
        {
            SetGameState(GameState.RoundEnding);
            // Stop tanks from moving.
            DisableTankControl();

            // Clear the winner from the previous round.
            // m_RoundWinner = null;

            // See if there is a winner now the round is over.
            m_RoundWinner = GetRoundWinner();
            if (m_RoundWinner != null)
            {
                m_RoundWinner.m_Wins++;
                // Debug.Log($"{m_RoundWinner.m_ColoredPlayerText} wins this round! Total wins: {m_RoundWinner.m_Wins}");
            }

            // Now the winner's score has been incremented, see if someone has one the game.
            m_GameWinner = GetGameWinner();

            if (m_RoundWinner != null)
            {
                m_RoundWinner.m_Wins++;
            }

            m_GameWinner = GetGameWinner();

            if (CountWins.Instance != null)
            {
                CountWins.Instance.UpdateWinStars();
            }

            if (m_GameWinner != null)
            {
                m_MessageText.text = $"{m_GameWinner.m_ColoredPlayerText} WINS THE GAME!";
                yield return m_EndWait;
                PhotonNetwork.LoadLevel(SceneNames.TitleScene);
                yield break;
            }
            // Get a message based on the scores and whether or not there is a game winner and display it.
            string message = EndMessage();
            m_MessageText.text = message;

            // Wait for the specified length of time until yielding control back to the game loop.
            yield return m_EndWait;
        }


        // This is used to check if there is one or fewer tanks remaining and thus the round should end.
        private bool OneTankLeft()
        {
            // Start the count of tanks left at zero.
            int numTanksLeft = 0;

            // Go through all the tanks...
            /* for (int i = 0; i < m_Tanks.Length; i++)
            {
                // ... and if they are active, increment the counter.
                if (m_Tanks[i].m_Instance.activeSelf)
                    numTanksLeft++;
            } */

            foreach (var tank in m_Tanks)
            {
                if (tank.m_Instance != null && tank.m_Instance.activeSelf)
                {
                    numTanksLeft++;
                }
            }

            // If there are one or fewer tanks remaining return true, otherwise return false.
            return numTanksLeft <= 1;
        }

        // This function is to find out if there is a winner of the round.
        // This function is called with the assumption that 1 or fewer tanks are currently active.
        private TankManager GetRoundWinner()
        {
            // Go through all the tanks...
            /* for (int i = 0; i < m_Tanks.Length; i++)
            {
                // ... and if one of them is active, it is the winner so return it.
                if (m_Tanks[i].m_Instance.activeSelf)
                    return m_Tanks[i];
            } */

            foreach (var tank in m_Tanks)
            {
                if (tank.m_Instance != null && tank.m_Instance.activeSelf)
                {
                    return tank;
                }
            }

            // If none of the tanks are active it is a draw so return null.
            return null;
        }


        // This function is to find out if there is a winner of the game.
        private TankManager GetGameWinner()
        {
            // Go through all the tanks...
            /* for (int i = 0; i < m_Tanks.Length; i++)
            {
                // ... and if one of them has enough rounds to win the game, return it.
                if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                    return m_Tanks[i];
            } */

            foreach (var tank in m_Tanks)
            {
                if (tank.m_Wins == m_NumRoundsToWin)
                {
                    return tank;
                }
            }

            // If no tanks have enough rounds to win, return null.
            return null;
        }


        // Returns a string message to display at the end of each round.
        private string EndMessage()
        {
            // By default when a round ends there are no winners so the default end message is a draw.
            string message = "DRAW!";

            // If there is a winner then change the message to reflect that.
            if (m_RoundWinner != null)
                message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

            // Add some line breaks after the initial message.
            message += "\n\n\n\n";

            // Go through all the tanks and add each of their scores to the message.
            /* for (int i = 0; i < m_Tanks.Length; i++)
            {
                message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
            } */

            foreach (var tank in m_Tanks)
            {
                message += tank.m_ColoredPlayerText + ": " + tank.m_Wins + " WINS\n";
            }
            // If there is a game winner, change the entire message to reflect that.
            if (m_GameWinner != null)
                message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

            return message;
        }


        // This function is used to turn all the tanks back on and reset their positions and properties.
        private void ResetAllTanks()
        {
            /* for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].Reset();
            } */

            foreach (var tank in m_Tanks)
            {
                tank.Reset();
            }
        }


        private void EnableTankControl()
        {
            /* for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].EnableControl();
            } */

            foreach (var tank in m_Tanks)
            {
                tank.EnableControl();
            }
        }


        private void DisableTankControl()
        {
            /* for (int i = 0; i < m_Tanks.Length; i++)
            {
                m_Tanks[i].DisableControl();
            } */

            foreach (var tank in m_Tanks)
            {
                tank.DisableControl();
            }
        }
    }
}