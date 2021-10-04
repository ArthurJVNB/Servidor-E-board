using Assets.Scripts;
using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnviandoDados : MonoBehaviour
{
    Thread ThreadTCP, ThreadUSB;
    bool disconnect = false, switch_login = false;
    public static string actual_app, actual_app_pic;
    TcpListener tcpListener = null;

    [DllImport("user32.dll")]
    static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

    const int ALT = 0xA4;
    const int EXTENDEDKEY = 0x1;
    const int KEYUP = 0x2;

    private void RefocusWindow()
    {
        // Simulate alt press
        keybd_event((byte)ALT, 0x45, EXTENDEDKEY | 0, 0);

        // Simulate alt release
        keybd_event((byte)ALT, 0x45, EXTENDEDKEY | KEYUP, 0);

        SetForegroundWindow(Connection.unityWindow);
    }

    // Start is called before the first frame update
    void Start()
    {
        ThreadTCP = new Thread(ServerTCP);
        ThreadTCP.Start();
        ThreadUSB = new Thread(ServerSerial);
        ThreadUSB.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (disconnect)
        {
            ThreadTCP.Abort();

            // Dá destaque pro Unity
            RefocusWindow();

            if (Connection.idioma == "es")
                LaunchNotification("La Eboard ha sido desconectada.", 0);
            else if (Connection.idioma == "en")
                LaunchNotification("The Eboard was disconnected.", 0);
            else
                LaunchNotification("A Eboard foi desconectada.", 0);

            if (SceneManager.GetActiveScene().name != "Connection")
                SceneManager.LoadSceneAsync("Connection");

            Destroy(GameObject.FindGameObjectWithTag("Conectando"));
            Destroy(gameObject);
        }
        else if (switch_login)
        {
            SceneManager.LoadSceneAsync("Login");
            switch_login = false;
        }
    }

    private void ServerSerial()
    {
        Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        Eboard erro = null;

        while (true)
        {
            // Conexão USB
            try
            {
                foreach (Eboard e in Connection.dispositivo)
                {
                    erro = e;
                    e.Measure();

                    if (e.LowBattery == false && e.AverageBattery <= 20)
                    {
                        if (Connection.idioma == "es")
                            LaunchNotification("Batería baja!", 0);
                        else if (Connection.idioma == "en")
                            LaunchNotification("Low battery!", 0);
                        else
                            LaunchNotification("Bateria fraca!", 0);
                        e.LowBattery = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                Connection.dispositivo.Remove(erro);
                disconnect = true;
                break;
            }
        }
    }

    private void ServerTCP()
    {
        Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
        IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 2001);

        tcpListener = new TcpListener(localEndPoint);
        tcpListener.Start();

        while (true)
        {
            if (tcpListener.Pending())
            {
                Thread tmp_thread = new Thread(new ThreadStart(AsyncConnection));
                tmp_thread.Start();
            }

            Thread.Sleep(100);
        }
    }


    private void AsyncConnection()
    {
        Socket socket = tcpListener.AcceptSocket();
        string request = "";
        NetworkStream nStream = new NetworkStream(socket);
        BinaryWriter bWriter = new BinaryWriter(nStream);
        BinaryReader bReader = new BinaryReader(nStream);
        bool canSend = false;

        while (true)
        {
            // Conexão TCP
            try
            {
                if (disconnect)
                    break;

                request = bReader.ReadString();

                foreach (Eboard e in Connection.dispositivo)
                {
                    switch (request)
                    {
                        case "pararLeitura":
                            canSend = false;
                            bWriter.Write("pararLeitura");
                            break;
                        case "iniciarLeitura":
                            canSend = true;
                            bWriter.Write("iniciarLeitura");
                            break;
                        case "GravityData":
                            if (canSend)
                                bWriter.Write(e.GetGravityData());
                            break;
                        case "eboard-data":
                            bWriter.Write(e.GetData());
                            break;
                        case "eboard-point":
                            bWriter.Write(e.GetPoint());
                            break;
                        case "eboard-weight":
                            bWriter.Write(e.GetWeight());
                            break;
                        case "eboard-raw":
                            bWriter.Write(e.GetRaw());
                            break;
                        case "eboard-allweights":
                            bWriter.Write(e.GetAllWeights());
                            break;
                        case "eboard-ranking":
                            bWriter.Write(GetRanking(Executar.jogo_id));
                            break;
                        case "eboard-switch":
                            // Dá destaque pro Unity
                            RefocusWindow();

                            switch_login = true;
                            break;
                        default:
                            if (request.Contains("eboard-lms"))
                            {
                                string[] splitted = request.Replace("eboard-lms|", "").Split('|');
                                LMS(0, ControlHubObjects.usuario_id, Executar.jogo_id, splitted[0], splitted[1], float.Parse(splitted[2]), float.Parse(splitted[3]), int.Parse(splitted[4]), int.Parse(splitted[5]), int.Parse(splitted[6]), int.Parse(splitted[7]), 0);
                            }
                            else if (request.Contains("eboard-window"))
                            {
                                Executar.gameWindow = new IntPtr(Convert.ToInt32(request.Replace("eboard-window|", "")));
                            }
                            else
                            {
                                bWriter.Write("eboard-unknown");
                                Debug.Log("Requisição não reconhecida.");
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                // Problema ao ler
                Debug.Log(ex);
                break;
            }
        }

        socket.Close();
    }
    
    private string GetRanking(int id)
    {
        // Escreve os dados no banco de dados
        IDbConnection dbcon = new SqliteConnection(Connection.dbconnection);
        dbcon.Open();
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;
        string query = "SELECT u.usu_nome, l.pontuacao_atual, strftime('%d/%m/%Y', l.hora_entrada) FROM USUARIO u, LMS l WHERE u.usu_id = l.usu_id AND l.jogo_id = " + id.ToString() + " AND l.hora_entrada = (SELECT MAX(hora_entrada) FROM lms WHERE usu_id = l.usu_id AND pontuacao_atual = (SELECT MAX(pontuacao_atual) FROM lms WHERE usu_id = l.usu_id AND jogo_id = " + id.ToString() + ")) ORDER BY l.pontuacao_atual DESC";
        cmnd_read.CommandText = query;
        reader = cmnd_read.ExecuteReader();
        string result = "";
        int pos = 1;
        while (reader.Read())
        {
            result += (pos++).ToString() + "|";
            for (int i = 0; i < 3; i++)
            {
                result += reader[i].ToString() + "|";
            }
        }
        dbcon.Close();

        return result;
    }

    public static void LMS(int lms_id_plataforma, int usu_id, int jogo_id, string hora_entrada, string hora_saida, float peso_registrado, float altura_atual, int pontuacao_atual, int vida_atual, int tempo_atual, int nivel_atual, int sincronizacao)
    {
        // Escreve os dados no banco de dados
        IDbConnection dbcon = new SqliteConnection(Connection.dbconnection);
        dbcon.Open();
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;
        string query = "INSERT INTO LMS (lms_id_plataforma, usu_id, jogo_id, hora_entrada, hora_saida, peso_registrado, altura_atual, pontuacao_atual, vida_atual, tempo_atual, nivel_atual, sincronizacao) VALUES (" + lms_id_plataforma.ToString() + ", " + usu_id.ToString() + ", " + jogo_id.ToString() + ", '" + hora_entrada + "', '" + hora_saida + "', " + peso_registrado.ToString() + ", " + altura_atual.ToString() + ", " + pontuacao_atual.ToString() + ", " + vida_atual.ToString() + ", " + tempo_atual.ToString() + ", " + nivel_atual.ToString() + ", " + sincronizacao.ToString() + ")";
        cmnd_read.CommandText = query;
        reader = cmnd_read.ExecuteReader();
        dbcon.Close();
    }

    public static void LaunchNotification(string msg, int img)
    {
        // 0 = img sys, 1 = img user, 2 = img game
        try
        {
            using (System.IO.StreamWriter sw = new StreamWriter(@"eboard.notify", false))
            {
                sw.WriteLine(msg);
                if (img == 0)
                {
                    sw.WriteLine("logo.png");
                }
                else if (img == 1)
                {
                    sw.WriteLine("user.png");
                }
                else if (img == 2)
                {
                    sw.WriteLine(actual_app_pic);
                }
            }

            System.Diagnostics.Process.Start(@"Notify.exe");
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }
}
