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
    public GameObject playerText;

    [SyncVar(hook = nameof(SetColor))]
    public Color color;

    public SpriteRenderer sr;

    [SyncVar]
    public float hp = 10;

    private void Start()
    {
        Invoke("TurnOffText", 2);
    }

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

            // dispara izq o derecha
            Transform firePoint = mousePosition.x > transform.position.x ? rightFirePoint : leftFirePoint;

            CmdShoot(direction, firePoint.position);
        }
    }

    [Command]
    void CmdShoot(Vector3 direction, Vector3 firePointPosition)
    {
        if (bulletPrefab == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePointPosition, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * bulletSpeed;
        }

        NetworkServer.Spawn(bullet);
        Destroy(bullet, 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            print("Colisión detectada");
            hp -= 1;
            print("HP: " + hp);
            if (hp <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    [Command]
    private void TurnOffText()
    {
        playerText.SetActive(false);
    }

    [Command]
    private void CommandSetColor(Color newColor)
    {
        color = newColor;
    }

    private void SetColor(Color oldColor, Color newColor)
    {
        sr.color = newColor;
    }

    public override void OnStartClient()
    {
        CommandSetColor(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
    }
}
