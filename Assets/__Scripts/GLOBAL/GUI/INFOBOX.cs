using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class INFOBOX : MonoBehaviour
{
    public static INFOBOX instance;
    [SerializeField] Animator anim;
    [SerializeField] GameObject INFOBOXobject;
    [SerializeField] TextMeshProUGUI T_Title;
    [SerializeField] TextMeshProUGUI T_Description;
    [SerializeField] List<INFOBOXMessage> Queue;

    bool IgnoreRequests = false;

    public delegate void CloseHandler();
    public event CloseHandler CloseEvent;

    [System.Serializable]
    public struct INFOBOXMessage
    {
        public string title;
        public string description;
    }


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void Request(string Title, string Description)
    {
        if (IgnoreRequests)
            return;

        Debug.Log("Got request!");
        INFOBOXMessage message = new INFOBOXMessage()
        {
            title = Title,
            description = Description
        };
        Queue.Add(message);
    }
    public void Request(INFOBOXMessage message)
    {
        if (IgnoreRequests)
            return;

        Debug.Log("Got request!");
        Queue.Add(message);
    }

    private void Update()
    {
        if (Queue.Count > 0 && !INFOBOXobject.activeInHierarchy && TranslationManager.ready)
        {
            Open(Queue[0]);
        }
    }

    void Open(INFOBOXMessage msg)
    {
        Queue.Remove(Queue[0]);

        T_Title.text = msg.title;
        T_Description.text = msg.description;

        INFOBOXobject.SetActive(true);

        anim.Play("ib_show");

    }
    public void Close()
    {
        CloseEvent?.Invoke();
        anim.Play("ib_hide");
        StartCoroutine(DisableObject());
    }
    IEnumerator DisableObject()
    {
        yield return new WaitForSeconds(1);
        INFOBOXobject.SetActive(false);
    }


    public void Disable()
    {
        IgnoreRequests = true;
    }
}