using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mirror
{
    public class PirateCtfGameManager : MonoBehaviour
    {
        public int MinimumPlayersForGame = 1;

        public Movement LocalPlayer;
        public GameObject StartPanel;
        public GameObject GameOverPanel;
        public GameObject HealthTextLabel;
        public GameObject ScoreTextLabel;
        public Text HealthText;
        public Text ScoreText;
        public Text PlayerNameText;
        public Text WinnerNameText;
        public bool IsGameReady;
        public bool IsGameOver;
        public List<Movement> players = new List<Movement>();

        void Update()
        {
            if (NetworkManager.singleton.isNetworkActive)
            {
                GameReadyCheck();
                GameOverCheck();

                if (LocalPlayer == null)
                {
                    FindLocalMovement();
                }
                else
                {
                    ShowReadyMenu();
                    UpdateStats();
                }
            }
            else
            {
                //Cleanup state once network goes offline
                IsGameReady = false;
                LocalPlayer = null;
                players.Clear();
            }
        }

        void ShowReadyMenu()
        {
            if (NetworkManager.singleton.mode == NetworkManagerMode.ServerOnly)
                return;

            if (LocalPlayer.isReady)
                return;

            StartPanel.SetActive(true);
        }

        void GameReadyCheck()
        {
            if (!IsGameReady)
            {
                //Look for connections that are not in the player list
                foreach (KeyValuePair<uint, NetworkIdentity> kvp in NetworkIdentity.spawned)
                {
                    Movement comp = kvp.Value.GetComponent<Movement>();

                    //Add if new
                    if (comp != null && !players.Contains(comp))
                    {
                        players.Add(comp);
                    }
                }

                //If minimum connections has been check if they are all ready
                if (players.Count >= MinimumPlayersForGame)
                {
                    bool AllReady = true;
                    foreach (Movement Movement in players)
                    {
                        if (!Movement.isReady)
                        {
                            AllReady = false;
                        }
                    }
                    if (AllReady)
                    {
                        IsGameReady = true;
                        AllowMovementMovement();

                        //Update Local GUI:
                        StartPanel.SetActive(false);
                        HealthTextLabel.SetActive(true);
                        ScoreTextLabel.SetActive(true);
                    }
                }
            }
        }

        void GameOverCheck()
        {
            if (!IsGameReady)
                return;

            //Cant win a game you play by yourself. But you can still use this example for testing network/movement
            if (players.Count == 1)
                return;

            int alivePlayerCount = 0;
            foreach (Movement Movement in players)
            {
                if (!Movement.isDead)
                {
                    alivePlayerCount++;

                    //If there is only 1 player left alive this will end up being their name
                    WinnerNameText.text = Movement.playerName;
                }
            }

            if (alivePlayerCount == 1)
            {
                IsGameOver = true;
                GameOverPanel.SetActive(true);
                DisallowMovementMovement();
            }
        }

        void FindLocalMovement()
        {
            //Check to see if the player is loaded in yet
            if (ClientScene.localPlayer == null)
                return;

            LocalPlayer = ClientScene.localPlayer.GetComponent<Movement>();
        }

        void UpdateStats()
        {
            HealthText.text = LocalPlayer.health.ToString();
            ScoreText.text = LocalPlayer.score.ToString();
        }

        public void ReadyButtonHandler()
        {
            LocalPlayer.SendReadyToServer(PlayerNameText.text);
        }

        //All players are ready and game has started. Allow players to move.
        void AllowMovementMovement()
        {
            foreach (Movement Movement in players)
            {
                Movement.allowMovement = true;
            }
        }

        //Game is over. Prevent movement
        void DisallowMovementMovement()
        {
            foreach (Movement Movement in players)
            {
                Movement.allowMovement = false;
            }
        }
    }
}
