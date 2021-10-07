// A complement to BoxScript. These events are called in separate objects called Relayers, which
// usually contain a collider that waits for a box to go through it and then relays the events
// added in the inspector. This allows us to control the box's behavior with great detail,
// when we need it. For example, we might need the box to fall in a very specific place when
// the player drops it, so we can either turn off its usual behavior and run an animation, 
// or use a combination of other events. This works very similarly to ButtonEvents.

// This type of script will be further developed to be even friendlier to non-devs and to
// modify Unity's standard UI.
using UnityEngine;

public class BoxEvents : MonoBehaviour
{
    BoxScript boxScript;
    Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxScript = GetComponent<BoxScript>();
    }


    public void TurnOffBoxScript()
    {
        boxScript.isInteractable = false;
    }

    public void TurnOnBoxScript()
    {
        boxScript.isInteractable = true;
    }

    public void TurnKinematic()
    {
        rb.bodyType = RigidbodyType2D.Static;
    }

    public void TurnDynamic()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public void TurnOffGroundCheck()
    {
        boxScript.shouldIGround = false;
        boxScript.isGrounded = false;
    }

    public void TurnOnGroundCheck()
    {
        boxScript.shouldIGround = true;
    }

    public void ChangeMass(float mass)
    {
        rb.mass = mass;
    }

    public void ChangeGravity(float gravity)
    {
        rb.gravityScale = gravity;
    }

}
