using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainGame : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float starDistance = 100f;
    [SerializeField] private float minDistanceBetweenStars = 150f;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color connectedColor = Color.cyan;
    [SerializeField] private Color wrongColor = Color.red;
    [SerializeField] private Color noiseStarColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    [SerializeField] private Color hintColor = new Color(0.2f, 1f, 0.2f, 1f);
    [SerializeField] private string nextScene;

    [Header("Префабы")]
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private GameObject noiseStarPrefab;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private Transform starsContainer;
    [SerializeField] private Transform noiseStarsContainer;
    [SerializeField] private Transform linesContainer;

    [Header("Спрайты звезд")]
    [SerializeField] private Sprite[] starSprites; // Массив спрайтов для всех звезд

    [Header("Настройки анимации")]
    [SerializeField] private float pulseMinScale = 0.95f;
    [SerializeField] private float pulseMaxScale = 1.05f;
    [SerializeField] private float pulseDuration = 1f;
    [SerializeField] private float minPulseDelay = 2f;
    [SerializeField] private float maxPulseDelay = 5f;

    [Header("Помехи")]
    [SerializeField] private int minNoiseStars = 3;
    [SerializeField] private int maxNoiseStars = 8;

    [Header("UI")]
    [SerializeField] private Button hintButton;

    [System.Serializable]
    public class Constellation
    {
        public string name;
        public Vector2[] starPositions;
        public int[] connections;
    }

    [Header("Созвездия")]
    [SerializeField] private Constellation[] constellations;
    private int currentIndex = 0;

    private List<Star> stars = new List<Star>();
    private List<Star> noiseStars = new List<Star>();
    private List<Line> lines = new List<Line>();
    private Star selectedStar;
    private bool isDragging = false;
    private Line dragLine;
    private List<int[]> correctConnections = new List<int[]>();

    private List<Coroutine> pulseCoroutines = new List<Coroutine>();

    private void Start()
    {
        LoadLevel();

        GameObject dragLineObj = Instantiate(linePrefab, linesContainer);
        dragLineObj.name = "DragLine";
        dragLine = dragLineObj.GetComponent<Line>();
        dragLine.SetColor(selectedColor);
        dragLine.Hide();

        if (hintButton != null)
            hintButton.onClick.AddListener(ShowHint);
    }

    private bool IsPointerOverUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<Star>() != null ||
                result.gameObject.GetComponent<Line>() != null ||
                result.gameObject.name == "DragLine")
                continue;

            if (result.gameObject.GetComponent<Selectable>() != null ||
                result.gameObject.GetComponent<Text>() != null ||
                result.gameObject.GetComponent<Image>() != null)
                return true;
        }
        return false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            Star clicked = GetStarAtMouse();
            if (clicked != null)
            {
                selectedStar = clicked;
                selectedStar.SaveOriginalColor();
                selectedStar.SetColor(selectedColor);
                isDragging = true;
                dragLine.Show();
            }
        }

        if (isDragging && selectedStar != null)
        {
            Vector3 mousePos = Input.mousePosition;
            dragLine.Draw(selectedStar.transform.position, mousePos);
            ResetOtherStars();
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            if (selectedStar != null)
            {
                Star target = GetClosestStar(Input.mousePosition, selectedStar);

                if (target != null && target != selectedStar)
                {
                    if (selectedStar.isNoiseStar || target.isNoiseStar)
                        ShowWrong(selectedStar, target, true);
                    else if (IsCorrectPair(selectedStar.id, target.id))
                        Connect(selectedStar, target);
                    else
                        ShowWrong(selectedStar, target, false);
                }
                else
                    ResetStars();
            }

            isDragging = false;
            dragLine.Hide();
            CheckWin();
        }
    }

    private Star GetStarAtMouse()
    {
        return GetClosestStar(Input.mousePosition, null);
    }

    private Star GetClosestStar(Vector3 position, Star exclude)
    {
        Star closest = null;
        float minDist = starDistance;

        CheckList(stars);
        CheckList(noiseStars);

        void CheckList(List<Star> starList)
        {
            foreach (Star star in starList)
            {
                if (star == exclude) continue;
                float dist = Vector3.Distance(position, star.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = star;
                }
            }
        }

        return closest;
    }

    private void ResetStars()
    {
        foreach (Star star in stars)
            star.SetColor(star.isConnected ? connectedColor : normalColor);

        foreach (Star noise in noiseStars)
            noise.ResetToOriginalColor();
    }

    private void ResetOtherStars()
    {
        foreach (Star star in stars)
            if (star != selectedStar)
                star.SetColor(star.isConnected ? connectedColor : normalColor);

        foreach (Star noise in noiseStars)
            if (noise != selectedStar)
                noise.ResetToOriginalColor();
    }

    private bool IsCorrectPair(int id1, int id2)
    {
        foreach (int[] pair in correctConnections)
            if ((pair[0] == id1 && pair[1] == id2) || (pair[0] == id2 && pair[1] == id1))
                return true;
        return false;
    }

    private bool IsPairMade(int id1, int id2)
    {
        foreach (Line line in lines)
            if ((line.star1.id == id1 && line.star2.id == id2) ||
                (line.star1.id == id2 && line.star2.id == id1))
                return true;
        return false;
    }

    private void Connect(Star star1, Star star2)
    {
        if (IsPairMade(star1.id, star2.id)) return;

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

    private void ShowWrong(Star star1, Star star2, bool isNoise)
    {
        GameObject lineObj = Instantiate(linePrefab, linesContainer);
        Line line = lineObj.GetComponent<Line>();
        line.Draw(star1.transform.position, star2.transform.position);
        line.SetColor(isNoise ? new Color(1f, 0.5f, 0.5f) : wrongColor);
        Destroy(lineObj, 1f);

        StartCoroutine(FlashStars(star1, star2, isNoise));
    }

    IEnumerator FlashStars(Star star1, Star star2, bool isNoise)
    {
        Color flashColor = isNoise ? new Color(1f, 1f, 1f) : wrongColor;
        star1.SetColor(flashColor);
        star2.SetColor(flashColor);

        yield return new WaitForSeconds(0.5f);

        if (star1.isNoiseStar) star1.ResetToOriginalColor();
        else star1.SetColor(star1.isConnected ? connectedColor : normalColor);

        if (star2.isNoiseStar) star2.ResetToOriginalColor();
        else star2.SetColor(star2.isConnected ? connectedColor : normalColor);
    }

    private void LoadLevel()
    {
        ClearAll();
        StopAllStarPulses();
        Constellation constellation = constellations[currentIndex];

        for (int i = 0; i < constellation.connections.Length; i += 2)
            correctConnections.Add(new int[] {
                constellation.connections[i],
                constellation.connections[i + 1]
            });

        for (int i = 0; i < constellation.starPositions.Length; i++)
        {
            Vector2 pos = constellation.starPositions[i];
            Vector3 screenPos = new Vector3(pos.x * Screen.width, pos.y * Screen.height, 0);

            GameObject starObj = Instantiate(starPrefab, starsContainer);
            starObj.transform.position = screenPos;

            Star star = starObj.GetComponent<Star>();
            star.id = i;
            star.isNoiseStar = false;
            star.SetOriginalColor(normalColor);

            if (starSprites.Length > 0)
            {
                Sprite randomSprite = starSprites[Random.Range(0, starSprites.Length)];
                star.SetSprite(randomSprite);
            }

            stars.Add(star);

            StartStarPulse(star);
        }

        CreateNoiseStars();
        if (hintButton != null) hintButton.interactable = true;
    }

    private void CreateNoiseStars()
    {
        int count = Random.Range(minNoiseStars, maxNoiseStars + 1);

        for (int i = 0; i < count; i++)
        {
            Vector2 pos = GetRandomPosition();
            GameObject noiseObj = Instantiate(noiseStarPrefab, noiseStarsContainer);
            noiseObj.transform.position = new Vector3(pos.x, pos.y, 0);

            Star noiseStar = noiseObj.GetComponent<Star>();
            noiseStar.id = -100 - i;
            noiseStar.isNoiseStar = true;
            noiseStar.SetOriginalColor(noiseStarColor);
            noiseStar.SetColor(noiseStarColor);

            if (starSprites.Length > 0)
            {
                Sprite randomSprite = starSprites[Random.Range(0, starSprites.Length)];
                noiseStar.SetSprite(randomSprite);
            }

            noiseStars.Add(noiseStar);
        }
    }

    private Vector2 GetRandomPosition()
    {
        for (int attempt = 0; attempt < 100; attempt++)
        {
            float padding = 0.1f;
            float x = Random.Range(padding, 1f - padding) * Screen.width;
            float y = Random.Range(padding, 1f - padding) * Screen.height;
            Vector2 pos = new Vector2(x, y);

            bool farEnough = true;

            foreach (Star star in stars)
                if (Vector2.Distance(pos, star.transform.position) < minDistanceBetweenStars)
                { farEnough = false; break; }

            foreach (Star noise in noiseStars)
                if (Vector2.Distance(pos, noise.transform.position) < minDistanceBetweenStars)
                { farEnough = false; break; }

            if (farEnough) return pos;
        }

        return new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
    }

    private void ClearAll()
    {
        foreach (Star star in stars) Destroy(star.gameObject);
        foreach (Star noise in noiseStars) Destroy(noise.gameObject);
        foreach (Line line in lines) Destroy(line.gameObject);

        stars.Clear();
        noiseStars.Clear();
        lines.Clear();
        correctConnections.Clear();
    }

    private void CheckWin()
    {
        foreach (int[] pair in correctConnections)
            if (!IsPairMade(pair[0], pair[1]))
                return;

        SceneManager.LoadScene(nextScene);
    }

    public void ShowHint()
    {
        foreach (Star noise in noiseStars)
            noise.SetColor(new Color(0.5f, 0.5f, 0.5f, 0.3f));

        foreach (Star star in stars)
        {
            bool hasUnmadeConnections = false;
            foreach (int[] pair in correctConnections)
            {
                if ((pair[0] == star.id || pair[1] == star.id) && !IsPairMade(pair[0], pair[1]))
                { hasUnmadeConnections = true; break; }
            }

            if (!hasUnmadeConnections)
                star.SetColor(new Color(1f, 1f, 1f, 0.3f));
        }

        StartCoroutine(BlinkCorrectStars());
        Invoke("ResetStars", 3f);
    }

    private IEnumerator BlinkCorrectStars()
    {
        List<Star> toBlink = new List<Star>();

        foreach (int[] pair in correctConnections)
        {
            if (IsPairMade(pair[0], pair[1])) continue;

            Star s1 = stars.Find(s => s.id == pair[0]);
            Star s2 = stars.Find(s => s.id == pair[1]);

            if (s1 != null && !toBlink.Contains(s1)) toBlink.Add(s1);
            if (s2 != null && !toBlink.Contains(s2)) toBlink.Add(s2);
        }

        for (int i = 0; i < 6; i++)
        {
            foreach (Star star in toBlink)
                star.SetColor(i % 2 == 0 ? hintColor : normalColor);

            yield return new WaitForSeconds(0.5f);
        }
    }

    private void StartStarPulse(Star star)
    {
        Coroutine pulseCoroutine = StartCoroutine(StarPulseCoroutine(star));
        pulseCoroutines.Add(pulseCoroutine);
    }

    private void StopAllStarPulses()
    {
        foreach (Coroutine coroutine in pulseCoroutines)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }
        pulseCoroutines.Clear();
    }

    private IEnumerator StarPulseCoroutine(Star star)
    {
        Image starImage = star.GetComponent<Image>();
        RectTransform rectTransform = star.GetComponent<RectTransform>();
        Vector3 originalScale = rectTransform.localScale;

        while (true)
        {
            float delay = Random.Range(minPulseDelay, maxPulseDelay);
            yield return new WaitForSeconds(delay);

            float elapsed = 0f;
            while (elapsed < pulseDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.PingPong(elapsed / pulseDuration * 2, 1f);
                float scale = Mathf.Lerp(pulseMinScale, pulseMaxScale, t);
                rectTransform.localScale = originalScale * scale;
                yield return null;
            }

            rectTransform.localScale = originalScale;
        }
    }

    public void NextLevel()
    {
        currentIndex = (currentIndex + 1) % constellations.Length;
        LoadLevel();
    }

    public void Restart()
    {
        LoadLevel();
    }
}