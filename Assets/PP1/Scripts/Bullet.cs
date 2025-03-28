using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
            return;
        else
            Destroy(gameObject);
    }
}
