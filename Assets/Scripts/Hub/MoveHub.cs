using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHub : MonoBehaviour
{
    public float base_x, base_y;
    public int id = 0;
    public int actual_position = 0; // 0 = left, -1 = leftout, 1 = centerleft, 2 = centerright, 3 = right, 4 = rightout
    float velocity = 0.1f;
    public UnityEngine.UI.Image base_obj, pic;
    public UnityEngine.UI.Text txt;
    public int num_objects = 1;
    public float time_anim = 0.8f;
    public string exe = "", path_pic = "", path_bg = "";

    public void MoveRight()
    {
        actual_position++;

        if (actual_position > num_objects - 2)
        {
            actual_position = -1;
            InitPosition(actual_position);
        }

        switch (actual_position)
        {
            case 0:
                InitPosition(-1);
                StartCoroutine(SetLeft());
                break;
            case 1:
                StartCoroutine(SetCenterLeft());
                break;
            case 2:
                StartCoroutine(SetCenterRight());
                break;
            case 3:
                StartCoroutine(SetRight());
                break;
            case 4:
                StartCoroutine(SetRightOut());
                break;
            case -1:
                StartCoroutine(SetLeftOut());
                break;
            default:
                ControlHubObjects.moving--;
                break;
        }
    }

    public void MoveLeft()
    {
        actual_position--;

        if (actual_position < -1)
        {
            actual_position = num_objects - 2;
            InitPosition(actual_position);
        }

        switch (actual_position)
        {
            case 0:
                StartCoroutine(SetLeft());
                break;
            case 1:
                StartCoroutine(SetCenterLeft());
                break;
            case 2:
                StartCoroutine(SetCenterRight());
                break;
            case 3:
                InitPosition(4);
                StartCoroutine(SetRight());
                break;
            case 4:
                StartCoroutine(SetRightOut());
                break;
            case -1:
                StartCoroutine(SetLeftOut());
                break;
            default:
                ControlHubObjects.moving--;
                break;
        }
    }


    public void InitPosition(int pos)
    {
        transform.localScale = new Vector3(1, 1, 1);

        switch (pos)
        {
            case 0:
                transform.localPosition = new Vector3(base_x, base_y, 0);
                base_obj.color = new Color(232f / 255, 232f / 255, 232f / 255);
                pic.color = Color.white;
                txt.color = new Color(64f / 255, 62f / 255, 86f / 255);
                break;
            default:
                if (pos <= 3 && pos >= 0)
                {
                    transform.localPosition = new Vector3(pos * (GetComponent<RectTransform>().sizeDelta.x + 25) + base_x, base_y, 0);
                    base_obj.color = new Color(1, 1, 1, 0.5f);
                    pic.color = new Color(1, 1, 1, 0.5f);
                    txt.color = new Color(202f / 255, 202f / 255, 202f / 255);
                }
                else
                {
                    if (pos < 0)
                        transform.localPosition = new Vector3(-2 * (GetComponent<RectTransform>().sizeDelta.x + 25) + base_x, base_y, 0);
                    else
                        transform.localPosition = new Vector3(4 * (GetComponent<RectTransform>().sizeDelta.x + 25) + base_x, base_y, 0);
                    
                    base_obj.color = new Color(1, 1, 1, 0.5f);
                    pic.color = new Color(1, 1, 1, 0.25f);
                    txt.color = new Color(232f / 255, 232f / 255, 232f / 255);
                }

                break;
        }
    }

    IEnumerator SetRightOut()
    {
        float time = 0;
        while (time < time_anim)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(4 * (GetComponent<RectTransform>().sizeDelta.x + 25) + base_x, base_y, 0), velocity).Round();
            base_obj.color = Color.Lerp(base_obj.color, new Color(1, 1, 1, 0.5f), velocity);
            pic.color = Color.Lerp(pic.color, new Color(1, 1, 1, 0.25f), velocity);
            txt.color = Color.Lerp(txt.color, new Color(232f / 255, 232f / 255, 232f / 255, 0.5f), velocity);
            yield return null;
            time += Time.deltaTime;
        }

        ControlHubObjects.moving--;
    }

    IEnumerator SetLeftOut()
    {
        float time = 0;
        while (time < time_anim)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(-1.5f * (GetComponent<RectTransform>().sizeDelta.x + 25) + base_x, base_y, 0), velocity).Round();
            base_obj.color = Color.Lerp(base_obj.color, new Color(1, 1, 1, 0.5f), velocity);
            pic.color = Color.Lerp(pic.color, new Color(1, 1, 1, 0.25f), velocity);
            txt.color = Color.Lerp(txt.color, new Color(232f / 255, 232f / 255, 232f / 255), velocity);
            yield return null;
            time += Time.deltaTime;
        }

        ControlHubObjects.moving--;
    }

    IEnumerator SetRight()
    {
        float time = 0;
        while (time < time_anim)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(3 * (GetComponent<RectTransform>().sizeDelta.x + 25) + base_x, base_y, 0), velocity).Round();
            base_obj.color = Color.Lerp(base_obj.color, Color.white, velocity);
            pic.color = Color.Lerp(pic.color, new Color(1, 1, 1, 0.5f), velocity);
            txt.color = Color.Lerp(txt.color, new Color(202f / 255, 202f / 255, 202f / 255), velocity);
            yield return null;
            time += Time.deltaTime;
        }

        ControlHubObjects.moving--;
    }

    IEnumerator SetCenterRight()
    {
        float time = 0;
        while (time < time_anim)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(2 * (GetComponent<RectTransform>().sizeDelta.x + 25) + base_x, base_y, 0), velocity).Round();
            base_obj.color = Color.Lerp(base_obj.color, Color.white, velocity);
            pic.color = Color.Lerp(pic.color, new Color(1, 1, 1, 0.5f), velocity);
            txt.color = Color.Lerp(txt.color, new Color(202f / 255, 202f / 255, 202f / 255), velocity);
            yield return null;
            time += Time.deltaTime;
        }

        ControlHubObjects.moving--;
    }

    IEnumerator SetCenterLeft()
    {
        float time = 0;
        while (time < time_anim)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(GetComponent<RectTransform>().sizeDelta.x + 25 + base_x, base_y, 0), velocity).Round();
            base_obj.color = Color.Lerp(base_obj.color, Color.white, velocity);
            pic.color = Color.Lerp(pic.color, new Color(1, 1, 1, 0.5f), velocity);
            txt.color = Color.Lerp(txt.color, new Color(202f / 255, 202f / 255, 202f / 255), velocity);
            yield return null;
            time += Time.deltaTime;
        }

        ControlHubObjects.moving--;
    }

    IEnumerator SetLeft()
    {
        float time = 0;
        while (time < time_anim)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(base_x, base_y, 0), velocity).Round();
            base_obj.color = Color.Lerp(base_obj.color, new Color(232f / 255, 232f / 255, 232f / 255), velocity);
            pic.color = Color.Lerp(pic.color, Color.white, velocity);
            txt.color = Color.Lerp(txt.color, new Color(64f / 255, 62f / 255, 86f / 255), velocity);
            yield return null;
            time += Time.deltaTime;
        }

        ControlHubObjects.moving--;
    }
}