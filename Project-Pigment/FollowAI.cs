//Simple and effective script! In order to allow the enemy to follow someone within the tilemap, we can't rely on 
//pathfinding as Unity doesn't yet support it for 2D. Instead, we use a big circle collider as a child of the enemy, 
//and whenever the desired object goes within range, the enemy will face and walk in the direction said object is, 
//in relation to it's current position. It can be expanded to only allow changes in movement if the object is visible, 
//with raycasts.
using UnityEngine;

public class FollowCollisionCheck : MonoBehaviour
{
    public EnemyAI enemyAIScript;

    private bool currentlyDashing;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(enemyAIScript.followTag) && enemyAIScript.followSomething)
        {
            // If the player is in front of the enemy, the enemy should move to the right
            if (collision.transform.position.x > transform.position.x)
            {
                enemyAIScript.TurnRight();
            }
            else if (collision.transform.position.x < transform.position.x)
            {
                enemyAIScript.TurnLeft();
            }
        }

        if(enemyAIScript.enemyColor == EnemyColor.red && collision.CompareTag("Player"))
        {
            float collisionY = collision.transform.position.y;
            float selfY = transform.position.y;
            float yDifference = collisionY - selfY;

            if (((collisionY > selfY && yDifference <= 12) || yDifference <= 5) && !currentlyDashing)
            {
                currentlyDashing = true;
                // In order for the red enemy to dash towards the player, the player must be close by(12 units of difference) and
                // above the enemy. Alternatively, if the player is below but at 5 units of difference or less, the enemy will also dash.
                // Raycasts will be implemented to make this check more precise, later.
                if (collision.transform.position.x > transform.position.x)
                {
                    StartCoroutine(enemyAIScript.RedDash(true));
                }
                else if (collision.transform.position.x < transform.position.x)
                {
                    StartCoroutine(enemyAIScript.RedDash(false));
                }
            }            
            
        }
    }

    public void DashIsFinished()
    {
        currentlyDashing = false;
    }
}
