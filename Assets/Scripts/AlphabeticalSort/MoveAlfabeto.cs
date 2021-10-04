using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveAlfabeto : MonoBehaviour
{
    public Text[] Alfabeto;
    public Image frente;
    public Fade fade;
    float velocidade = 500f;
    float velocidade_lerp = 10f;
    float time_base = 1.5f, time = 0f;
    public static string letra = "A";

    // Start is called before the first frame update
    void Start()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

        transform.localPosition = new Vector3((int)(letra[0] - 'A') * -200, transform.localPosition.y);
    }

    // Update is called once per frame
    void Update()
    {
        Position pos = GetPos();

        if (pos != Position.Front)
        {
            ExecMovement(pos);
            time = 0;
        }
        else
        {
            time += Time.deltaTime;
            int index_selected = Mathf.Clamp(Mathf.RoundToInt(Mathf.Abs(transform.localPosition.x / 200)), 0, 25);

            Alfabeto[index_selected].color = Color.Lerp(new Color(202 / 255f, 202 / 255f, 202 / 255f), new Color(1, 1, 1), time / time_base);
            if (time >= time_base)
                ExecMovement(pos);
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
        int index_selected = Mathf.Clamp(Mathf.RoundToInt(Mathf.Abs(transform.localPosition.x / 200)), 0, 25);

        if (pos == Position.Right)
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, new Vector3((Alfabeto.Length - 1) * -200, transform.localPosition.y), Time.deltaTime * velocidade);
        else if (pos == Position.Left)
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, new Vector3(0, transform.localPosition.y), Time.deltaTime * velocidade);
        else
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(index_selected * -200, transform.localPosition.y), Time.deltaTime * velocidade_lerp);

        foreach (Text t in Alfabeto)
        {
            t.color = new Color(49 / 255f, 48 / 255f, 68 / 255f);
        }

        Alfabeto[index_selected].color = new Color(202 / 255f, 202 / 255f, 202 / 255f);

        if (pos == Position.Front)
        {
            letra = Alfabeto[index_selected].text;
            fade.transform.SetAsLastSibling();
            fade.GetComponent<Fade>().FadeIn(true, "Login");
        }
    }

    public void VerifyMouse(string letter)
    {
        letra = letter;
        fade.transform.SetAsLastSibling();
        fade.GetComponent<Fade>().FadeIn(true, "Login");
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
