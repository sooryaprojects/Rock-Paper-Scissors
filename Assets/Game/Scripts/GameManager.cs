using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using RCP;
using UnityEngine;

namespace RCP
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public UIManager uIManager;

        string currentPlayerName;
        public string CurrentPlayerName
        {
            get
            {
                return currentPlayerName;
            }
            set
            {
                currentPlayerName = value;
                FirebaseManager.instance.currentPlayer.name = currentPlayerName;
            }
        }

        private void Awake()
        {
            instance = this;
        }



        public void DeclareResult(bool isFromSelection)
        {
            print("DeclareResult isFromSelection " + isFromSelection);
            Result result = isWin(FirebaseManager.instance.currentPlayer.selctedType, FirebaseManager.instance.opponentPlayer.selctedType);
            //uIManager.resultStatusText.text = result.ToString();
            if (result.Equals(Result.Win))
            {
                FirebaseManager.instance.currentPlayer.score++;
                uIManager.UpdateUIOfPlayer(FirebaseManager.instance.currentPlayer);
                if (!isFromSelection)
                    FirebaseManager.instance.currentPlayer.selctedType = 0;

                FirebaseManager.instance.UpdateCurrentPlayerData();
            }
            if (isFromSelection)
            {
                FirebaseManager.instance.Invoke("TurnOver", 5);
            }
            isOnWaitingState = true;
        }
        public bool isOnWaitingState;
        // Game Play

        public Result isWin(RPC currentPlayer, RPC opponentSelection)
        {

            if (currentPlayer == opponentSelection && currentPlayer != RPC.None)
            {
                uIManager.resultStatusText.text = " Draw ";
                return Result.Draw;
            }
            else if ((currentPlayer == RPC.Rock && opponentSelection == RPC.Paper) ||
                (currentPlayer == RPC.Paper && opponentSelection == RPC.Scissor) ||
                (currentPlayer == RPC.Scissor && opponentSelection == RPC.Rock))
            {
                uIManager.resultStatusText.text = " Looser ";
                return Result.Looser;
            }

            else if ((currentPlayer == RPC.Paper && opponentSelection == RPC.Rock) ||
                (currentPlayer == RPC.Scissor && opponentSelection == RPC.Paper) ||
                (currentPlayer == RPC.Rock && opponentSelection == RPC.Scissor))
            {
                uIManager.resultStatusText.text = " Win ";
                return Result.Win;
            }
            else
            {
                uIManager.resultStatusText.text = "";
                return Result.None;
            }
        }



    }
}