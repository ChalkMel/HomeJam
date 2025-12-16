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

        if (playerSprite != null)
        {
            Color c = playerSprite.color;
            c.a = 0.5f;
            playerSprite.color = c;
        }

        if (myWall != null)
        {
            Color wallColor = myWall.color;
            wallColor.a = 0.3f;
            myWall.color = wallColor;
        }

        if (wallCollider != null)
        {
            wallCollider.enabled = false;
        }

        Vector3 oldPos = transform.position;


        yield return new WaitForSeconds(time);

        if (wallCollider != null && wallCollider.enabled == false)
        {
            Collider2D playerCollider = GetComponent<Collider2D>();
            if (playerCollider != null)
            {
                playerCollider.enabled = false;
                transform.position = oldPos;
                playerCollider.enabled = true;
            }
        }

        if (wallCollider != null)
        {
            wallCollider.enabled = true;
        }

        if (myWall != null)
        {
            Color wallColor = myWall.color;
            wallColor.a = 1f;
            myWall.color = wallColor;
        }

        if (playerSprite != null)
        {
            Color c = playerSprite.color;
            c.a = 1f;
            playerSprite.color = c;
        }
        nowGhost = false;
    }
}