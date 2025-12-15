using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainGame : MonoBehaviour
{
    [Header("Настройки")]
    public float starDistance = 100f;
    public float minDistanceBetweenStars = 150f;
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;
    public Color connectedColor = Color.cyan;
    public Color wrongColor = Color.red;
    public Color noiseStarColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    public Color noiseStarSelectedColor = new Color(1f, 0.6f, 0.6f, 1f);
    public Color hintColor = new Color(0.2f, 1f, 0.2f, 1f);

    [Header("Префабы")]
    public GameObject starPrefab;
    public GameObject noiseStarPrefab;
    public GameObject linePrefab;
    public Transform starsContainer;
    public Transform noiseStarsContainer;
    public Transform linesContainer;

    [Header("Помехи")]
    public int minNoiseStars = 3;
    public int maxNoiseStars = 8;

    [Header("UI")]
    public Button hintButton;

    [System.Serializable]
    public class Constellation
    {
        public string name;
        public Vector2[] starPositions;
        public int[] connections;
    }

    [Header("Созвездия")]
    public Constellation[] constellations;
    private int currentIndex = 0;

    private List<Star> stars = new List<Star>();
    private List<Star> noiseStars = new List<Star>();
    private List<Line> lines = new List<Line>();
    private Star selectedStar;
    private bool isDragging = false;
    private Line dragLine;
    private List<int[]> correctConnections = new List<int[]>();
    private bool isHintActive = false;

    void Start()
    {
        LoadConstellation(currentIndex);

        GameObject dragLineObj = Instantiate(linePrefab, linesContainer);
        dragLineObj.name = "DragLine";
        dragLine = dragLineObj.GetComponent<Line>();
        dragLine.SetColor(selectedColor);
        dragLine.Hide();

        if (hintButton != null)
        {
            hintButton.onClick.AddListener(ShowHint);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Star clickedStar = FindStarAtMouse();
            if (clickedStar != null)
            {
                selectedStar = clickedStar;
                selectedStar.SaveOriginalColor();

                if (selectedStar.isNoiseStar)
                    selectedStar.SetColor(noiseStarSelectedColor);
                else
                    selectedStar.SetColor(selectedColor);

                isDragging = true;
                dragLine.Show();
            }
        }

        if (isDragging && selectedStar != null)
        {
            Vector3 mousePos = Input.mousePosition;
            dragLine.Draw(selectedStar.transform.position, mousePos);

            // ТОЛЬКО подсветка выбранной звезды, остальные в обычном цвете
            // НЕ подсвечиваем возможные соединения заранее!
            ResetAllStarsExceptSelected();
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            if (selectedStar != null)
            {
                Vector3 mousePos = Input.mousePosition;
                Star targetStar = FindClosestStar(mousePos, selectedStar);

                if (targetStar != null && targetStar != selectedStar)
                {
                    // ТОЛЬКО ЗДЕСЬ проверяем правильность соединения
                    if (selectedStar.isNoiseStar || targetStar.isNoiseStar)
                    {
                        // Попытка соединить с помехой
                        ShowWrongConnection(selectedStar, targetStar, true);
                    }
                    else if (IsCorrectConnection(selectedStar.id, targetStar.id))
                    {
                        // Правильное соединение
                        ConnectStars(selectedStar, targetStar);
                    }
                    else
                    {
                        // Неправильное соединение
                        ShowWrongConnection(selectedStar, targetStar, false);
                    }
                }
                else
                {
                    // Не попали ни в одну звезду - просто сбрасываем
                    ResetAllStars();
                }
            }

            isDragging = false;
            dragLine.Hide();
            CheckWin();
        }
    }

    // ПОДСКАЗКА
    public void ShowHint()
    {
        // Помехи становятся полупрозрачными
        foreach (Star noiseStar in noiseStars)
        {
            noiseStar.SetColor(new Color(0.5f, 0.5f, 0.5f, 0.3f));
        }

        // Звезды без неподключенных правильных соединений - полупрозрачные
        foreach (Star star in stars)
        {
            if (!star.isConnected)
            {
                bool hasCorrectConnections = false;
                foreach (int[] connection in correctConnections)
                {
                    if (connection[0] == star.id || connection[1] == star.id)
                    {
                        if (!IsConnectionMade(connection[0], connection[1]))
                        {
                            hasCorrectConnections = true;
                            break;
                        }
                    }
                }

                if (!hasCorrectConnections)
                {
                    star.SetColor(new Color(1f, 1f, 1f, 0.3f));
                }
            }
        }

        // Правильные звезды мигают зеленым
        StartCoroutine(HighlightCorrectStars());

        Invoke("ResetAfterHint", 3f);
    }

    System.Collections.IEnumerator HighlightCorrectStars()
    {
        List<Star> starsToHighlight = new List<Star>();

        foreach (int[] connection in correctConnections)
        {
            if (!IsConnectionMade(connection[0], connection[1]))
            {
                Star star1 = stars.Find(s => s.id == connection[0]);
                Star star2 = stars.Find(s => s.id == connection[1]);

                if (star1 != null && !starsToHighlight.Contains(star1))
                    starsToHighlight.Add(star1);
                if (star2 != null && !starsToHighlight.Contains(star2))
                    starsToHighlight.Add(star2);
            }
        }

        // Анимация мигания
        for (int i = 0; i < 6; i++)
        {
            foreach (Star star in starsToHighlight)
            {
                if (star != null)
                {
                    if (i % 2 == 0)
                        star.SetColor(hintColor);
                    else
                        star.SetColor(normalColor);
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    void ResetAfterHint()
    {
        isHintActive = false;
        ResetAllStars();
    }

    void ResetAllStarsExceptSelected()
    {
        // Восстанавливаем цвета всех звезд, кроме выбранной
        foreach (Star star in stars)
        {
            if (star != selectedStar)
            {
                if (star.isConnected)
                    star.SetColor(connectedColor);
                else
                    star.SetColor(normalColor);
            }
        }

        foreach (Star noiseStar in noiseStars)
        {
            if (noiseStar != selectedStar)
            {
                noiseStar.ResetToOriginalColor();
            }
        }
    }

    void ResetAllStars()
    {
        foreach (Star star in stars)
        {
            if (star.isConnected)
                star.SetColor(connectedColor);
            else
                star.SetColor(normalColor);
        }

        foreach (Star noiseStar in noiseStars)
        {
            noiseStar.ResetToOriginalColor();
        }
    }

    void LoadConstellation(int index)
    {
        ClearAll();

        Constellation constellation = constellations[index];

        for (int i = 0; i < constellation.connections.Length; i += 2)
        {
            int[] connection = new int[2];
            connection[0] = constellation.connections[i];
            connection[1] = constellation.connections[i + 1];
            correctConnections.Add(connection);
        }

        // Основные звезды
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
            star.isNoiseStar = false;
            star.SetOriginalColor(normalColor);
            stars.Add(star);
        }

        // Помехи
        CreateNoiseStars();

        if (hintButton != null)
            hintButton.interactable = true;
    }

    void CreateNoiseStars()
    {
        int noiseCount = Random.Range(minNoiseStars, maxNoiseStars + 1);

        for (int i = 0; i < noiseCount; i++)
        {
            Vector2 randomPos = GetRandomStarPosition();
            GameObject noiseObj = Instantiate(noiseStarPrefab, noiseStarsContainer);
            noiseObj.transform.position = new Vector3(randomPos.x, randomPos.y, 0);

            Star noiseStar = noiseObj.GetComponent<Star>();
            noiseStar.id = -100 - i;
            noiseStar.isNoiseStar = true;
            noiseStar.SetOriginalColor(noiseStarColor);
            noiseStar.SetColor(noiseStarColor);
            noiseStars.Add(noiseStar);
        }
    }

    Vector2 GetRandomStarPosition()
    {
        int attempts = 0;
        int maxAttempts = 100;

        while (attempts < maxAttempts)
        {
            float padding = 0.1f;
            float x = Random.Range(padding, 1f - padding) * Screen.width;
            float y = Random.Range(padding, 1f - padding) * Screen.height;
            Vector2 candidatePos = new Vector2(x, y);

            bool tooClose = false;

            foreach (Star star in stars)
            {
                float distance = Vector2.Distance(candidatePos, star.transform.position);
                if (distance < minDistanceBetweenStars)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                foreach (Star noiseStar in noiseStars)
                {
                    float distance = Vector2.Distance(candidatePos, noiseStar.transform.position);
                    if (distance < minDistanceBetweenStars)
                    {
                        tooClose = true;
                        break;
                    }
                }
            }

            if (!tooClose) return candidatePos;
            attempts++;
        }

        return new Vector2(
            Random.Range(0.2f, 0.8f) * Screen.width,
            Random.Range(0.2f, 0.8f) * Screen.height
        );
    }

    void ClearAll()
    {
        foreach (Star star in stars) Destroy(star.gameObject);
        foreach (Star noiseStar in noiseStars) Destroy(noiseStar.gameObject);
        foreach (Line line in lines) Destroy(line.gameObject);

        stars.Clear();
        noiseStars.Clear();
        lines.Clear();
        correctConnections.Clear();
    }

    Star FindStarAtMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        float closestDistance = starDistance;
        Star closestStar = null;

        foreach (Star star in stars)
        {
            float distance = Vector3.Distance(mousePos, star.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestStar = star;
            }
        }

        foreach (Star noiseStar in noiseStars)
        {
            float distance = Vector3.Distance(mousePos, noiseStar.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestStar = noiseStar;
            }
        }

        return closestStar;
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

        foreach (Star noiseStar in noiseStars)
        {
            if (noiseStar == exclude) continue;

            float distance = Vector3.Distance(position, noiseStar.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = noiseStar;
            }
        }

        return closest;
    }

    bool IsCorrectConnection(int starId1, int starId2)
    {
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

    void ConnectStars(Star star1, Star star2)
    {
        if (IsConnectionMade(star1.id, star2.id))
            return;

        GameObject lineObj = Instantiate(linePrefab, linesContainer);
        Line line = lineObj.GetComponent<Line>();
        line.Draw(star1.transform.position, star2.transform.position);
        line.star1 = star1;
        line.star2 = star2;
        line.SetColor(connectedColor);
        lines.Add(line);

        star1.Connect();
        star2.Connect();
        star1.SetColor(connectedColor);
        star2.SetColor(connectedColor);
    }

    void ShowWrongConnection(Star star1, Star star2, bool isNoise)
    {
        // Создаем временную красную линию
        GameObject lineObj = Instantiate(linePrefab, linesContainer);
        Line line = lineObj.GetComponent<Line>();
        line.Draw(star1.transform.position, star2.transform.position);

        if (isNoise)
            line.SetColor(new Color(1f, 0.5f, 0.5f)); // Светло-красный для помех
        else
            line.SetColor(wrongColor); // Ярко-красный

        Destroy(lineObj, 1f);

        // Мигаем звездами красным
        StartCoroutine(FlashStars(star1, star2, isNoise));
    }

    System.Collections.IEnumerator FlashStars(Star star1, Star star2, bool isNoise)
    {
        Color flashColor = isNoise ? new Color(1f, 0.5f, 0.5f) : wrongColor;

        star1.SetColor(flashColor);
        star2.SetColor(flashColor);

        yield return new WaitForSeconds(0.5f);

        // Возвращаем нормальные цвета
        if (star1.isNoiseStar)
            star1.ResetToOriginalColor();
        else if (star1.isConnected)
            star1.SetColor(connectedColor);
        else
            star1.SetColor(normalColor);

        if (star2.isNoiseStar)
            star2.ResetToOriginalColor();
        else if (star2.isConnected)
            star2.SetColor(connectedColor);
        else
            star2.SetColor(normalColor);
    }

    void CheckWin()
    {
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
            Debug.Log("🎉 Созвездие собрано!");

            StartCoroutine(NoiseStarsVictoryFlash());

            foreach (Star star in stars)
            {
                star.isConnected = true;
            }

            if (hintButton != null)
                hintButton.interactable = false;
        }
    }

    System.Collections.IEnumerator NoiseStarsVictoryFlash()
    {
        for (int i = 0; i < 3; i++)
        {
            foreach (Star noiseStar in noiseStars)
            {
                noiseStar.SetColor(Color.yellow);
            }
            yield return new WaitForSeconds(0.2f);

            foreach (Star noiseStar in noiseStars)
            {
                noiseStar.ResetToOriginalColor();
            }
            yield return new WaitForSeconds(0.2f);
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