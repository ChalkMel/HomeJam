using UnityEngine;
using UnityEngine.UI;

public class Star : MonoBehaviour
{
    public int id;
    public bool isConnected = false;
    public bool isNoiseStar = false;
    private Image image;
    private Color currentColor;
    private Color originalColor;

    void Start()
    {
        image = GetComponent<Image>();
        currentColor = image.color;
        originalColor = currentColor;
    }

    public void SetColor(Color color)
    {
        if (image != null)
        {
            image.color = color;
            currentColor = color;
        }
    }

    public void SetOriginalColor(Color color)
    {
        originalColor = color;
    }

    public void SaveOriginalColor()
    {
        originalColor = currentColor;
    }

    public void ResetToOriginalColor()
    {
        SetColor(originalColor);
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
        ResetToOriginalColor();
    }
}