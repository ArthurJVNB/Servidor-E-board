using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using Assets.Scripts;
using System;
using UnityEngine.EventSystems;
using System.Threading;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class Connection : MonoBehaviour
{
    public bool sem_fio, not_found = false;
    public static bool connected = false;
    public static string idioma = "pt";
    public static string dbconnection = "URI=file:" + System.IO.Directory.GetCurrentDirectory() + @"\Database\database.db";
    public GameObject tela_conectando, enviando, fade;
    UnityEngine.UI.Text texto;
    public static List<Eboard> dispositivo = new List<Eboard>();
    Thread verifying;
    Thread ThreadBT, ThreadUSB;
    string state = "";
    public GameObject obj_connecting;
    public static int players = 0;
    ConectandoManager cm;

    public static bool PtrInitialized = false;
    public static IntPtr unityWindow, previousWindow;

    [DllImport("user32.dll")]
    static extern IntPtr GetActiveWindow();

    // Start is called before the first frame update
    void Start()
    {
        if (PtrInitialized == false)
        {
            unityWindow = GetActiveWindow();
            PtrInitialized = true;
        }
        
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(Clique);
    }

    // Update is called once per frame
    void Update()
    {
        VerifyConnection();
    }

    private void VerifyConnection()
    {
        if (connected)
        {
            StartCoroutine(Connecting());
            connected = false;
        }
        else if (not_found)
        {
            Destroy(GameObject.FindGameObjectWithTag("Conectando"));
            not_found = false;
        }
    }

    IEnumerator Connecting()
    {
        yield return null;
        DontDestroyOnLoad(Instantiate(enviando));
        fade.transform.SetAsLastSibling();
        //fade.GetComponent<Fade>().FadeIn(true, "Calibration");
        if (Executar.jogo_id != -1)
            fade.GetComponent<Fade>().FadeIn(true, "RunningGame");
        else if (ControlHubObjects.usuario_id != -1)
            fade.GetComponent<Fade>().FadeIn(true, "Hub");
        else
            fade.GetComponent<Fade>().FadeIn(true, "Login");
    }

    void Clique()
    {
        GameObject g = Instantiate(tela_conectando);
        g.transform.parent = gameObject.GetComponentInParent<Canvas>().gameObject.transform;
        g.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        g.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        cm = FindObjectOfType<ConectandoManager>();

        if (sem_fio)
        {
            ThreadBT = new Thread(ConnectBT);
            ThreadBT.Start();
        }
        else
        {
            ThreadUSB = new Thread(ConnectUSB);
            ThreadUSB.Start();
        }
    }

    void SetText(string text)
    {
        if (texto != null)
            texto.text = text;
    }

    string GetText()
    {
        return texto.text;
    }

    void SetState(string text)
    {
        state = text;
    }

    string GetState()
    {
        return state;
    }

    public static void SetConnection()
    {
        connected = true;
    }

    void SetNotFound()
    {
        not_found = true;
    }

    void ReadLine(SerialPort Porta, out string line)
    {
        Porta.ReadTo("\n");
        line = Porta.ReadLine();
    }

    SerialPort CreatePort(string Name, Int32 BaudRate)
    {
        SerialPort Port = new SerialPort(Name, BaudRate, Parity.None, 8, StopBits.One);
        Port.DtrEnable = true;
        Port.Handshake = Handshake.None;
        Port.ReadTimeout = 500;
        Port.WriteTimeout = 500;

        if (Port.IsOpen)
            Port.Close();

        return Port;
    }

    void ConnectUSB()
    {
        SetState("Verificando portas USB...");

        int found = 0;

        foreach (string port in SerialPort.GetPortNames())
        {
            bool error = false;
            SerialPort Port = CreatePort(port, 115200);

            try
            {
                Port.Open();
                SetState("Verificando dispositivo (" + port + ")...");
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                error = true;
                SetState("Verificando portas USB...");
            }

            Thread.Sleep(1000);

            if (!error)
            {
                string line = "";
                int count = 0;
                while (count < 3)
                {
                    try
                    {
                        ReadLine(Port, out line);
                        break;
                    }
                    catch (Exception ex)
                    {
                        count++;
                    }
                }

                if (line.Contains("eboard") && line[10] == 'c')
                {
                    GameObject g = obj_connecting;
                    cm.AddObject(g, Port, false, "Eboard-" + line.Substring(6, 4), float.Parse(line.Substring(11, line.IndexOf('%') - 11)));
                    HubPanelImages.BT = false;
                    found++;
                }
                else
                {
                    Port.Close();
                    SetState("Verificando portas USB...");
                }
            }
        }

        cm.DestroyLoading();

        if (found == 0)
        {
            SetState("Nenhuma E-board encontrada!");
            Thread.Sleep(2000);
            SetNotFound();
        }
    }

    void ConnectBT()
    {
        SetState("Verificando conexões Bluetooth...");

        int found = 0;

        foreach (string port in SerialPort.GetPortNames())
        {
            bool error = false;
            SerialPort Port = CreatePort(port, 9600);

            try
            {
                SetState("Verificando dispositivo (" + port + ")...");
                Port.Open();
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                error = true;
                SetState("Verificando conexões Bluetooth...");
            }

            Thread.Sleep(1000);

            if (!error)
            {
                string line = "";
                int count = 0;
                while (count < 3)
                {
                    try
                    {
                        bool started = false;

                        while (Port.BytesToRead > 0)
                        {
                            char c = (char)Port.ReadByte();
                            if (started)
                            {
                                line += c;
                                if (c == '\n')
                                    break;
                            }
                            if (c == 'e')
                            {
                                line += c;
                                started = true;
                            }
                        }
                        break;
                    }
                    catch (Exception ex)
                    {
                        count++;
                    }
                }

                if (line.Contains("eboard") && line[10] == 'b')
                {
                    GameObject g = obj_connecting;
                    cm.AddObject(g, Port, true, "Eboard-" + line.Substring(6, 4), float.Parse(line.Substring(11, line.IndexOf('%') - 11)));
                    HubPanelImages.BT = true;
                    found++;
                }
                else
                {
                    Port.Close();
                    SetState("Verificando conexões Bluetooth...");
                }
            }
        }

        cm.DestroyLoading();

        if (found == 0)
        {
            SetState("Nenhuma E-board encontrada!");
            Thread.Sleep(2000);
            SetNotFound();
        }
    }
}
