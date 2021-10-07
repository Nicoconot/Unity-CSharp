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

    private Vector3 mousePos;

    public Sprite[] gunSprites;

    public SpriteRenderer gunSr;

    public PlayerSFX sfxController;
    [SerializeField] private float previousAngle;


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
        // This script works in two separate ways. We have an object called "gun" that is invisible in-game and
        // is simply responsible for placing the gun's tip correctly so we can shoot out of it.
        // Secondly, we have another object called "gunArm" and its Sprite renderer, which contains 21 different sprites
        // (x2 for each side) that are swapped depending on the mouse position. This is because we have a pixel
        // art game, and simply rotating the sprite looks ugly and wrong.


        //GETTING MOUSE AND PLAYER SCREEN POSITIONS

        //Get the Screen positions of the player object
        Vector2 positionOnScreen = Camera.main.WorldToViewportPoint(transform.position);


        //Get the Screen position of the mouse
        Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);

        //Get the angle between where the gun is pointing and the mouse position
        float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);


        //Here we rotate the invisible gun object to change the fireTip's position.
        if (!GameManager.gamePaused) gun.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));


        // In order to figure out which sprites to display at which angle, we first must convert the angle variable
        // into the range of our sprites array. This is done with this formula:

        // NewValue = (((OldValue - OldMin) * (NewMax - NewMin)) / (OldMax - OldMin)) + NewMin
        // Adapted to our needs:
        // NewValue = ((OldValue +180) * 41) / 360


        // Old range: -180 to 180
        // New range: 1 to (spriterange length, 41)

        int newValue = Mathf.RoundToInt(((angle + 180) * 41) / 360);


        gunSr.sprite = gunSprites[newValue];

        debugText.text = newValue.ToString();

        // Some tweaking had to be made to get the sprites order right, but it works perfectly.
    }

    void Shoot()
    {
        sfxController.PlaySFX("shot");

        // We could use the same vectors in RotateGun() but through game testing we found out that these two work slightly better 
        // for precise shooting, while the ones in RotateGun() serve their purpose the best
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)fireTip.transform.position).normalized;



        //In order to shoot, we first instantiate a bullet at the gun's tip. We then get its rigidbody 
        // and apply force to it.
        GameObject bullet = Instantiate(bulletPrefab, fireTip.position, Quaternion.identity, null);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        //bulletRb.AddForce(-fireTip.transform.right * bulletSpeed);
        bulletRb.AddForce(direction * bulletSpeed, ForceMode2D.Impulse);
    }

    void ChangeGunColors()
    {
        // For now, we switch the gun's color using the alphanumeric keyboard. I am currently developing a nicer system 
        // that uses the mouse wheel.

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

}
