using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class Loading : MonoBehaviour
{
    public int sceneID = -1;
    [SerializeField]
    Slider loadslider;

    private void Start()
    {
        StartCoroutine(Loadscene(sceneID));
    }
    public IEnumerator Loadscene(int id)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(id);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            float status = asyncOperation.progress * 100;
            loadslider.value = status;

            if (asyncOperation.progress >= 0.9f)
                StartCoroutine(CheckForRemoteCfg(asyncOperation));

            yield return null;
        }
    }

    public IEnumerator CheckForRemoteCfg(AsyncOperation asyncOperation)
    {
        if (RemoteConfig.instance.Ready)
            asyncOperation.allowSceneActivation = true;
        else
        {
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(CheckForRemoteCfg(asyncOperation));
        }
        yield return null;
    }
}
