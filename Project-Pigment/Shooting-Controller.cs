// Some exciting changes will be made to this code soon. We will stop relying on rotating the gun to face the mouse pointer, and instead 
// automatically swap out the gun sprites for the one that most accurately points to the cursor, within a list. This will allow us to be truly
// pixel-perfect. No cheats!

using UnityEngine;
using UnityEngine.UI;

public class ShootingController : MonoBehaviour
{
    public static GunColor gunColor;
    public Transform fireTip;
    public GameObject gun;
    public GameObject bulletPrefab;
    public float bulletSpeed;

    public Text debugText;
    public Text colorText;


    private bool isColiding;
    [SerializeField]private float previousAngle;

   
    void Update()
    {  
        if (Input.GetButtonDown("Fire1") && !GameManager.gamePaused)
        {
            Shoot();
        }

        RotateGun();
        ChangeGunColors();
    }

    private void RotateGun()
    {
        //Here we check the current mouse position and adjust the gun's rotation according to it

        //Get the Screen positions of the gun object
        Vector2 positionOnScreen = Camera.main.WorldToViewportPoint(transform.position);

        //Get the Screen position of the mouse
        Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);

        //Get the angle between where the gun is pointing and the mouse position
        float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);



        debugText.text = angle.ToString();
        //Ta Daaa
        if (!GameManager.gamePaused) gun.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
    }       

    void Shoot()
    {
        //In order to shoot, we first instantiate a bullet at the gun's tip. We then get its rigidbody 
        // and apply force to it.
        GameObject bullet = Instantiate(bulletPrefab, fireTip.position, Quaternion.identity, null);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.AddForce(-fireTip.right * bulletSpeed, ForceMode2D.Impulse);
    }

    void ChangeGunColors()
    {
        if (!GameManager.gamePaused)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                gunColor = GunColor.red;
                colorText.text = "RED";
                colorText.color = Color.red;
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                gunColor = GunColor.orange;
                colorText.text = "ORANGE";
                colorText.color = Colors.orange;
                // "Colors" is a script of our own to compensate for the small number of colors
                // Unity provides in its "Color" class. Be careful not to mix the two up!
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                gunColor = GunColor.yellow;
                colorText.text = "YELLOW";
                colorText.color = Color.yellow;
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                gunColor = GunColor.green;
                colorText.text = "GREEN";
                colorText.color = Color.green;
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                gunColor = GunColor.blue;
                colorText.text = "BLUE";
                colorText.color = Color.blue;
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                gunColor = GunColor.purple;
                colorText.text = "PURPLE";
                colorText.color = Colors.purple;
            }
        }
        
    }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // I used this for something before, but I can't remember what...
        // Will probably delete isColiding later
        isColiding = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isColiding = false;
    }
}
