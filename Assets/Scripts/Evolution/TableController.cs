using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TableController : MonoBehaviour
{
    public GameObject line, combobox;
    public Fade fade;
    public Text title;
    public int jogo_id = 0;

    public void PorPontos()
    {
        combobox.SetActive(true);

        foreach (LineTable todestroy in FindObjectsOfType<LineTable>())
        {
            Destroy(todestroy.gameObject);
        }

        List<string[]> linhas = new List<string[]>();

        jogo_id = ComboBoxController.id;

        // Lê os dados do banco de dados
        IDbConnection dbcon = new SqliteConnection(Connection.dbconnection);
        dbcon.Open();
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;
        string query = "SELECT u.usu_img, u.usu_nome, l.pontuacao_atual, l.altura_atual, l.peso_registrado FROM LMS l, USUARIO u WHERE l.usu_id = u.usu_id AND l.jogo_id = " + jogo_id.ToString() + " AND l.hora_entrada = (SELECT MAX(hora_entrada) FROM lms WHERE usu_id = l.usu_id AND pontuacao_atual = (SELECT MAX(pontuacao_atual) FROM lms WHERE usu_id = l.usu_id AND jogo_id = " + jogo_id.ToString() + ")) ORDER BY l.pontuacao_atual DESC";
        cmnd_read.CommandText = query;
        reader = cmnd_read.ExecuteReader();
        while (reader.Read())
        {
            string[] linha = new string[5];
            for (int i = 0; i < 5; i++)
            {
                linha[i] = reader[i].ToString();
            }
            linhas.Add(linha);
        }
        dbcon.Close();

        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(transform.GetComponent<RectTransform>().sizeDelta.x, (linhas.Count + 1) * line.GetComponent<RectTransform>().sizeDelta.y);
        transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0);

        for (int i = 1; i <= linhas.Count; i++)
        {
            GameObject newObject = Instantiate(line);
            LineTable lt = newObject.GetComponent<LineTable>();
            lt.id = i - 1;
            byte[] data = File.ReadAllBytes(Directory.GetCurrentDirectory() + "\\Pictures\\" + linhas[lt.id][0]);
            Texture2D texture = new Texture2D(222, 232, TextureFormat.ARGB32, false);
            texture.LoadImage(data);
            texture.name = Path.GetFileNameWithoutExtension(Directory.GetCurrentDirectory() + "\\Pictures\\" + linhas[lt.id][0]);
            lt.img.sprite = Sprite.Create(texture, new Rect(0, 0, 222, 232), new Vector2(0.5f, 0.5f));
            lt.nome.text = linhas[lt.id][1];
            lt.pontuacao.text = linhas[lt.id][2];
            lt.altura.text = linhas[lt.id][3];
            lt.peso.text = linhas[lt.id][4];
            newObject.transform.SetParent(transform);
            newObject.transform.localScale = line.transform.localScale;
            newObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(line.GetComponent<RectTransform>().anchoredPosition.x, line.GetComponent<RectTransform>().anchoredPosition.y - i * line.GetComponent<RectTransform>().sizeDelta.y);
        }

        switch(Connection.idioma)
        {
            case "pt":
                title.text = "Pontuação";
                break;
            case "es":
                title.text = "Pontuación";
                break;
            case "en":
                title.text = "Score";
                break;
            default:
                title.text = "Pontuação";
                break;
        }

        LineTable.selected = 0;
    }

    public void PorIMC()
    {
        combobox.SetActive(false);

        foreach (LineTable todestroy in FindObjectsOfType<LineTable>())
        {
            Destroy(todestroy.gameObject);
        }

        List<string[]> linhas = new List<string[]>();

        // Lê os dados do banco de dados
        IDbConnection dbcon = new SqliteConnection(Connection.dbconnection);
        dbcon.Open();
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;
        string query = "SELECT u.usu_img, u.usu_nome, ROUND(l.peso_registrado / (l.altura_atual * l.altura_atual), 2) AS imc, l.altura_atual, l.peso_registrado FROM LMS l, USUARIO u WHERE l.usu_id = u.usu_id AND l.hora_entrada = (SELECT MAX(hora_entrada) FROM lms WHERE usu_id = l.usu_id) ORDER BY imc DESC";
        cmnd_read.CommandText = query;
        reader = cmnd_read.ExecuteReader();
        while (reader.Read())
        {
            string[] linha = new string[5];
            for (int i = 0; i < 5; i++)
            {
                linha[i] = reader[i].ToString();
            }
            linhas.Add(linha);
        }
        dbcon.Close();

        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(transform.GetComponent<RectTransform>().sizeDelta.x, (linhas.Count + 1) * line.GetComponent<RectTransform>().sizeDelta.y);
        transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0);

        for (int i = 1; i <= linhas.Count; i++)
        {
            GameObject newObject = Instantiate(line);
            LineTable lt = newObject.GetComponent<LineTable>();
            lt.id = i - 1;
            byte[] data = File.ReadAllBytes(Directory.GetCurrentDirectory() + "\\Pictures\\" + linhas[lt.id][0]);
            Texture2D texture = new Texture2D(222, 232, TextureFormat.ARGB32, false);
            texture.LoadImage(data);
            texture.name = Path.GetFileNameWithoutExtension(Directory.GetCurrentDirectory() + "\\Pictures\\" + linhas[lt.id][0]);
            lt.img.sprite = Sprite.Create(texture, new Rect(0, 0, 222, 232), new Vector2(0.5f, 0.5f));
            lt.nome.text = linhas[lt.id][1];
            lt.pontuacao.text = linhas[lt.id][2];
            lt.altura.text = linhas[lt.id][3];
            lt.peso.text = linhas[lt.id][4];
            newObject.transform.SetParent(transform);
            newObject.transform.localScale = line.transform.localScale;
            newObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(line.GetComponent<RectTransform>().anchoredPosition.x, line.GetComponent<RectTransform>().anchoredPosition.y - i * line.GetComponent<RectTransform>().sizeDelta.y);
        }
        
        switch (Connection.idioma)
        {
            case "pt":
                title.text = "IMC";
                break;
            case "es":
                title.text = "IMC";
                break;
            case "en":
                title.text = "BMI";
                break;
            default:
                title.text = "IMC";
                break;
        }

        LineTable.selected = 0;
    }

    public void Exit()
    {
        fade.transform.SetAsLastSibling();
        fade.GetComponent<Fade>().FadeIn(true, "Hub");
    }
}
