using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchResult : MonoBehaviour
{
    public static MatchResult instance;
    [SerializeField] Animator anim;
    [SerializeField] GameObject MatchResultobject;
    [SerializeField] TextMeshProUGUI T_Content;
    [SerializeField] List<ResultMessage> Templates;
    [SerializeField] List<ResultMessage> Queue;

    [System.Serializable]
    public struct ResultMessage
    {
        [TextArea(1,5)]
        public string content;
        public Color color;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public ResultMessage GetResult(int ID, string emoteName)
    {
        if (Templates.Count >= ID - 1)
        {
            ResultMessage msg = Templates[ID];
            msg.content = msg.content.Replace("{name}", emoteName);
            return msg;
        }
        return new ResultMessage();
    }

    public void Request(int ID, string emoteName)
    {
        Queue.Add(GetResult(ID, emoteName));
    }
    public void Request(string content)
    {
        ResultMessage message = new ResultMessage()
        {
            content = content
        };
        Queue.Add(message);
    }
    public void Request(ResultMessage message)
    {
        Queue.Add(message);
    }

    private void Update()
    {
        if (Queue.Count > 0)
        {
            Open(Queue[0]);
        }
    }

    void Open(ResultMessage msg)
    {
        Queue.Remove(Queue[0]);
        T_Content.text = msg.content;
        T_Content.color = msg.color;
        MatchResultobject.SetActive(true);
        StopAllCoroutines();
        //anim.StopPlayback();
        anim.Play("mr_show");
        StartCoroutine(DisableObject());

    }
    IEnumerator DisableObject()
    {
        yield return new WaitForSeconds(3);
        MatchResultobject.SetActive(false);
    }

    [ContextMenu("Debug0")]
    public void Debug0()
    {
        Request(0, "peepoHappy");
    }
    [ContextMenu("Debug1")]
    public void Debug1()
    {
        Request(1, "peepoHappy");
    }
    [ContextMenu("Debug2")]
    public void Debug2()
    {
        Request(2, "peepoHappy");
    }
}
