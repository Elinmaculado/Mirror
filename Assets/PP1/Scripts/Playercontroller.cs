using UnityEngine;
using Mirror;

public class Playercontroller : NetworkBehaviour
{
    public float playerSpeed = 2.5f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;

    private void Update()
    {
        if (!isLocalPlayer) return;

        // Movimiento del jugador
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 playerMovement = new Vector3(h * playerSpeed, v * playerSpeed, 0) * Time.deltaTime;
        transform.position += playerMovement;

        // Detectar clic para disparar
        if (Input.GetMouseButtonDown(0)) // Click izquierdo
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // Asegurar que est� en el mismo plano que el jugador

            Vector3 direction = (mousePosition - transform.position).normalized;

            // Llamamos al comando en el servidor para disparar
            CmdShoot(direction);
        }
    }

    [Command]
    void CmdShoot(Vector3 direction)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * bulletSpeed;
        }

        // Spawnear la bala en la red
        NetworkServer.Spawn(bullet);
        Destroy(bullet, 3);
    }
}
