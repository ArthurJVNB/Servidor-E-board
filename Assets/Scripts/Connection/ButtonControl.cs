using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Image img;
    public Text texto_botao;
    public Sprite not_selected, selected;

    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        texto_botao.color = new Color(63 / 255f, 61 / 255f, 86 / 255f);
        img.sprite = selected;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        texto_botao.color = new Color(172 / 255f, 172 / 255f, 172 / 255f);
        img.sprite = not_selected;
    }
}
