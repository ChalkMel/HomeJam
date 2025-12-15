using UnityEngine;
using UnityEngine.UI;

public class Line : MonoBehaviour
{
    public Star star1;
    public Star star2;
    private Image image;
    private RectTransform rectTransform;

    void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        // Убедимся, что это UI элемент
        if (rectTransform == null)
        {
            gameObject.AddComponent<RectTransform>();
            rectTransform = GetComponent<RectTransform>();
        }
    }

    public void Draw(Vector3 from, Vector3 to)
    {
        // Позиция - середина между точками
        Vector3 middle = (from + to) / 2;
        transform.position = middle;

        // Длина и поворот
        Vector3 direction = to - from;
        float distance = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Устанавливаем размер - distance это длина, 5 это толщина
        rectTransform.sizeDelta = new Vector2(distance, 5f);
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetColor(Color color)
    {
        if (image != null)
            image.color = color;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}