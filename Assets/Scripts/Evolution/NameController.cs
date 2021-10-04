using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class NameController : MonoBehaviour
{
    public GameObject Combobox;
    public TableController Table;
    public static bool EndInstatiate = false;

    void Start()
    {
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(Click);

        ComboBoxController.games.Clear();

        IDbConnection dbcon = new SqliteConnection(Connection.dbconnection);
        dbcon.Open();
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;
        string query = "SELECT jogo_id, jogo_nome, jogo_logo FROM JOGOS ORDER BY jogo_nome ASC";
        cmnd_read.CommandText = query;
        reader = cmnd_read.ExecuteReader();

        while (reader.Read())
        {
            string[] linha = new string[3];
            for (int i = 0; i < 3; i++)
            {
                linha[i] = reader[i].ToString();
            }

            ComboBoxController.games.Add(linha);
        }

        dbcon.Close();
        SetText(ComboBoxController.games[0][1]);
        ComboBoxController.id = int.Parse(ComboBoxController.games[0][0]);
        Table.PorPontos();
    }

    void Update()
    {
        if (EndInstatiate)
        {
            SetText(ComboBoxController.nome);
            Table.jogo_id = ComboBoxController.id;
            Table.PorPontos();
            EndInstatiate = false;
        }
    }

    public void Click()
    {
        GameObject g = Instantiate(Combobox);
        g.transform.parent = gameObject.GetComponentInParent<Canvas>().gameObject.transform;
        g.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        g.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }

    public void SetText(string text)
    {
        GetComponent<UnityEngine.UI.Text>().text = text;
    }
}
