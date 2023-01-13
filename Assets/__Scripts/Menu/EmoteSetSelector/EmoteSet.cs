using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EmoteGuesser.Menu.SetSelector
{
    public class EmoteSet : MonoBehaviour
    {
        public string setName;
        public int capacity;
        public int emoteCount;

        [SerializeField] TextMeshProUGUI T_setName;
        [SerializeField] TextMeshProUGUI T_slots;

        void Start()
        {
            T_setName.text = setName;
            T_slots.text = $"{emoteCount}/{capacity}";
        }
    }
}
