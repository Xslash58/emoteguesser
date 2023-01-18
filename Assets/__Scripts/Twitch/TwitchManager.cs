using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using Lexone.UnityTwitchChat;
using TMPro;
using System.Linq;

namespace EmoteGuesser.Twitch
{
    public class TwitchManager : MonoBehaviour
    {
        public static TwitchManager instance = null;
        Dictionary<string, int> Scores = new Dictionary<string, int>();
        List<string> GuessedRound = new List<string>();

        GameModes gameMode = GameModes.CLASSIC;

        [SerializeField] TextMeshProUGUI T_scoreboard;
        [SerializeField] GameObject TwitchCanvas;
        [SerializeField] TMP_InputField IF_emote;

        GameManager gameManager;
        IRC ttv = IRC.Instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this);

            if (PlayerPrefs.HasKey("settings_twitch_enabled") && bool.Parse(PlayerPrefs.GetString("settings_twitch_enabled")))
            {
                ttv.Connect();
                ttv.OnChatMessage += OnChatMessage;
                TwitchCanvas.SetActive(true);

                if (PlayerPrefs.HasKey("settings_twitch_gamemode"))
                {
                    GameModes gm = (GameModes) PlayerPrefs.GetInt("settings_twitch_gamemode");
                    gameMode = gm;
                }
            }
        }

        private void Start()
        {
            gameManager = GameManager.instance;
            GameManager.OnReroll += OnEmoteReroll;
        }

        private void OnEmoteReroll()
        {
            GuessedRound.Clear();
        }

        private void OnChatMessage(Chatter chatter)
        {
            if (chatter.message.Contains(gameManager.Emote.name) && !GuessedRound.Contains(chatter.login))
            {
                Scores.TryGetValue(chatter.login, out int score);
                if (score == 0)
                    Scores.Add(chatter.login, 1);
                else
                    Scores[chatter.login] += 1;

                GuessedRound.Add(chatter.login);
                UpdateUI();

                if (gameMode == GameModes.TWITCHPLAYS)
                {
                    IF_emote.text = gameManager.Emote.name;
                    gameManager.Guess();
                }
            }

        }

        void UpdateUI()
        {
            List<KeyValuePair<string, int>> Scoreboard = Scores.OrderBy(d => d.Value).ToList();
            Scoreboard.Reverse();

            string scoreboardtext = $"1. {Scoreboard[0].Key} {Scoreboard[0].Value}";
            for (int i = 1; i < Scoreboard.Count; i++)
                scoreboardtext += $"\n{i + 1}. {Scoreboard[i].Key} {Scoreboard[i].Value}";

            T_scoreboard.text = scoreboardtext;

        }

        void OnDestroy()
        {
            ttv.OnChatMessage -= OnChatMessage;
            GameManager.OnReroll -= OnEmoteReroll;
        }
    }

    public enum GameModes { CLASSIC, TWITCHPLAYS }
}
