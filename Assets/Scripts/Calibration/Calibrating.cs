using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calibrating : MonoBehaviour
{
    public const float range = 0.2f, range_z = 4;
    public const float time = 2;
    float remaining, average_x = 0, average_y = 0, average_z = 0;
    public const float minWeight = 10;
    bool end_calibrating;

    // Start is called before the first frame update
    void Start()
    {
        end_calibrating = false;
        InitialMeasures();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!end_calibrating)
        {
            remaining -= Time.deltaTime;

            try
            {
                string[] data = Connection.dispositivo[0].GetData().Split('|');
                float x = float.Parse(data[0]);
                float y = float.Parse(data[1]);
                float z = float.Parse(data[2]);

                if (Mathf.Clamp(x, average_x - range, average_x + range) == x &&
                    Mathf.Clamp(y, average_y - range, average_y + range) == y &&
                    Mathf.Clamp(z, average_z - range_z, average_z + range_z) == z &&
                    z > minWeight)
                {
                    average_x += x;
                    average_y += y;
                    average_z += z;
                    average_x /= 2;
                    average_y /= 2;
                    average_z /= 2;
                }
                else
                {
                    InitialMeasures();
                }
            }
            catch (Exception ex)
            {
                Debug.Log("Erro ao adquirir os dados" + ex);
            }

            // Notificação
            if (remaining <= 0)
            {
                Connection.dispositivo[0].AverageMass = average_z;
                Connection.dispositivo[0].AverageX = average_x;
                Connection.dispositivo[0].AverageY = average_y;
                transform.parent.GetComponentInChildren<Fade>().transform.SetAsLastSibling();
                transform.parent.GetComponentInChildren<Fade>().GetComponent<Fade>().FadeIn(true, "Login");
                end_calibrating = true;
            }
        }
    }

    void InitialMeasures()
    {
        try
        {
            string[] data = Connection.dispositivo[0].GetData().Split('|');
            average_x = float.Parse(data[0]);
            average_y = float.Parse(data[1]);
            average_z = float.Parse(data[2]);
        }
        catch (Exception ex)
        {
            Debug.Log("Erro ao adquirir os dados");
        }

        remaining = time;
    }
}