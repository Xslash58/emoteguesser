using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Utilities;
using SevenTV.Types;
using FFZ.Types;
using BTTV.Types;

public class AotTypeEnforcer : MonoBehaviour
{
    private void Awake()
    {
        //SevenTV-lib
        AotHelper.EnsureType<Connection>();
        AotHelper.EnsureType<SevenTV.Types.User>();
        AotHelper.EnsureType<Style>();
        AotHelper.EnsureType<Editor>();
        AotHelper.EnsureType<EmoteSet>();
        AotHelper.EnsureType<SevenTV.Types.Emote>();
        AotHelper.EnsureType<EmoteData>();
        AotHelper.EnsureType<EmoteHost>();
        AotHelper.EnsureType<EmoteFile>();
        AotHelper.EnsureType<TwitchUser>();
        AotHelper.EnsureType<TwitchRoles>();
        AotHelper.EnsureType<TwitchBadge>();
        AotHelper.EnsureType<TwitchPanel>();

        //FFZ
        AotHelper.EnsureType<FFZ.Types.Emote>();
        AotHelper.EnsureType<Room>();
        AotHelper.EnsureType<Set>();

        //BTTV
        AotHelper.EnsureType<BTTV.Types.User>();
        AotHelper.EnsureType<SharedEmote>();
        AotHelper.EnsureType<ChannelEmote>();
    }
}
