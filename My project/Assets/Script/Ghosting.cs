using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghosting : MonoBehaviour
{
    [SerializeField] private Tilemap myWall;
    [SerializeField] private float time = 3f;

    private TilemapCollider2D wallCollider;
    private SpriteRenderer playerSprite;
    private bool nowGhost = false;

    private void Start()
    {
        wallCollider = myWall.GetComponent<TilemapCollider2D>();
        playerSprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !nowGhost)
        {
            StartCoroutine(GoGhost());
        }
    }

    private IEnumerator GoGhost()
    {
        nowGhost = true;
        Vector3 oldPos = transform.position;

        SetGhostMode(true);

        yield return new WaitForSeconds(time);

        SetGhostMode(false);

        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            if (wallCollider != null)
            {
                wallCollider.enabled = true;
                if (playerCollider.IsTouching(wallCollider))
                {
                    transform.position = oldPos;
                }

            }
        }

        nowGhost = false;
    }

    private void SetGhostMode(bool isGhost)
    {
        float playerAlpha = isGhost ? 0.5f : 1f;
        float wallAlpha = isGhost ? 0.3f : 1f;

        if (playerSprite != null)
        {
            Color c = playerSprite.color;
            c.a = playerAlpha;
            playerSprite.color = c;
        }

        if (myWall != null)
        {
            Color wallColor = myWall.color;
            wallColor.a = wallAlpha;
            myWall.color = wallColor;
        }

        if (wallCollider != null)
        {
            wallCollider.enabled = !isGhost;
        }
    }
}