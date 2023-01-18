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

        [SerializeField] TextMeshProUGUI T_scoreboard;
        [SerializeField] GameObject TwitchCanvas;

        GameManager gameManager;
        IRC ttv = IRC.Instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this);

            //if (bool.Parse(PlayerPrefs.GetString("settings_twitch_enabled")))
            if(true)
            {
                ttv.Connect();
                ttv.OnChatMessage += OnChatMessage;
                TwitchCanvas.SetActive(true);
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
            }

        }

        void UpdateUI()
        {
            //sort scoreboard
            List<KeyValuePair<string, int>> Scoreboard = Scores.OrderBy(d => d.Value).ToList();
            //Scoreboard.Sort((x, y) => x.Value.CompareTo(y.Value));
            string scoreboardtext = $"1. {Scoreboard[0].Key} {Scoreboard[0].Value}";
            for (int i = 1; i < Scoreboard.Count; i++)
                scoreboardtext += $"\n{i + 1}. {Scoreboard[0].Key} {Scoreboard[0].Value}";

            T_scoreboard.text = scoreboardtext;

        }

        void OnDestroy()
        {
            ttv.OnChatMessage -= OnChatMessage;
            GameManager.OnReroll -= OnEmoteReroll;
        }
    }
}
