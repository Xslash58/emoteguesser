using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EmoteGuesser.Types
{
    public enum Provider { SEVENTV, BETTERTTV, FRANKERFACEZ }
    public class Emote
    {
        public string name;
        public string emoteUrl;
        public bool animated;
        public Provider provider;
    }
}