using UnityEngine;
using System.Collections;

public class Charm : MonoBehaviour
{
    [Header("Настройки")]
    public float requiredTime = 3f;

    private PlayerController player;
    private bool isPlayerInside = false;
    private float timer = 0f;
    private Coroutine damageCoroutine;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<PlayerController>();
            isPlayerInside = true;
            timer = 0f;
            damageCoroutine = StartCoroutine(DamageTimer());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            timer = 0f;

            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    IEnumerator DamageTimer()
    {
        while (isPlayerInside && timer < requiredTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        if (isPlayerInside)
            player.GetDamage();
    }
}