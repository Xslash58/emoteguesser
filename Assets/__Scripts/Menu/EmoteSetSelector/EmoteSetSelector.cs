using System.Collections;
using System.Collections.Generic;
using SevenTV.Types;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

namespace EmoteGuesser.Menu.SetSelector
{
    public class EmoteSetSelector : MonoBehaviour
    {
        public static EmoteSetSelector instance = null;
        [SerializeField] GameObject SetSelector;

        [SerializeField] List<SevenTV.Types.EmoteSet> EmoteSets = new List<SevenTV.Types.EmoteSet>();
        [SerializeField] ScrollRect SR_EmoteSets;
        [SerializeField] GameObject EmoteSetObject;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this);
        }

        public void Show(SevenTV.Types.EmoteSet[] emoteSets)
        {
            EmoteSets = emoteSets.ToList();
            Reload();
            SetSelector.SetActive(true);
        }
        public void Hide()
        {
            SetSelector.SetActive(false);
        }

        void Reload()
        {
            foreach (GameObject obj in SR_EmoteSets.content)
                Destroy(obj);

            foreach(SevenTV.Types.EmoteSet set in EmoteSets)
            {
                GameObject obj = Instantiate(EmoteSetObject, SR_EmoteSets.content);
                EmoteSet eset = obj.GetComponent<EmoteSet>();
                obj.name = set.name;
                eset.setName = set.name;
                eset.capacity = set.capacity;
                eset.emoteCount = set.emotes.Count();
            }
        }
    }
}
