using UnityEngine;

public class EnemyPerception : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRadius = 10f;
    public float attackRadius = 2f;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;

    public Transform PlayerTransform { get; private set; }
    public bool CanSeePlayer { get; private set; }
    public bool PlayerInAttackRange { get; private set; }

    void Update()
    {
        DetectPlayer();
    }

    void DetectPlayer()
    {
        // Comprovar si el player és a la zona de detecció
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        if (hits.Length == 0)
        {
            CanSeePlayer = false;
            PlayerTransform = null;
            PlayerInAttackRange = false;
            return;
        }

        PlayerTransform = hits[0].transform;

        // Raycast per comprovar línia de visió (obstacles al mig?)
        Vector3 dir = (PlayerTransform.position - transform.position).normalized;
        float dist = Vector3.Distance(transform.position, PlayerTransform.position);

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, dist, obstacleLayer))
        {
            CanSeePlayer = false;   // hi ha un obstacle al mig
        }         
        else
        {
            CanSeePlayer = true;
        }
            
        // Rang d'atac
        PlayerInAttackRange = dist <= attackRadius;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
