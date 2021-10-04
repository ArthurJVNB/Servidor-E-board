using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectEvolutionController : MonoBehaviour
{
    public static int selected = 0;
    public int id;
    private Image img;
    private Text txt;
    public bool activated;
    
    void Start()
    {
        img = GetComponent<Image>();
        txt = GetComponentInChildren<Text>();
    }

    void Update()
    {
        if (id == selected && activated == false)
        {
            img.color = new Color(232/255f, 232 / 255f, 232 / 255f);
            txt.color = new Color(63 / 255f, 61 / 255f, 86 / 255f);
            activated = true;
        }
        else if (id != selected && activated == true)
        {
            img.color = new Color(1, 1, 1);
            txt.color = new Color(172 / 255f, 172 / 255f, 172 / 255f);
            activated = false;
        }
    }

    public void ChangeSelected(int new_state)
    {
        selected = new_state;
    }
}
