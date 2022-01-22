using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using RCP;
//using RPC = RCP.RPC;
namespace RCP
{
    public class UIManager : MonoBehaviour
    {
        public Sprite[] rcpSprite;

        [SerializeField] Text wallteCoin;
        [SerializeField] InputField nameInputField;
        [SerializeField] GameObject waitingForOpponent, gamePlayArea;

        [SerializeField] Text resultDeclareTxt;

        [SerializeField] Text currentPlayerScore;
        [SerializeField] Text currentPlayerName;
        [SerializeField] Image currentPlayerSelection;

        [SerializeField] Text opponentName;
        [SerializeField] Text opponentScore;
        [SerializeField] Image opponentSelection;

        public Text resultStatusText;

        [Header("_____ Screen Canvas _____")]
        public Canvas shopScreen;
        public Canvas homecreen;
        public Canvas gamePlayScreen;
        public Canvas resultScreen;



        private void Start()
        {
            wallteCoin.text = $"Wallet : {PlayerPrefs.GetInt("coin", 1000)} coins";
            HomeScreen();
        }

        public void Play()
        {
            GameManager.instance.CurrentPlayerName = nameInputField.text;
            FirebaseManager.instance.CreateOrJoinRoom();
            currentPlayerName.text = GameManager.instance.CurrentPlayerName;
            currentPlayerScore.text = "Score : 0";
            //nameInputField.text = "";

        }


        public void BuyCoin(int coins)
        {
            int coin = PlayerPrefs.GetInt("coin", 1000);
            coin += coins;
            wallteCoin.text = $"Wallet : {coin} coins";
            PlayerPrefs.SetInt("coin", coin);
        }

        public void UpdateUIOfPlayer(Player player, bool islocal = true)
        {
            if (islocal)
            {
                currentPlayerScore.text = "Score : " + player.score;
                if (player.selctedType > 0)
                    currentPlayerSelection.sprite = rcpSprite[(int)player.selctedType - 1];
                currentPlayerSelection.enabled = player.selctedType > 0;
            }
            else
            {
                opponentScore.text = "Score : " + player.score;
                if (player.selctedType > 0)
                    opponentSelection.sprite = rcpSprite[(int)player.selctedType - 1];
                opponentSelection.enabled = player.selctedType > 0;
            }
        }

        public void SelectWeapon(int rpc)
        {
            FirebaseManager.instance.SelectWeapon((RPC)rpc);
        }



        public void ShopScreen()
        {
            EnableScreen(shopScreen);
        }
        public void GamePlayScreen()
        {
            EnableScreen(gamePlayScreen);
        }
        public void ResultScreen()
        {
            if (FirebaseManager.instance.currentPlayer.score > FirebaseManager.instance.opponentPlayer.score)
            {
                resultDeclareTxt.text = "You Win";
            }
            else if (FirebaseManager.instance.currentPlayer.score == FirebaseManager.instance.opponentPlayer.score)
            {
                resultDeclareTxt.text = "DRAW";
            }
            else
            {
                resultDeclareTxt.text = $"{FirebaseManager.instance.opponentPlayer.name} is Win";
            }
            EnableScreen(resultScreen);
        }
        public void HomeScreen()
        {
            EnableScreen(homecreen);
        }

        public void Restart()
        {
            currentPlayerName.text = opponentName.text = "";
            currentPlayerScore.text = opponentScore.text = "Score : 0";
            SceneManager.LoadScene("Game");
        }

        public void SetWaitingArea(bool isCreateRoom)
        {
            waitingForOpponent.SetActive(isCreateRoom);
            gamePlayArea.SetActive(!isCreateRoom);
            GamePlayScreen();
            opponentName.text = FirebaseManager.instance.opponentPlayer.name;
            opponentScore.text = "Score : 0";
        }

        void EnableScreen(Canvas onCanvas)
        {
            shopScreen.enabled = shopScreen.Equals(onCanvas);
            homecreen.enabled = homecreen.Equals(onCanvas);
            gamePlayScreen.enabled = gamePlayScreen.Equals(onCanvas);
            resultScreen.enabled = resultScreen.Equals(onCanvas);
        }

        void UpdateScore(int score, bool isLocalPlayer)
        {
            if (isLocalPlayer)
                currentPlayerScore.text = $"Score : {score}";
            else
                opponentScore.text = $"Score : {score}";
        }
    }
}