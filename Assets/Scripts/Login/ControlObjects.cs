using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;

public class ControlObjects : MonoBehaviour
{
    public GameObject obj_login, fade;
    public Image esquerda, direita, frente, tras;
    public Text frente_txt, tras_txt;
    List<GameObject> objs = new List<GameObject>();
    public List<string[]> users = new List<string[]>();
    bool pressed = false;
    public static int moving = 0;
    float time = 0, time_base = 1.5f;
    Position last_pos = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        moving = 0;
        System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

        // Lê os dados do banco de dados
        IDbConnection dbcon = new SqliteConnection(Connection.dbconnection);
        dbcon.Open();
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;
        string query = "SELECT * FROM USUARIO ORDER BY usu_nome ASC";
        cmnd_read.CommandText = query;
        reader = cmnd_read.ExecuteReader();
        while (reader.Read())
        {
            string[] linha = new string[9];
            for (int i = 0; i < 9; i++)
            {
                linha[i] = reader[i].ToString();
            }
            users.Add(linha);
        }
        dbcon.Close();
        
        users.Sort(new CustomSorter());

        MoveAlfabeto.letra = users[0][4][0].ToString();

        int index = 0;
        bool negativo = true;

        if (users.Count == 2)
        {
            users.Add(users[1]);
            users.Add(users[0]);
        }
        else if (users.Count < 2)
        {
            esquerda.transform.localScale = Vector3.zero;
            direita.transform.localScale = Vector3.zero;
        }

        foreach (string[] linha in users)
        {
            GameObject obj = Instantiate(obj_login);
            obj.transform.SetParent(transform);
            obj.transform.SetSiblingIndex(obj.transform.GetSiblingIndex() - 1);
            obj.GetComponentInChildren<Text>().text = linha[4];
            byte[] data = File.ReadAllBytes(Directory.GetCurrentDirectory() + "\\Pictures\\" + linha[8]);
            Texture2D texture = new Texture2D(222, 232, TextureFormat.ARGB32, false);
            texture.LoadImage(data);
            texture.name = Path.GetFileNameWithoutExtension(Directory.GetCurrentDirectory() + "\\Pictures\\" + linha[8]);
            obj.GetComponent<MoveLogin>().pic.sprite = Sprite.Create(texture, new Rect(0, 0, 222, 232), new Vector2(0.5f, 0.5f));
            obj.GetComponent<MoveLogin>().num_objects = users.Count;
            obj.GetComponent<MoveLogin>().id_usuario = int.Parse(linha[0]);
            if (negativo)
            {
                obj.GetComponent<MoveLogin>().actual_position = -(index / 2 + index % 2);
                obj.GetComponent<MoveLogin>().InitPosition(-(index / 2 + index % 2));
            }
            else
            {
                obj.GetComponent<MoveLogin>().actual_position = index / 2 + index % 2;
                obj.GetComponent<MoveLogin>().InitPosition(index / 2 + index % 2);
            }

            negativo = !negativo;
            objs.Add(obj);
            index++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Position pos = GetPos();

        if (pos > 0 && (pos == last_pos || last_pos == 0) && moving == 0)
        {
            last_pos = pos;

            if (time >= time_base)
            {
                ExecMovement(pos);
            }

            if (pos == Position.Left)
                esquerda.color = Color.Lerp(new Color(202f / 255, 202f / 255, 202f / 255), new Color(64f / 255, 62f / 255, 86f / 255), time / time_base);
            else if (pos == Position.Right)
                direita.color = Color.Lerp(new Color(202f / 255, 202f / 255, 202f / 255), new Color(64f / 255, 62f / 255, 86f / 255), time / time_base);
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

            if (last_pos == Position.Random || (time_base == 0.2f && pos != 0))
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
        switch(pos)
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
                    obj.GetComponent<MoveLogin>().MoveRight();
                    if (obj.GetComponent<MoveLogin>().actual_position == 0)
                        MoveAlfabeto.letra = obj.GetComponent<MoveLogin>().txt.text[0].ToString();
                }
            }
        }
        else if (pos == Position.Right && moving == 0)
        {
            if (!pressed)
            {
                pressed = true;
                foreach (GameObject obj in objs)
                {
                    moving++;
                    obj.GetComponent<MoveLogin>().MoveLeft();
                    if (obj.GetComponent<MoveLogin>().actual_position == 0)
                        MoveAlfabeto.letra = obj.GetComponent<MoveLogin>().txt.text[0].ToString();
                }
            }
        }
        else if (pos == Position.Back)
        {
            if (!pressed && moving == 0)
            {
                pressed = true;
                fade.transform.SetAsLastSibling();
                fade.GetComponent<Fade>().FadeIn(true, "AlphabeticalSort");
                moving = 100;
            }
        }
        else if (pos == Position.Front)
        {
            if (!pressed && moving == 0)
            {
                pressed = true;
                foreach (GameObject obj in objs)
                {
                    if (obj.GetComponent<MoveLogin>().actual_position == 0)
                    {
                        ControlHubObjects.nome = obj.GetComponent<MoveLogin>().txt.text;
                        ControlHubObjects.foto = obj.GetComponent<MoveLogin>().pic.sprite;
                        ControlHubObjects.usuario_id = obj.GetComponent<MoveLogin>().id_usuario;
                        break;
                    }
                }
                fade.transform.SetAsLastSibling();

                if (Executar.jogo_id != -1)
                    fade.GetComponent<Fade>().FadeIn(true, "RunningGame");
                else
                    fade.GetComponent<Fade>().FadeIn(true, "Hub");

                moving = 100;
            }
        }

        last_pos = Position.Random;
    }

    public enum Position
    {
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

public class CustomSorter : IComparer<string[]>
{
    public int Compare(string[] x, string[] y)
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string[] subs = chars.Split(MoveAlfabeto.letra[0]);
        chars = MoveAlfabeto.letra;
        chars += subs[1] + subs[0];
        chars += "0123456789";

        if (chars.IndexOf(x[4][0]) < chars.IndexOf(y[4][0]))
            return -1;
        return chars.IndexOf(x[4][0]) > chars.IndexOf(y[4][0]) ? 1 : 0;
    }
}
 