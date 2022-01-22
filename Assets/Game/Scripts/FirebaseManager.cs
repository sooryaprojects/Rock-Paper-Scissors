using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using SimpleJSON;

namespace RCP
{
    public class FirebaseManager : MonoBehaviour
    {
        public static FirebaseManager instance;
        DatabaseReference baseDB;

        DatabaseReference newRoomDBRef
        {
            get
            {
                return baseDB.Child("newRoom");
            }
        }
        DatabaseReference currentMatchDBRef
        {
            get
            {
                return baseDB.Child("ActiveMatch").Child(ID.ToString());//.Child("Player1").;
            }
        }
        int currentPlayerIndex = 1;
        DatabaseReference currentPlayerDBRef
        {
            get
            {
                return baseDB.Child("ActiveMatch").Child(ID.ToString()).Child($"Player{currentPlayerIndex}");
            }
        }
        DatabaseReference activeMatchDBRef
        {
            get
            {
                return baseDB.Child("ActiveMatch");
            }
        }
        DatabaseReference overMatchDBRef
        {
            get
            {
                return baseDB.Child("OvereMatch");
            }
        }

        DatabaseReference selctionRefPlayer(int num)
        {
            return currentMatchDBRef;//.Child($"Player{num}").Child("selctedType");
        }
        DatabaseReference scoreRefPlayer(int num)
        {
            return currentMatchDBRef;//.Child($"Player{num}").Child("score");
        }
 
        //string 
        public int ID;
        public Match currentMatch;
        public Player currentPlayer;
        public Player opponentPlayer;

        private void Awake()
        {
            instance = this;
        }

        void Start()
        {
            baseDB = FirebaseDatabase.DefaultInstance.RootReference;
        }

        [SerializeField] List<Match> matches;

        [ContextMenu("Create Room")]
        public void CreateOrJoinRoom()
        {
            baseDB.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted) { Debug.Log("Caught Error  " + task.Exception); }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    var rootnode = JSON.Parse(snapshot.GetRawJsonValue());

                    matches = new List<Match>();
                    if (rootnode["ActiveMatch"].ToString() == "null")
                        for (int i = 0; i < rootnode["ActiveMatch"].Count; i++)
                            matches.Add(JsonUtility.FromJson<Match>(rootnode["ActiveMatch"][i].ToString()));

                    currentMatch.id = matches.Count > 0 ? matches[matches.Count - 1].id + 1 : 0;
                    ID = currentMatch.id;

                    if (rootnode["newRoom"].ToString() == "null")
                    {
                        Debug.Log("Room Created Successfully");
                        newRoomDBRef.SetRawJsonValueAsync(ToJSON(currentPlayer));
                        currentPlayerIndex = 1;
                        GameManager.instance.uIManager.SetWaitingArea(true);
                        baseDB.ChildRemoved += HandleChildRemoved;
                    }
                    else
                    {
                        Debug.Log("Join Room Successfully");
                        opponentPlayer = JsonUtility.FromJson<Player>(rootnode["newRoom"].ToString());
                        CreateMatch();
                    }
                }
            });
        }

        void GetCurrentMatchData()
        {
            currentMatchDBRef.GetValueAsync().ContinueWithOnMainThread(task =>
         {
             if (task.IsFaulted)
             {
                 // Handle the error...
                 Debug.Log("Caught Error  " + task.Exception);
             }
             else if (task.IsCompleted)
             {

                 DataSnapshot snapshot = task.Result;
                 Debug.Log(snapshot.GetRawJsonValue());
                 currentMatch = JsonUtility.FromJson<Match>(snapshot.GetRawJsonValue());
                 ImageLoader.Instance.SetBG();

                 opponentPlayer = currentMatch.Player1;
                 currentMatchDBRef.ChildChanged += HandleChildChanged;
                 GameManager.instance.uIManager.SetWaitingArea(false);

             }
         });
        }

        public void CreateMatch()
        {
            currentMatch.Player1 = currentPlayer;
            currentMatch.Player2 = opponentPlayer;
            currentMatch.bgNumber = Random.Range(1, 8);
            activeMatchDBRef.Child(ID.ToString()).SetRawJsonValueAsync(ToJSON(currentMatch));
            ImageLoader.Instance.SetBG();
            //Debug.Log("Removing new Room");
            newRoomDBRef.RemoveValueAsync();
            currentPlayerIndex = 2;
            GameManager.instance.uIManager.SetWaitingArea(false);
            currentMatchDBRef.ChildChanged += HandleChildChanged;
        }

        public void UpdateCurrentPlayerData()
        {
            currentPlayerDBRef.SetRawJsonValueAsync(ToJSON(currentPlayer));
        }

        public void SelectWeapon(RPC rPC)
        {
            if (currentPlayer.selctedType != 0 || GameManager.instance.isOnWaitingState)
                return;
            currentPlayer.selctedType = rPC;
            UpdateCurrentPlayerData();
            GameManager.instance.uIManager.UpdateUIOfPlayer(currentPlayer);
            if (opponentPlayer.selctedType > 0)
            {
                GameManager.instance.uIManager.UpdateUIOfPlayer(opponentPlayer, false);
                print("From Weapon Selection Declare Result");
                GameManager.instance.DeclareResult(true);
            }
        }

        public void TurnOver()
        {
            Debug.Log("Turn Over");
            currentMatch.TurnCount++;
            currentMatchDBRef.Child("TurnCount").SetValueAsync(currentMatch.TurnCount);
        }



        [ContextMenu("Leave Room")]
        public void LeaveRoom()
        {
            overMatchDBRef.Child(ID.ToString()).SetRawJsonValueAsync(ToJSON(currentMatch));
            currentMatchDBRef.RemoveValueAsync();

        }

        private void OnDisable()
        {
            currentMatchDBRef.ChildChanged -= HandleChildChanged;
        }
        void HandleChildChanged(object sender, ChildChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }

            // Do something with the data in args.Snapshot

            //Debug.Log(args.Snapshot.Key);
            //Debug.Log(args.Snapshot.GetRawJsonValue());
            if (args.Snapshot.Key == "TurnCount")
            {
                //Debug.Log(args.Snapshot.GetValue(true));
                currentMatch.TurnCount = int.Parse((args.Snapshot.GetRawJsonValue()));
                if (currentMatch.TurnCount <= 5)
                {
                    Debug.Log(" turn Value " + args.Snapshot.Value);
                    currentPlayer.selctedType = 0;
                    opponentPlayer.selctedType = 0;
                    //currentPlayerDBRef.Child("selctedType").SetValueAsync(0);
                    GameManager.instance.uIManager.UpdateUIOfPlayer(currentPlayer, true);
                    GameManager.instance.uIManager.UpdateUIOfPlayer(opponentPlayer, false);
                    UpdateCurrentPlayerData();
                    GameManager.instance.uIManager.resultStatusText.text = "";
                    GameManager.instance.isOnWaitingState = false;
                }
                else
                {
                    GameManager.instance.uIManager.ResultScreen();
                }
            }
            else if (args.Snapshot.Key != $"Player{currentPlayerIndex}" && !GameManager.instance.isOnWaitingState)
            {
                opponentPlayer = JsonUtility.FromJson<Player>(args.Snapshot.GetRawJsonValue().ToString());

                if (currentPlayer.selctedType > 0)
                {
                    GameManager.instance.uIManager.UpdateUIOfPlayer(opponentPlayer, false);
                    print("From Changed to Declare result current Player " + currentPlayerIndex);

                    GameManager.instance.DeclareResult(false);
                }
            }
        }

        void HandleChildRemoved(object sender, ChildChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.Log("Get Error");
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            if (args.Snapshot.Key == "newRoom")
            {
                Debug.Log("Removed new Room");
                baseDB.ChildRemoved -= HandleChildRemoved;
                GetCurrentMatchData();

            }
        }

        string ToJSON(object obj)
        {
            return JsonUtility.ToJson(obj);
        }

    }


    [System.Serializable]
    public class Player
    {
        public string name;
        public int score;
        public RPC selctedType;
    }

    [System.Serializable]
    public enum RPC
    {
        None,
        Rock,
        Scissor,
        Paper
    }
    public enum Result
    {
        None,
        Draw,
        Win,
        Looser
    }
    [System.Serializable]
    public class Match
    {
        public int id;
        public int bgNumber;
        public string MatchStatus;
        public Player Player1;
        public Player Player2;
        public float Time;
        public int TurnCount;
        public string Winner;
    }

}