using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonConnectingControl : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    Image img;
    public Image board;
    public float bateria;
    public Text texto_botao, texto_com;
    private float holdTime = 0.75f, counter = 0;
    private bool held = false, longPress = false;
    bool selected = false;
    public int numPlayer = 0;
    public GameObject top;
    public GameObject top_exist;
    public SerialPort Porta;
    public bool bluetooth = false, allowMultiplayer = false;

    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (counter >= holdTime && allowMultiplayer)
        {
            held = true;
            longPress = false;
            counter = 0;
            CliqueLongo();
        }

        if (longPress)
        {
            counter += Time.deltaTime;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selected)
        {
            board.color = new Color(1, 1, 1, 0.75f);
            img.color = new Color(232 / 255f, 232 / 255f, 232 / 255f);
        }
        else
        {
            board.color = new Color(1, 1, 1, 0.65f);
            img.color = new Color(232 / 255f, 232 / 255f, 232 / 255f, 0.5f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selected)
        {
            board.color = new Color(1, 1, 1, 1f);
            img.color = new Color(232 / 255f, 232 / 255f, 232 / 255f);
        }
        else
        {
            board.color = new Color(1, 1, 1, 0.5f);
            img.color = new Color(1, 1, 1);
        }

        longPress = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        held = false;
        longPress = true;
        counter = 0;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        counter = 0;

        if (!held && longPress)
            Clique();

        longPress = false;
    }

    void Clique()
    {
        if (Connection.players == 0)
        {
            selected = true;
            numPlayer = ++Connection.players;
            Connection.dispositivo.Add(new Eboard(Porta, bluetooth, texto_botao.GetComponentInChildren<Text>().text, bateria));
        }
        else if (!selected)
        {
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("BaseConnecting"))
            {
                Destroy(g.GetComponent<ButtonConnectingControl>().top_exist);
                g.GetComponent<ButtonConnectingControl>().top_exist = null;
                g.GetComponent<ButtonConnectingControl>().texto_botao.color = new Color(202 / 255f, 202 / 255f, 202 / 255f);
                g.GetComponent<ButtonConnectingControl>().board.color = new Color(1, 1, 1, 0.5f);
                g.GetComponent<ButtonConnectingControl>().img.color = new Color(1, 1, 1);
                top_exist = null;
            }

            foreach (Eboard e in Connection.dispositivo)
            {
                e.Disconnect();
                Connection.dispositivo.Remove(e);
            }

            Connection.dispositivo.Add(new Eboard(Porta, bluetooth, texto_botao.GetComponentInChildren<Text>().text, bateria));
            selected = true;
            Connection.players = 1;
        }

        texto_botao.color = new Color(63 / 255f, 61 / 255f, 86 / 255f);
        board.color = new Color(1, 1, 1, 1f);
        img.color = new Color(232 / 255f, 232 / 255f, 232 / 255f);
        Connection.SetConnection();
    }

    void CliqueLongo()
    {
        if (selected)
        {
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("BaseConnecting"))
            {
                if (g.GetComponent<ButtonConnectingControl>().numPlayer > numPlayer)
                {
                    g.GetComponent<ButtonConnectingControl>().top_exist.GetComponentInChildren<Text>().text = "P" + (--g.GetComponent<ButtonConnectingControl>().numPlayer).ToString();
                }
            }

            Connection.dispositivo[numPlayer - 1].Disconnect();
            Connection.dispositivo.Remove(Connection.dispositivo[numPlayer - 1]);

            texto_botao.color = new Color(202 / 255f, 202 / 255f, 202 / 255f);
            board.color = new Color(1, 1, 1, 0.5f);
            img.color = new Color(1, 1, 1);

            numPlayer = 0;
            --Connection.players;
            selected = false;
            Destroy(top_exist);
            top_exist = null;
        }
        else
        {
            selected = true;
            texto_botao.color = new Color(63 / 255f, 61 / 255f, 86 / 255f);
            board.color = new Color(1, 1, 1, 1f);
            img.color = new Color(232 / 255f, 232 / 255f, 232 / 255f);
            Connection.dispositivo.Add(new Eboard(Porta, bluetooth, texto_botao.GetComponentInChildren<Text>().text, bateria));
            numPlayer = ++Connection.players;
            top_exist = Instantiate(top, transform, true);
            top_exist.GetComponentInChildren<Text>().text = "P" + numPlayer.ToString();
            top_exist.transform.localScale = top.transform.localScale;
            top_exist.GetComponent<RectTransform>().anchoredPosition = top.GetComponent<RectTransform>().anchoredPosition;
        }
    }
}
