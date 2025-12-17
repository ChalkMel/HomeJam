using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image), typeof(Animator))]
public class Star : MonoBehaviour
{
    public int id;
    public bool isConnected = false;
    public bool isNoiseStar = false;
    private Image image;
    private Color currentColor;
    private Color originalColor;

    private Image starImage;

    private void Awake()
    {
        image = GetComponent<Image>();
        currentColor = image.color;
        originalColor = currentColor;
        starImage = GetComponent<Image>();
    }
    public void SetSprite(Sprite sprite)
    {
        starImage.sprite = sprite;
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