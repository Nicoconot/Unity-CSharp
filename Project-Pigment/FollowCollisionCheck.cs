using UnityEngine;

public class FollowCollisionCheck : MonoBehaviour
{
    public EnemyAI enemyAIScript;

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
    }
}
