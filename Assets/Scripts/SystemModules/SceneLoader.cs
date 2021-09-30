using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : PersistentSingleton<SceneLoader>
{
    /// <summary>
    /// 转场图片
    /// </summary>
    [Header("转场图片")]
    [SerializeField] Image transitionImage;

    /// <summary>
    /// 转场时间
    /// </summary>
    [Header("转场时间")]
    [SerializeField] float fadeTime = 3.5f;

    Color color;

    const string GAMEPLAY = "GamePlay";

    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="sceneName"></param>
    void Load(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadGameplayScene()
    {
        StartCoroutine(LoadCoroutine(GAMEPLAY));
    }

    IEnumerator LoadCoroutine(string sceneName)
    {
        var loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        loadingOperation.allowSceneActivation = false;


        transitionImage.gameObject.SetActive(true);//开始时将转场图片启用

        //图片透明度增加
        while (color.a<1f)
        {
            color.a=Mathf.Clamp01(color.a + Time.unscaledDeltaTime / fadeTime);
            transitionImage.color = color;

            yield return null;
        }

        loadingOperation.allowSceneActivation = true;

        while (color.a > 0f)
        {
            color.a = Mathf.Clamp01(color.a - Time.unscaledDeltaTime / fadeTime);
            transitionImage.color = color;

            yield return null;
        }
        transitionImage.gameObject.SetActive(false);
    }


    public void LoadEN()
    {
        StartCoroutine(LoadCoroutine("MainMenu"));
    }

}
