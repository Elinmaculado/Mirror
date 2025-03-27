using UnityEngine;
using Mirror;
using UnityEngine.Rendering;

public class Playercontroller : NetworkBehaviour
{
    public float playerSpeed = 2.5f;
    public GameObject bulletPrefab;
    public Transform rightFirePoint;
    public Transform leftFirePoint;
    public float bulletSpeed = 1f;

    [SyncVar]
    public float hp = 10;
    private void Update()
    {
        if (!isLocalPlayer) return;

        // Movimiento del jugador
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 playerMovement = new Vector3(h * playerSpeed, v * playerSpeed, 0) * Time.deltaTime;
        transform.position += playerMovement;

        // Detectar clic para disparar
        if (Input.GetMouseButtonDown(0))
        {
           
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            Vector3 direction = (mousePosition - transform.position).normalized;
            /*
            if (mousePosition.x > transform.position.x)
            {
                CmdShoot(direction, mousePosition, rightFirePoint);
            }
            else
            {
                CmdShoot(direction, mousePosition, leftFirePoint);
            }
            */
                // Llamamos al comando en el servidor para disparar
                CmdShoot(direction, mousePosition);
        }
    }

    [Command]
    void CmdShoot(Vector3 direction, Vector3 mousePosition/*, Transform firePoint*/)
    {
        if (bulletPrefab == null || rightFirePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, rightFirePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * bulletSpeed;
        }

        // Spawnear la bala en la red
        NetworkServer.Spawn(bullet);
        Destroy(bullet, 3);
    }

    public void TakeDamage(float damage)
    {
        if (!isServer) return;

        hp -= damage;
        Debug.Log("Current HP: " + hp);

        if (hp <= 0)
        {
            NetworkServer.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Bullet"))
        {
            print("Colision detectada");
            hp -= 1;
            print("HP: " + hp);
            if (hp <= 0) 
            { 
                Destroy(gameObject);
            }
        }
    }
}
