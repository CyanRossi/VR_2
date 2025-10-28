using UnityEngine;

public class ArrowCaster : MonoBehaviour
{
    [SerializeField] private Transform tip;
    [SerializeField] private LayerMask layerMask = ~0;
    [SerializeField] private int damageAmount = 10;

    private Vector3 lastPosition = Vector3.zero;

    public bool CheckForCollision(out RaycastHit hit)
    {
        if (lastPosition == Vector3.zero)
            lastPosition = tip.position;

        bool collided = Physics.Linecast(lastPosition, tip.position, out hit, layerMask);
        if (collided)
        {
            GameObject hitObject = hit.transform.gameObject;
            if (hitObject.CompareTag("Enemy"))
            {
                // Check for SpiderEnemy
               // SpiderEnemy enemy = hit.transform.GetComponent<SpiderEnemy>();
                //if (enemy != null)
                //{
                //    enemy.TakeDamage(damageAmount);
                //    return true;
                //}
                //// Apply impact force and disable/enable NavMeshAgent
                //if (hit.rigidbody != null)
                //{
                //    enemy.DisableAgent();
                //    hit.rigidbody.AddForce(-hit.normal * 100f, ForceMode.Impulse); // impact force
                //    enemy.EnableAgent();
                //}
            }
        }
        lastPosition = collided ? lastPosition : tip.position;

        return collided;
    }
}
