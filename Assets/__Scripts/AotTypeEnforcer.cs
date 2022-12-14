using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Utilities;
using SevenTV.Types;

public class AotTypeEnforcer : MonoBehaviour
{
    private void Awake()
    {
        AotHelper.EnsureType<Connection>();
        AotHelper.EnsureType<User>();
        AotHelper.EnsureType<Style>();
        AotHelper.EnsureType<Editor>();
        AotHelper.EnsureType<EmoteSet>();
        AotHelper.EnsureType<Emote>();
        AotHelper.EnsureType<EmoteData>();
        AotHelper.EnsureType<EmoteHost>();
        AotHelper.EnsureType<EmoteFile>();
        AotHelper.EnsureType<TwitchUser>();
        AotHelper.EnsureType<TwitchRoles>();
        AotHelper.EnsureType<TwitchBadge>();
    }
}
