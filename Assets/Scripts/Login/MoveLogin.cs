using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLogin : MonoBehaviour
{
    public int actual_position = 0; // 0 = center, 1 = right, -1 = left, 2 = rightout, -2 = leftout
    float velocity = 0.08f, deltaTime = 0.01f;
    public UnityEngine.UI.Image base_obj, pic;
    public UnityEngine.UI.Text txt;
    public int num_objects = 1;
    public float time_anim = 0.8f;
    public int id_usuario = 0;

    public void MoveRight()
    {
        int left = (num_objects - 1) / 2;
        int right = (num_objects - 1) / 2 + (num_objects - 1) % 2;
        actual_position++;

        if (actual_position > right)
        {
            actual_position = -left;
            InitPosition(actual_position);
        }

        switch (actual_position)
        {
            case 0:
                StartCoroutine(SetCenter());
                break;
            case 1:
                StartCoroutine(SetRight());
                break;
            case -1:
                InitPosition(-2);
                StartCoroutine(SetLeft());
                break;
            case 2:
                StartCoroutine(SetRightOut());
                break;
            default:
                ControlObjects.moving--;
                break;
        }
    }

    public void MoveLeft()
    {
        int left = (num_objects - 1) / 2;
        int right = (num_objects - 1) / 2 + (num_objects - 1) % 2;
        actual_position--;

        if (actual_position < -left)
        {
            actual_position = right;
            InitPosition(actual_position);
        }

        switch (actual_position)
        {
            case 0:
                StartCoroutine(SetCenter());
                break;
            case 1:
                InitPosition(2);
                StartCoroutine(SetRight());
                break;
            case -1:
                StartCoroutine(SetLeft());
                break;
            case -2:
                StartCoroutine(SetLeftOut());
                break;
            default:
                ControlObjects.moving--;
                break;
        }
    }


    public void InitPosition(int pos)
    {
        switch (pos)
        {
            case 0:
                transform.localPosition = new Vector3(0, 0, 0);
                transform.localScale = new Vector3(1, 1, 1);
                base_obj.color = new Color(232f / 255, 232f / 255, 232f / 255);
                pic.color = Color.white;
                txt.color = new Color(64f / 255, 62f / 255, 86f / 255);
                break;
            case 1:
                transform.localPosition = new Vector3(400, 0, 0);
                transform.localScale = new Vector3(0.75f, 0.75f, 1);
                base_obj.color = Color.white;
                pic.color = new Color(1, 1, 1, 0.5f);
                txt.color = new Color(202f / 255, 202f / 255, 202f / 255);
                break;
            case -1:
                transform.localPosition = new Vector3(-400, 0, 0);
                transform.localScale = new Vector3(0.75f, 0.75f, 1);
                base_obj.color = Color.white;
                pic.color = new Color(1, 1, 1, 0.5f);
                txt.color = new Color(202f / 255, 202f / 255, 202f / 255);
                break;
            default:
                if (pos >= 2)
                {
                    transform.localPosition = new Vector3(800, 0, 0);
                    transform.localScale = new Vector3(0.5f, 0.5f, 1);
                    base_obj.color = new Color(1, 1, 1, 0.5f);
                    pic.color = new Color(1, 1, 1, 0.25f);
                    txt.color = new Color(232f / 255, 232f / 255, 232f / 255);
                }
                else
                {
                    transform.localPosition = new Vector3(-800, 0, 0);
                    transform.localScale = new Vector3(0.5f, 0.5f, 1);
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
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(800, 0, 0), velocity).Round();
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.5f, 0.5f, 1), velocity).Round();
            base_obj.color = Color.Lerp(base_obj.color, new Color(1, 1, 1, 0.5f), velocity);
            pic.color = Color.Lerp(pic.color, new Color(1, 1, 1, 0.25f), velocity);
            txt.color = Color.Lerp(txt.color, new Color(232f / 255, 232f / 255, 232f / 255, 0.5f), velocity);
            yield return null;
            time += Time.deltaTime;
        }

        ControlObjects.moving--;
    }

    IEnumerator SetLeftOut()
    {
        float time = 0;
        while (time < time_anim)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(-800, 0, 0), velocity).Round();
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.5f, 0.5f, 1), velocity).Round();
            base_obj.color = Color.Lerp(base_obj.color, new Color(1, 1, 1, 0.5f), velocity);
            pic.color = Color.Lerp(pic.color, new Color(1, 1, 1, 0.25f), velocity);
            txt.color = Color.Lerp(txt.color, new Color(232f / 255, 232f / 255, 232f / 255), velocity);
            yield return null;
            time += Time.deltaTime;
        }

        ControlObjects.moving--;
    }

    IEnumerator SetRight()
    {
        float time = 0;
        while (time < time_anim)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(400, 0, 0), velocity).Round();
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.75f, 0.75f, 1), velocity).Round();
            base_obj.color = Color.Lerp(base_obj.color, Color.white, velocity);
            pic.color = Color.Lerp(pic.color, new Color(1, 1, 1, 0.5f), velocity);
            txt.color = Color.Lerp(txt.color, new Color(202f / 255, 202f / 255, 202f / 255), velocity);
            yield return null;
            time += Time.deltaTime;
        }

        ControlObjects.moving--;
    }

    IEnumerator SetCenter()
    {
        float time = 0;
        while (time < time_anim)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, 0, 0), velocity).Round();
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 1, 1), velocity).Round();
            base_obj.color = Color.Lerp(base_obj.color, new Color(232f / 255, 232f / 255, 232f / 255), velocity);
            pic.color = Color.Lerp(pic.color, Color.white, velocity);
            txt.color = Color.Lerp(txt.color, new Color(64f / 255, 62f / 255, 86f / 255), velocity);
            yield return null;
            time += Time.deltaTime;
        }

        ControlObjects.moving--;
    }

    IEnumerator SetLeft()
    {
        float time = 0;
        while (time < time_anim)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(-400, 0, 0), velocity).Round();
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.75f, 0.75f, 1), velocity).Round();
            base_obj.color = Color.Lerp(base_obj.color, Color.white, velocity);
            pic.color = Color.Lerp(pic.color, new Color(1, 1, 1, 0.5f), velocity);
            txt.color = Color.Lerp(txt.color, new Color(202f/255, 202f/255, 202f/255), velocity);
            yield return null;
            time += Time.deltaTime;
        }

        ControlObjects.moving--;
    }
}

static class ExtensionMethods
{
    /// <summary>
    /// Rounds Vector3.
    /// </summary>
    /// <param name="vector3"></param>
    /// <param name="decimalPlaces"></param>
    /// <returns></returns>
    public static Vector3 Round(this Vector3 vector3, int decimalPlaces = 2)
    {
        float multiplier = 1;
        for (int i = 0; i < decimalPlaces; i++)
        {
            multiplier *= 10f;
        }
        return new Vector3(
            Mathf.Round(vector3.x * multiplier) / multiplier,
            Mathf.Round(vector3.y * multiplier) / multiplier,
            Mathf.Round(vector3.z * multiplier) / multiplier);
    }
}