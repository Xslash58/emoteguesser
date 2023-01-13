using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EmoteGuesser.Menu.SetSelector
{
    public class EmoteSet : MonoBehaviour
    {
        public SevenTV.Types.EmoteSet Set;

        [SerializeField] TextMeshProUGUI T_setName;
        [SerializeField] TextMeshProUGUI T_slots;
        [SerializeField] GameObject B_active;

        void Start()
        {
            T_setName.text = Set.name;
            T_slots.text = $"{Set.emotes.Length}/{Set.capacity}";
            foreach (string tag in Set.tags)
                if (tag == "eg_active")
                    B_active.SetActive(true);
        }

        public void onClick()
        {
            EmoteSetSelector.instance.SelectedEmoteSet = Set;
        }
    }
}
