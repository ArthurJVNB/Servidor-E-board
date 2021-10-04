using UnityEngine.UI;
using UnityEngine;

public class Languages : MonoBehaviour
{
    public string pt, es, en;

    void Start()
    {
        switch (Connection.idioma)
        {
            case "pt":
                GetComponent<Text>().text = pt;
                break;
            case "es":
                GetComponent<Text>().text = es;
                break;
            case "en":
                GetComponent<Text>().text = en;
                break;
            default:
                GetComponent<Text>().text = pt;
                break;
        }
    }
}
