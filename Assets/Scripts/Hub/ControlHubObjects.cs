using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;

public class ControlHubObjects : MonoBehaviour
{
    public GameObject obj_hub, fade, jogos;
    public Image esquerda, direita, frente, tras, img_player;
    public Text frente_txt, tras_txt, player_txt;
    List<GameObject> objs = new List<GameObject>();
    List<string[]> names = new List<string[]>();
    bool pressed = false;
    public static int moving = 0;
    float time = 0, time_base = 1.5f;
    Position last_pos = 0;
    public static int usuario_id = -1;
    public static string nome = "";
    public static Sprite foto;
    
    // Start is called before the first frame update
    void Start()
    {
        img_player.sprite = foto;
        player_txt.text = nome;
        moving = 0;
        System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

        // Lê os dados do banco de dados
        IDbConnection dbcon = new SqliteConnection(Connection.dbconnection);
        dbcon.Open();
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;
        string query = "SELECT * FROM JOGOS";
        cmnd_read.CommandText = query;
        reader = cmnd_read.ExecuteReader();
        while (reader.Read())
        {
            string nome = reader[2].ToString();
            string exe = reader[5].ToString();
            string logo = reader[3].ToString();
            string bg = reader[4].ToString();
            string id = reader[0].ToString();

            names.Add(new string[] { nome, exe, logo, bg, id });
        }
        dbcon.Close();

        int index = 0;

        foreach (string[] name in names)
        {
            GameObject obj = Instantiate(obj_hub);
            obj.transform.SetParent(jogos.transform);
            obj.transform.SetSiblingIndex(obj.transform.GetSiblingIndex() - 1);
            obj.GetComponentInChildren<Text>().text = name[0];
            obj.GetComponent<MoveHub>().id = int.Parse(name[4]);
            obj.GetComponent<MoveHub>().num_objects = names.Count + 1;
            obj.GetComponent<MoveHub>().exe = Directory.GetCurrentDirectory() + "\\Games\\" + name[1];
            obj.GetComponent<MoveHub>().path_pic = Directory.GetCurrentDirectory() + "\\Games\\" + name[2];
            obj.GetComponent<MoveHub>().path_bg = Directory.GetCurrentDirectory() + "\\Games\\" + name[3];
            byte[] data = File.ReadAllBytes(Directory.GetCurrentDirectory() + "\\Games\\" + name[2]);
            Texture2D texture = new Texture2D(240, 240, TextureFormat.ARGB32, false);
            texture.LoadImage(data);
            texture.name = Path.GetFileNameWithoutExtension(Directory.GetCurrentDirectory() + "\\Games\\" + name[2]);
            obj.GetComponent<MoveHub>().pic.sprite = Sprite.Create(texture, new Rect(0, 0, 240, 240), new Vector2(0.5f, 0.5f));

            obj.GetComponent<MoveHub>().actual_position = index;
            obj.GetComponent<MoveHub>().InitPosition(index);

            objs.Add(obj);
            index++;
        }

        // Cria o Evolução
        GameObject obj2 = Instantiate(obj_hub);
        obj2.transform.SetParent(jogos.transform);
        obj2.transform.SetSiblingIndex(obj2.transform.GetSiblingIndex() - 1);
        switch (Connection.idioma)
        {
            case "pt":
                obj2.GetComponentInChildren<Text>().text = "Evolução";
                break;
            case "es":
                obj2.GetComponentInChildren<Text>().text = "Evolución";
                break;
            case "en":
                obj2.GetComponentInChildren<Text>().text = "Evolution";
                break;
            default:
                obj2.GetComponentInChildren<Text>().text = "Evolução";
                break;
        }
        
        obj2.GetComponent<MoveHub>().num_objects = names.Count + 1;
        obj2.GetComponent<MoveHub>().exe = "Evolution";
        obj2.GetComponent<MoveHub>().path_pic = Directory.GetCurrentDirectory() + "\\Evolution.png";
        byte[] data2 = File.ReadAllBytes(Directory.GetCurrentDirectory() + "\\Evolution.png");
        Texture2D texture2 = new Texture2D(240, 240, TextureFormat.ARGB32, false);
        texture2.LoadImage(data2);
        texture2.name = "Evolution";
        obj2.GetComponent<MoveHub>().pic.sprite = Sprite.Create(texture2, new Rect(0, 0, 240, 240), new Vector2(0.5f, 0.5f));
        if (index > 3)
            index = -1;

        obj2.GetComponent<MoveHub>().actual_position = index;
        obj2.GetComponent<MoveHub>().InitPosition(index);

        objs.Add(obj2);
    }

    // Update is called once per frame
    void Update()
    {
        Position pos = GetPos();

        if (pos != Position.None && (pos == last_pos || last_pos == Position.None) && moving == 0)
        {
            last_pos = pos;

            if (time >= time_base)
            {
                ExecMovement(pos);
            }

            if (pos == Position.Right)
            {
                esquerda.color = Color.Lerp(new Color(202f / 255, 202f / 255, 202f / 255), new Color(64f / 255, 62f / 255, 86f / 255), time / time_base);
            }
            else if (pos == Position.Left)
            {
                direita.color = Color.Lerp(new Color(202f / 255, 202f / 255, 202f / 255), new Color(64f / 255, 62f / 255, 86f / 255), time / time_base);
            }
            else if (pos == Position.Back)
            {
                tras.color = Color.Lerp(new Color(202f / 255, 202f / 255, 202f / 255), new Color(64f / 255, 62f / 255, 86f / 255), time / time_base);
                tras_txt.color = Color.Lerp(new Color(202f / 255, 202f / 255, 202f / 255), new Color(64f / 255, 62f / 255, 86f / 255), time / time_base);
            }
            else if (pos == Position.Front)
            {
                frente.color = Color.Lerp(new Color(202f / 255, 202f / 255, 202f / 255), new Color(64f / 255, 62f / 255, 86f / 255), time / time_base);
                frente_txt.color = Color.Lerp(new Color(202f / 255, 202f / 255, 202f / 255), new Color(64f / 255, 62f / 255, 86f / 255), time / time_base);
            }

            time += Time.deltaTime;
        }
        else
        {
            esquerda.color = new Color(202f / 255, 202f / 255, 202f / 255);
            direita.color = new Color(202f / 255, 202f / 255, 202f / 255);
            tras.color = new Color(202f / 255, 202f / 255, 202f / 255);
            frente.color = new Color(202f / 255, 202f / 255, 202f / 255);
            tras_txt.color = new Color(202f / 255, 202f / 255, 202f / 255);
            frente_txt.color = new Color(202f / 255, 202f / 255, 202f / 255);

            if (last_pos == Position.Random || (time_base == 0.2f && pos != Position.None))
                time_base = 0.2f;
            else
                time_base = 1.5f;

            time = 0;
            last_pos = 0;
            pressed = false;
        }
    }

    public void VerifyPos(int pos)
    {
        switch (pos)
        {
            case 1:
                ExecMovement(Position.Front);
                break;
            case 2:
                ExecMovement(Position.Back);
                break;
            case 3:
                ExecMovement(Position.Right);
                break;
            case 4:
                ExecMovement(Position.Left);
                break;
        }
    }

    public void ExecMovement(Position pos)
    {
        if (pos == Position.Left)
        {
            if (!pressed && moving == 0)
            {
                pressed = true;
                foreach (GameObject obj in objs)
                {
                    moving++;
                    obj.GetComponent<MoveHub>().MoveRight();
                }
            }
        }
        else if (pos == Position.Right)
        {
            if (!pressed && moving == 0)
            {
                pressed = true;
                foreach (GameObject obj in objs)
                {
                    moving++;
                    obj.GetComponent<MoveHub>().MoveLeft();
                }
            }
        }
        else if (pos == Position.Back)
        {
            if (!pressed && moving == 0)
            {
                pressed = true;
                fade.transform.SetAsLastSibling();
                fade.GetComponent<Fade>().FadeIn(true, "Login");
                usuario_id = -1;
                moving = 100;
            }
        }
        else if (pos == Position.Front)
        {
            if (!pressed && moving == 0)
            {
                pressed = true;
                fade.transform.SetAsLastSibling();
                foreach (GameObject obj in objs)
                {
                    if (obj.GetComponent<MoveHub>().actual_position == 0)
                    {
                        Executar.path = obj.GetComponent<MoveHub>().exe;
                        if (Executar.path.Contains(".exe"))
                        {
                            byte[] data = File.ReadAllBytes(obj.GetComponent<MoveHub>().path_bg);
                            Texture2D texture = new Texture2D(240, 240, TextureFormat.ARGB32, false);
                            texture.LoadImage(data);
                            texture.name = Path.GetFileNameWithoutExtension(obj.GetComponent<MoveHub>().path_bg);
                            Executar.jogo_id = obj.GetComponent<MoveHub>().id;
                            Executar.bgSprite = Sprite.Create(texture, new Rect(0, 0, 1280, 720), new Vector2(0.5f, 0.5f));
                            Executar.logoSprite = obj.GetComponent<MoveHub>().pic.sprite;
                            fade.GetComponent<Fade>().FadeIn(true, "RunningGame");
                            EnviandoDados.actual_app = obj.GetComponent<MoveHub>().txt.text;
                            EnviandoDados.actual_app_pic = obj.GetComponent<MoveHub>().path_pic;
                            moving = 100;
                        }
                        else if (Executar.path ==  "Evolution")
                        {
                            fade.GetComponent<Fade>().FadeIn(true, "Evolution");
                        }
                        break;
                    }
                }
            }
        }

        last_pos = Position.Random;
    }

    public enum Position {
        None = 0,
        Front = 1,
        Back = 2,
        Right = 3,
        Left = 4,
        Random = 5
    }

    Position GetPos()
    {
        Position p = Position.None;

        try
        {
            if (Input.GetAxis("Vertical") > 0)
                ExecMovement(Position.Front);
            else if (Input.GetAxis("Vertical") < 0)
                ExecMovement(Position.Back);
            else if (Input.GetAxis("Horizontal") > 0)
                ExecMovement(Position.Right);
            else if (Input.GetAxis("Horizontal") < 0)
                ExecMovement(Position.Left);

            string[] points = Connection.dispositivo[0].GetPoint().Split('|');

            if (float.Parse(points[0]) < -0.4)
                p = Position.Left;
            else if (float.Parse(points[0]) > 0.4)
                p = Position.Right;
            else if (float.Parse(points[1]) < -0.4)
                p = Position.Back;
            else if (float.Parse(points[1]) > 0.4)
                p = Position.Front;
        }
        catch (System.Exception ex)
        {
            // Não faz nada
        }

        return p;
    }
}
 