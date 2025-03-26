using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour
{
    public float damage = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Colision detectada");
        if (!isServer) return; // Solo el servidor maneja colisiones

        if (other.CompareTag("Player"))
        {
            Playercontroller player = other.GetComponent<Playercontroller>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
