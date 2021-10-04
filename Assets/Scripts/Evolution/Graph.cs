using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour
{
    public Sprite circleSprite;
    private RectTransform graphContainer;

    void Start()
    {
        graphContainer = GetComponent<RectTransform>();

        List<float> valueList = new List<float> { 10f, 12f, 15f, 21f, 30f, 60f, 93f };
        ShowGraph(valueList);
    }

    private GameObject CreateCircle (Vector3 anchoredPosition)
    {
        GameObject g = new GameObject("circle", typeof(Image));
        g.transform.SetParent(graphContainer, false);
        g.GetComponent<Image>().sprite = circleSprite;
        g.GetComponent<Image>().color = new Color(63 / 255f, 61 / 255f, 86 / 255f);
        RectTransform rectTransform = g.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        return g;
    }

    private void ShowGraph (List<float> valueList)
    {
        float graphHeight = graphContainer.sizeDelta.y;
        float yMaximum = valueList[0];
        float yMinimum = valueList[0];
        float xSize = 50f;

        foreach (float value in valueList)
        {
            if (value > yMaximum)
                yMaximum = value;

            if (value < yMinimum)
                yMinimum = value;
        }

        yMaximum += (yMaximum - yMinimum) * 0.2f;
        yMinimum -= (yMaximum - yMinimum) * 0.2f;

        GameObject previous = null;

        for (int i = 0; i < valueList.Count; i++)
        {
            float xPosition = xSize + i * xSize;
            float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;
            GameObject circleGameObject = CreateCircle(new Vector3(xPosition, yPosition));

            if (previous != null)
            {
                //GameObject line = new GameObject("dotConnection", typeof(LineRenderer));
                //line.GetComponent<LineRenderer>().startWidth = 2;
                //line.GetComponent<LineRenderer>().endWidth = 3;
                //line.GetComponent<LineRenderer>().positionCount = 2;
                //line.GetComponent<LineRenderer>().SetPosition(0, previous.transform.position);
                //line.GetComponent<LineRenderer>().SetPosition(1, circleGameObject.transform.position);
                CreateDotConnection(previous.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
            }

            previous = circleGameObject;
        }
    }

    private void CreateDotConnection (Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject g = new GameObject("dotConnection", typeof(Image));
        g.transform.SetParent(graphContainer, false);
        g.transform.SetParent(graphContainer, false);
        g.GetComponent<Image>().color = new Color(63 / 255f, 61 / 255f, 86 / 255f);
        RectTransform rectTransform = g.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance / 2;
        rectTransform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * 180 / Mathf.PI);
    }
}
