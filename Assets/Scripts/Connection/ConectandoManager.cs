using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.UI;

public class ConectandoManager : MonoBehaviour
{
    int num_obj = 0;
    string nome;
    float bateria;
    SerialPort sp;
    bool blue, addobj = false, destroy = false;
    GameObject go;
    public GameObject loading;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (addobj == true)
        {
            num_obj++;
            float passo = (1280 - 326 * 0.6f * 5) / 8f;
            GameObject ge = Instantiate(go, transform, true);
            ge.GetComponentInChildren<ButtonConnectingControl>().Porta = sp;
            ge.GetComponentInChildren<ButtonConnectingControl>().bluetooth = blue;
            ge.GetComponentInChildren<ButtonConnectingControl>().bateria = bateria;
            ge.GetComponentInChildren<ButtonConnectingControl>().texto_botao.text = nome;
            ge.GetComponentInChildren<ButtonConnectingControl>().texto_com.text = ge.GetComponentInChildren<ButtonConnectingControl>().Porta.PortName;
            ge.transform.localScale = go.transform.localScale;
            if (num_obj > 5)
            {
                ge.GetComponent<RectTransform>().anchoredPosition = new Vector2(((num_obj - 6) * (326f * 0.6f + passo)) + (-640 + 326 * 0.3f + 37.75f * 2), -150);
                loading.GetComponent<RectTransform>().anchoredPosition = new Vector2(((num_obj - 5) * (326f * 0.6f + passo)) + (-640 + 326 * 0.3f + 37.75f * 2), -150 - 20);
            }
            else
            {
                ge.GetComponent<RectTransform>().anchoredPosition = new Vector2(((num_obj - 1) * (326f * 0.6f + passo)) + (-640 + 326 * 0.3f + 37.75f * 2), 150);

                if (num_obj == 5)
                {
                    loading.GetComponent<RectTransform>().anchoredPosition = new Vector2(((num_obj - 5) * (326f * 0.6f + passo)) + (-640 + 326 * 0.3f + 37.75f * 2), -150-20);
                }
                else
                {
                    loading.GetComponent<RectTransform>().anchoredPosition = new Vector2((num_obj * (326f * 0.6f + passo)) + (-640 + 326 * 0.3f + 37.75f * 2), 150-20);
                }
            }
            loading.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
            addobj = false;
        }

        if (destroy)
        {
            Destroy(loading);
        }
    }

    public void AddObject(GameObject g, SerialPort serial, bool bt, string nom, float bat)
    {
        nome = nom;
        bateria = bat;
        go = g;
        sp = serial;
        blue = bt;
        addobj = true;
    }

    public void DestroyLoading()
    {
        destroy = true;
    }
}
