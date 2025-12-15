using UnityEngine;

public class CheckPointSys : MonoBehaviour
{
    private Vector3 lastCheckpoint;

    void Start()
    {
        lastCheckpoint = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Respawn();
        }
    }

    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpoint = position;
    }

    public void Respawn()
    {
        transform.position = lastCheckpoint;
    }
}
