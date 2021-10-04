using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDestroy : MonoBehaviour
{
    Button b;
    public GameObject parent, fade;

    void Start()
    {
        b = GetComponent<Button>();
        b.onClick.AddListener(DestroyObject);    
    }

    void DestroyObject()
    {
        Destroy(parent);
    }
}
