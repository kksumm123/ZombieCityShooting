using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    public LoadingUI loadingUI;

    void Update()
    {
        if (Input.anyKeyDown)
        {
            // 비동기 로드
            var progress = SceneManager.LoadSceneAsync("Main");
        }
    }
}
