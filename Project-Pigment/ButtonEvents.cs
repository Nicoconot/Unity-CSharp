// These events are called when the button is pressed or released, which currently
// means that a collider is overlapping with it, according to the predefined conditions.
// The behaviors called depend on what has been added in the inspector. This allows for
// great variability in button behavior, while remaining very friendly in the inspector.
// This works very similarly to BoxEvents.

// This type of script will be further developed to be even friendlier to non-devs and to
// modify Unity's standard UI.

using UnityEngine;
using UnityEngine.Events;

public class ButtonEvents : MonoBehaviour
{
    [Header("Button definitions")]
    [Space]
    public ButtonTypes buttonType;
    [Tooltip("If permanent, it only needs to be pressed once. Otherwise, it must be constantly pressed or it turns back off on its own.")]
    public bool isPermanent = false;
    [Tooltip("Each box, enemy or player weighs 1")]
    public int weightNeeded = 1;
    [Tooltip("If the button has a timer, it needs to be pressed for a set amount of time and will release slowly.")]
    
    //Timer to be implemented. This would work as those types of buttons that you press down for a certian amount
    // of time, then it gets released slowly, controlling a door or lever.
    public bool hasTimer = false;
    public float timeToPress = 0;

    private bool isPressed;


    private SpriteRenderer sr;

    [SerializeField]private int internalWeight;

    [Header("Events")]
    [Space]

    public UnityEvent OnButtonStep;

    public UnityEvent OnButtonRelease;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        UpdateButtonType();
    }

    public void PressDown()
    {
        transform.localScale = new Vector3(4, 0.5f, 1);
    }

    public void PressUp()
    {
        transform.localScale = new Vector3(4, 1, 1);
    }

    public void UpdateButtonType()
    {
        // We have 3 main button types. One is exclusively activated by a player, one
        // is exclusively activated by an enemy, and one doesn't have a preference aside
        // from weight(explained in the CheckWeight() method).

        switch (buttonType)
        {
            case ButtonTypes.PlayerExclusive:
                sr.color = Color.red;
                break;
            case ButtonTypes.EnemyExclusive:
                sr.color = Colors.orange;
                break;
            case ButtonTypes.WeightOnly:
                sr.color = Color.green;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (buttonType)
        {
            case ButtonTypes.PlayerExclusive:
                if (collision.CompareTag("Player"))
                {
                    internalWeight += 1;
                }
                break;
            case ButtonTypes.EnemyExclusive:
                if (collision.CompareTag("Enemy"))
                {
                    internalWeight += 1;
                }
                break;
            case ButtonTypes.WeightOnly:
                internalWeight += 1;
                break;
        }
        CheckWeight();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (buttonType)
        {
            case ButtonTypes.PlayerExclusive:
                if (collision.CompareTag("Player"))
                {
                    internalWeight -= 1;
                }
                break;
            case ButtonTypes.EnemyExclusive:
                if (collision.CompareTag("Enemy"))
                {
                    internalWeight -= 1;
                }
                break;
            case ButtonTypes.WeightOnly:
                internalWeight -= 1;
                break;
        }
        CheckWeight();
    }


    private void CheckWeight()
    {
        // Each object has, a priori, a weight of 1. Such a weight is added to the button's internal
        // counter and, when it matches or overcome's the desired weight for the button to be pressed,
        // it will be.

        if (internalWeight >= weightNeeded)
        {
            if (!isPressed)
            OnButtonStep.Invoke();
            isPressed = true;
            Debug.Log(name + " pressed");
        }

        if (internalWeight < weightNeeded && !isPermanent)
        {
            if (isPressed)
            OnButtonRelease.Invoke();
            isPressed = false;
            Debug.Log(name + " released");
        }
    }
}

public enum ButtonTypes{
    PlayerExclusive, EnemyExclusive, WeightOnly
}
