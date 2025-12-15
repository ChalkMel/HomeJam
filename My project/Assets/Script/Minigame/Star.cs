using UnityEngine;
using UnityEngine.UI;

public class Star : MonoBehaviour
{
    public int id;
    public bool isConnected = false;
    private Image image;
    private Color currentColor;
    private RectTransform rectTransform;

    void Start()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        currentColor = image.color;
    }

    public void SetColor(Color color)
    {
        if (image != null)
        {
            image.color = color;
            currentColor = color;
        }
    }

    public Color GetCurrentColor()
    {
        return currentColor;
    }

    public void Connect()
    {
        isConnected = true;
    }

    public void Reset()
    {
        isConnected = false;
        SetColor(Color.white);
    }

    // Получаем позицию звезды на экране
    public Vector3 GetPosition()
    {
        if (rectTransform != null)
        {
            return rectTransform.position;
        }
        return transform.position;
    }
}