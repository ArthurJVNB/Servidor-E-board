using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ComboBoxController : MonoBehaviour
{
    public static List<string[]> games = new List<string[]>();
    int num_obj = 0;
    public static int id = 0;
    public static string nome = "";
    public GameObject buttonCombobox;

    void Start()
    {
        foreach (string[] game in games)
        {
            AddObject(int.Parse(game[0]), game[1], game[2]);
        }
    }

    private void AddObject(int id, string nome, string path)
    {
        num_obj++;
        float passo = (1280 - 326 * 0.6f * 5) / 8f;
        GameObject ge = Instantiate(buttonCombobox, transform, true);
        ge.GetComponentInChildren<ButtonComboboxController>().texto_botao.text = nome;
        ge.GetComponentInChildren<ButtonComboboxController>().id_jogo = id;
        byte[] data = File.ReadAllBytes(Directory.GetCurrentDirectory() + "\\Games\\" + path);
        Texture2D texture = new Texture2D(240, 240, TextureFormat.ARGB32, false);
        texture.LoadImage(data);
        texture.name = Path.GetFileNameWithoutExtension(Directory.GetCurrentDirectory() + "\\Games\\" + path);
        ge.GetComponentInChildren<ButtonComboboxController>().game.sprite = Sprite.Create(texture, new Rect(0, 0, 240, 240), new Vector2(0.5f, 0.5f));
        ge.transform.localScale = buttonCombobox.transform.localScale;
        if (num_obj > 5)
            ge.GetComponent<RectTransform>().anchoredPosition = new Vector2(((num_obj - 6) * (326f * 0.6f + passo)) + (-640 + 326 * 0.3f + 37.75f * 2), -150);
        else
            ge.GetComponent<RectTransform>().anchoredPosition = new Vector2(((num_obj - 1) * (326f * 0.6f + passo)) + (-640 + 326 * 0.3f + 37.75f * 2), 150);
    }
}
