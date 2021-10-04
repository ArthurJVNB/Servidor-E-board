using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hora : MonoBehaviour
{
    UnityEngine.UI.Text txt;

    // Start is called before the first frame update
    void Start()
    {
        txt = GetComponent<UnityEngine.UI.Text>();
    }

    // Update is called once per frame
    void Update()
    {
        txt.text = DateTime.Now.ToShortTimeString();
    }
}
