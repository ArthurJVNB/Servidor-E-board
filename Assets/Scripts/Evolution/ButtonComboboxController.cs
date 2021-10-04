using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonComboboxController : MonoBehaviour, IPointerEnterHandler, IPointerUpHandler, IPointerExitHandler
{
    Image img;
    public Image game;
    public Text texto_botao;
    public int id_jogo;

    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        game.color = new Color(1, 1, 1, 1f);
        img.color = new Color(232 / 255f, 232 / 255f, 232 / 255f, 0.5f);
        texto_botao.color = new Color(63 / 255f, 61 / 255f, 86 / 255f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        game.color = new Color(1, 1, 1, 0.5f);
        img.color = new Color(1, 1, 1);
        texto_botao.color = new Color(172 / 255f, 172 / 255f, 172 / 255f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Clique();
    }

    void Clique()
    {
        NameController.EndInstatiate = true;
        ComboBoxController.id = id_jogo;
        ComboBoxController.nome = texto_botao.text;
        Destroy(GetComponentInParent<ComboBoxController>().gameObject);
    }
}
