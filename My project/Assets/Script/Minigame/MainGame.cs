using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainGame : MonoBehaviour
{
    [Header("Настройки")]
    public float starDistance = 100f;
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;
    public Color connectedColor = Color.cyan;
    public Color wrongColor = Color.red; // Цвет для неправильных линий

    [Header("Префабы")]
    public GameObject starPrefab;
    public GameObject linePrefab;
    public Transform starsContainer;
    public Transform linesContainer;

    [System.Serializable]
    public class Constellation
    {
        public string name;
        public Vector2[] starPositions;
        public int[] connections; // ТОЛЬКО правильные соединения
    }

    [Header("Созвездия")]
    public Constellation[] constellations;
    private int currentIndex = 0;

    private List<Star> stars = new List<Star>();
    private List<Line> lines = new List<Line>();
    private Star selectedStar;
    private bool isDragging = false;
    private Line dragLine;
    private List<int[]> correctConnections = new List<int[]>(); // Список правильных соединений

    void Start()
    {
        LoadConstellation(currentIndex);

        // Создаем линию для перетаскивания
        GameObject dragLineObj = Instantiate(linePrefab, linesContainer);
        dragLineObj.name = "DragLine";
        dragLine = dragLineObj.GetComponent<Line>();
        dragLine.SetColor(selectedColor);
        dragLine.Hide();
    }

    void Update()
    {
        // Начало перетаскивания
        if (Input.GetMouseButtonDown(0))
        {
            Star clickedStar = FindStarAtMouse();
            if (clickedStar != null)
            {
                selectedStar = clickedStar;
                selectedStar.SetColor(selectedColor);
                isDragging = true;
                dragLine.Show();
            }
        }

        // Перетаскивание
        if (isDragging && selectedStar != null)
        {
            Vector3 mousePos = Input.mousePosition;
            dragLine.Draw(selectedStar.transform.position, mousePos);

            // Подсветка ближайшей звезды
            Star closest = FindClosestStar(mousePos, selectedStar);
            foreach (Star star in stars)
            {
                if (star == closest && star != selectedStar)
                {
                    // Проверяем, правильное ли это соединение
                    if (IsCorrectConnection(selectedStar.id, star.id))
                        star.SetColor(selectedColor);
                    else
                        star.SetColor(wrongColor);
                }
                else if (star != selectedStar && !star.isConnected)
                    star.SetColor(normalColor);
            }
        }

        // Конец перетаскивания
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            if (selectedStar != null)
            {
                Vector3 mousePos = Input.mousePosition;
                Star targetStar = FindClosestStar(mousePos, selectedStar);

                if (targetStar != null && targetStar != selectedStar)
                {
                    // Проверяем, можно ли соединить эти звезды
                    if (IsCorrectConnection(selectedStar.id, targetStar.id))
                    {
                        ConnectStars(selectedStar, targetStar);
                    }
                    else
                    {
                        // Неправильное соединение - показываем ошибку
                        ShowWrongConnection(selectedStar, targetStar);
                    }
                }

                // Сброс цветов
                foreach (Star star in stars)
                {
                    if (!star.isConnected)
                        star.SetColor(normalColor);
                }
            }

            isDragging = false;
            dragLine.Hide();
            CheckWin();
        }
    }

    void LoadConstellation(int index)
    {
        // Очистка
        foreach (Star star in stars) Destroy(star.gameObject);
        foreach (Line line in lines) Destroy(line.gameObject);
        stars.Clear();
        lines.Clear();
        correctConnections.Clear();

        Constellation constellation = constellations[index];

        // Заполняем список правильных соединений
        for (int i = 0; i < constellation.connections.Length; i += 2)
        {
            int[] connection = new int[2];
            connection[0] = constellation.connections[i];
            connection[1] = constellation.connections[i + 1];
            correctConnections.Add(connection);
        }

        // Создаем звезды
        for (int i = 0; i < constellation.starPositions.Length; i++)
        {
            Vector2 pos = constellation.starPositions[i];
            Vector3 screenPos = new Vector3(
                pos.x * Screen.width,
                pos.y * Screen.height,
                0
            );

            GameObject starObj = Instantiate(starPrefab, starsContainer);
            starObj.transform.position = screenPos;

            Star star = starObj.GetComponent<Star>();
            star.id = i;
            stars.Add(star);
        }
    }

    bool IsCorrectConnection(int starId1, int starId2)
    {
        // Проверяем, есть ли такое соединение в списке правильных
        foreach (int[] connection in correctConnections)
        {
            if ((connection[0] == starId1 && connection[1] == starId2) ||
                (connection[0] == starId2 && connection[1] == starId1))
            {
                return true;
            }
        }
        return false;
    }

    bool IsConnectionMade(int starId1, int starId2)
    {
        // Проверяем, уже ли соединены эти звезды
        foreach (Line line in lines)
        {
            if ((line.star1.id == starId1 && line.star2.id == starId2) ||
                (line.star1.id == starId2 && line.star2.id == starId1))
            {
                return true;
            }
        }
        return false;
    }

    Star FindStarAtMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        foreach (Star star in stars)
        {
            if (Vector3.Distance(mousePos, star.transform.position) < starDistance)
                return star;
        }
        return null;
    }

    Star FindClosestStar(Vector3 position, Star exclude)
    {
        Star closest = null;
        float minDistance = starDistance;

        foreach (Star star in stars)
        {
            if (star == exclude) continue;

            float distance = Vector3.Distance(position, star.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = star;
            }
        }
        return closest;
    }

    void ConnectStars(Star star1, Star star2)
    {
        // Проверка, не соединены ли уже
        if (IsConnectionMade(star1.id, star2.id))
            return;

        // Создание линии
        GameObject lineObj = Instantiate(linePrefab, linesContainer);
        Line line = lineObj.GetComponent<Line>();
        line.Draw(star1.transform.position, star2.transform.position);
        line.star1 = star1;
        line.star2 = star2;
        line.SetColor(connectedColor);
        lines.Add(line);

        // Обновление звезд
        star1.Connect();
        star2.Connect();
        star1.SetColor(connectedColor);
        star2.SetColor(connectedColor);
    }

    void ShowWrongConnection(Star star1, Star star2)
    {
        // Временная красная линия (исчезает через секунду)
        GameObject lineObj = Instantiate(linePrefab, linesContainer);
        Line line = lineObj.GetComponent<Line>();
        line.Draw(star1.transform.position, star2.transform.position);
        line.SetColor(wrongColor);

        // Удаляем через секунду
        Destroy(lineObj, 1f);

        // Мигаем звездами красным
        StartCoroutine(FlashStarsRed(star1, star2));
    }

    System.Collections.IEnumerator FlashStarsRed(Star star1, Star star2)
    {
        Color original1 = star1.GetCurrentColor();
        Color original2 = star2.GetCurrentColor();

        star1.SetColor(wrongColor);
        star2.SetColor(wrongColor);

        yield return new WaitForSeconds(0.5f);

        if (star1.isConnected)
            star1.SetColor(connectedColor);
        else
            star1.SetColor(normalColor);

        if (star2.isConnected)
            star2.SetColor(connectedColor);
        else
            star2.SetColor(normalColor);
    }

    void CheckWin()
    {
        // Все ли правильные соединения сделаны?
        bool allCorrect = true;

        foreach (int[] connection in correctConnections)
        {
            if (!IsConnectionMade(connection[0], connection[1]))
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect)
        {
            Debug.Log("Созвездие собрано правильно! " + constellations[currentIndex].name);

            // Блокируем дальнейшие соединения
            foreach (Star star in stars)
            {
                star.isConnected = true;
            }
        }
    }

    public void NextLevel()
    {
        currentIndex = (currentIndex + 1) % constellations.Length;
        LoadConstellation(currentIndex);
    }

    public void Restart()
    {
        LoadConstellation(currentIndex);
    }
}