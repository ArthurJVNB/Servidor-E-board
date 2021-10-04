using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LineTable : MonoBehaviour
{
    public static int selected = 0;
    public int id;
    public Text nome, pontuacao, altura, peso;
    public Image img, base_img;
    public bool activated;

    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        selected = id;
    }

    void Update()
    {
        if (id == selected && activated == false)
        {
            img.color = new Color(1, 1, 1);
            base_img.color = new Color(232 / 255f, 232 / 255f, 232 / 255f);
            nome.color = new Color(63 / 255f, 61 / 255f, 86 / 255f);
            pontuacao.color = new Color(63 / 255f, 61 / 255f, 86 / 255f);
            altura.color = new Color(63 / 255f, 61 / 255f, 86 / 255f);
            peso.color = new Color(63 / 255f, 61 / 255f, 86 / 255f);
            activated = true;
        }
        else if (id != selected && activated == true)
        {
            img.color = new Color(1, 1, 1, 0.5f);
            base_img.color = new Color(1, 1, 1);
            nome.color = new Color(172 / 255f, 172 / 255f, 172 / 255f);
            pontuacao.color = new Color(172 / 255f, 172 / 255f, 172 / 255f);
            altura.color = new Color(172 / 255f, 172 / 255f, 172 / 255f);
            peso.color = new Color(172 / 255f, 172 / 255f, 172 / 255f);
            activated = false;
        }
    }

    public void ChangeSelected(int new_state)
    {
        selected = new_state;
    }
}
