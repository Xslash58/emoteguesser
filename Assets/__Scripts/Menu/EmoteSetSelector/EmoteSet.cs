using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EmoteGuesser.Utilities.Image.WebP;

namespace EmoteGuesser.Menu.SetSelector
{
    public class EmoteSet : MonoBehaviour
    {
        public SevenTV.Types.EmoteSet Set;

        [SerializeField] ScrollRect SV_emotes;
        [SerializeField] GameObject emoteobject;

        [SerializeField] TextMeshProUGUI T_setName;
        [SerializeField] TextMeshProUGUI T_slots;
        [SerializeField] GameObject B_active;
        [SerializeField] GameObject B_selected;

        async void Start()
        {
            T_setName.text = Set.name;
            T_slots.text = $"{Set.emotes.Length}/{Set.capacity}";
            foreach (string tag in Set.tags)
                if (tag == "eg_active")
                    B_active.SetActive(true);

            for (int i = 0; i < 10; i++)
            {
                if (i >= Set.emotes.Length)
                    return;

                GameObject obj = Instantiate(emoteobject, SV_emotes.content);
                
                string hosturl = Set.emotes[i].data.host.url;
                string finalurl = $"https:{hosturl}/4x.webp";
                var bytes = await GetBytes(finalurl);
                obj.GetComponent<RawImage>().texture = LoadAnimation(bytes).Item1[0].texture;

            }
        }

        public void onClick()
        {
            EmoteSetSelector.instance.SelectedEmoteSet = Set;
            EmoteSetSelector.instance.UnselectAll();
            B_selected.SetActive(true);
        }
        public void Unselect()
        {
            B_selected.SetActive(false);
        }
    }
}
