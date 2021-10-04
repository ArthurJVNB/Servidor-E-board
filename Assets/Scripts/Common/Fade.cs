using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public Image img;
    bool fadein = false, fadeout = true, ld = false, destroy_obj = false;
    string scn = "";
    GameObject go;

    public IEnumerator FadeImage(bool fadeAway, bool loadScene, string scene)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                img.color = new Color(img.color.r, img.color.g, img.color.b, i);
                yield return null;
            }
            img.enabled = false;

            if (loadScene)
                SceneManager.LoadScene(scene);
        }
        // fade from transparent to opaque
        else
        {
            img.enabled = true;
            // loop over 1 second
            for (float i = 0; i <= 1; i += Time.deltaTime)
            {
                // set color with i as alpha
                img.color = new Color(img.color.r, img.color.g, img.color.b, i);
                yield return null;
            }

            if (loadScene)
                SceneManager.LoadScene(scene);
            else if (destroy_obj)
                Destroy(go);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (fadein)
        {
            StartCoroutine(FadeImage(false, ld, scn));
            ld = false;
            scn = "";
            fadein = false;
        }
        else if (fadeout)
        {
            StartCoroutine(FadeImage(true, ld, scn));
            ld = false;
            scn = "";
            fadeout = false;
        }
    }

    public void FadeIn(bool loadScene, string scene)
    {
        fadein = true;
        ld = loadScene;
        scn = scene;
    }

    public void FadeOut(bool loadScene, string scene)
    {
        fadeout = true;
        ld = loadScene;
        scn = scene;
    }

    public void FadeIn(bool destroy, GameObject obj)
    {
        fadein = true;
        destroy_obj = destroy;
        go = obj;
    }
}
