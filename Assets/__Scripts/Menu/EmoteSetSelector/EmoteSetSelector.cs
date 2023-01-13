using System.Collections;
using System.Collections.Generic;
using EmoteGuesser.Menu.SetSelector;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class EmoteSetSelector : MonoBehaviour
{
    public static EmoteSetSelector instance = null;

    public SevenTV.Types.EmoteSet SelectedEmoteSet;

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

    public void Show(List<SevenTV.Types.EmoteSet> emoteSets)
    {
        EmoteSets = emoteSets;

        //Selector is useless on 1 emote set
        if (EmoteSets.Count == 1)
        {
            SelectedEmoteSet = EmoteSets[0];
            Continue();
        }

        Reload();
        SetSelector.SetActive(true);
    }
    public void Hide()
    {
        SetSelector.SetActive(false);
    }
    public void Continue()
    {
        if (SelectedEmoteSet == null)
            return;

        Menu.instance.Play(SelectedEmoteSet.id);
    }

    public void UnselectAll()
    {
        foreach (Transform obj in SR_EmoteSets.content)
            obj.GetComponent<EmoteSet>().Unselect();
    }


    void Reload()
    {
        foreach (Transform obj in SR_EmoteSets.content)
            Destroy(obj.gameObject);

        foreach (SevenTV.Types.EmoteSet set in EmoteSets)
        {
            GameObject obj = Instantiate(EmoteSetObject, SR_EmoteSets.content);
            EmoteSet eset = obj.GetComponent<EmoteSet>();
            obj.name = set.name;
            eset.Set = set;
        }
    }
}
