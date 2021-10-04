using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiscaImg : MonoBehaviour
{
    UnityEngine.UI.Image img;

    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<UnityEngine.UI.Image>();
    }

    // Update is called once per frame
    void Update()
    {
        img.color = new Color(img.color.r, img.color.g, img.color.b, Mathf.PingPong(Time.time / 1.5f, 1) * 0.5f + 0.5f);
    }
}