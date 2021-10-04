using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connecting : MonoBehaviour
{
    UnityEngine.UI.Text texto;

    // Start is called before the first frame update
    void Start()
    {
        texto = GetComponent<UnityEngine.UI.Text>();
    }

    // Update is called once per frame
    void Update()
    {
        texto.color = new Color(texto.color.r, texto.color.g, texto.color.b, Mathf.PingPong(Time.time, 1) * 0.5f + 0.5f);
    }
}