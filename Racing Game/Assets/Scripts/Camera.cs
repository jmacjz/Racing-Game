using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Vector3 offset;

    // Update is called once per frame
    void Update()
    {
        transform.position = player.position + offset;
    }
}
