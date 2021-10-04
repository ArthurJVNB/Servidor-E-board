using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubPanelImages : MonoBehaviour
{
    public Sprite bt, usb, b100, b80, b60, b40, b20, b0;
    public static bool BT = false;
    public bool bateria = false;

    // Update is called once per frame
    void Start()
    {
        StartCoroutine(UpdateIcons());
    }

    IEnumerator UpdateIcons()
    {
        while (true)
        {
            if (bateria && Connection.dispositivo.Count > 0)
            {
                if (Connection.dispositivo[0].AverageBattery > 90)
                    GetComponent<UnityEngine.UI.Image>().sprite = b100;
                else if (Connection.dispositivo[0].AverageBattery > 70)
                    GetComponent<UnityEngine.UI.Image>().sprite = b80;
                else if (Connection.dispositivo[0].AverageBattery > 50)
                    GetComponent<UnityEngine.UI.Image>().sprite = b60;
                else if (Connection.dispositivo[0].AverageBattery > 30)
                    GetComponent<UnityEngine.UI.Image>().sprite = b40;
                else if (Connection.dispositivo[0].AverageBattery > 10)
                    GetComponent<UnityEngine.UI.Image>().sprite = b20;
                else
                    GetComponent<UnityEngine.UI.Image>().sprite = b0;
            }
            else
            {
                if (BT)
                    GetComponent<UnityEngine.UI.Image>().sprite = bt;
                else
                    GetComponent<UnityEngine.UI.Image>().sprite = usb;
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
