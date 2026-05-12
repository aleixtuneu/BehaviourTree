using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyPerception))]
[RequireComponent(typeof(Animator))]
public class EnemyBT : MonoBehaviour
{
    [Header("Patrol")]
    public Transform[] waypoints;       // mínim 3 punts
    private int _waypointIndex = 0;

    [Header("Search")]
    public float searchDuration = 5f;   // temps buscant el player
    private float _searchTimer;
    private Vector3 _lastSeenPosition;
    private bool _isSearching = false;

    private NavMeshAgent _agent;
    private EnemyPerception _perception;
    private Node _rootNode;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _perception = GetComponent<EnemyPerception>();
        _rootNode = BuildTree();
    }

    void Update()
    {
        _rootNode.Evaluate();
    }

    // Arbre
    Node BuildTree()
    {
        // Atacar
        var attackSequence = new Sequence(new List<Node>
        {
            new ConditionNode(() => _perception.PlayerInAttackRange),
            new ActionNode(AttackPlayer)
        });

        // Perseguir (inclou Search quan perd el player)
        var chaseSequence = new Sequence(new List<Node>
        {
            new ConditionNode(() => _perception.CanSeePlayer || _isSearching),
            new ActionNode(ChaseOrSearch)
        });

        // Patrullar (estat per defecte)
        var patrolSequence = new Sequence(new List<Node>
        {
            new ConditionNode(() => !_perception.CanSeePlayer && !_isSearching),
            new ActionNode(Patrol)
        });

        // prova Attack ? Chase ? Patrol en ordre
        return new Selector(new List<Node>
        {
            attackSequence,
            chaseSequence,
            patrolSequence
        });
    }

    // Accions
    NodeState AttackPlayer()
    {
        _agent.isStopped = true;
        transform.LookAt(_perception.PlayerTransform);
        return NodeState.Success;
    }

    NodeState ChaseOrSearch()
    {
        if (_perception.CanSeePlayer)
        {
            // Acaba l'estat search si el torna a veure
            _isSearching = false;
            _lastSeenPosition = _perception.PlayerTransform.position;
            _agent.isStopped = false;
            _agent.SetDestination(_lastSeenPosition);
        }
        else
        {
            // El player s'ha perdut, iniciar/continuar Search
            if (!_isSearching)
            {
                _isSearching = true;
                _searchTimer = searchDuration;
                // Patrol random dins la zona on el va perdre
                SetRandomSearchDestination();
            }

            _searchTimer -= Time.deltaTime;

            // Ha acabat el temps, tornar a Patrol normal
            if (_searchTimer <= 0f)
            {
                _isSearching = false;
                return NodeState.Failure; // força tornar al Patrol
            }

            // Quan arriba al punt, nou punt de cerca
            if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
                SetRandomSearchDestination();
        }

        return NodeState.Running;
    }

    NodeState Patrol()
    {
        _agent.isStopped = false;

        if (waypoints.Length == 0) return NodeState.Failure;

        // Comprovar per distància
        float dist = Vector3.Distance(transform.position, waypoints[_waypointIndex].position);
        if (dist < 1f)
        {
            _waypointIndex = (_waypointIndex + 1) % waypoints.Length;
            _agent.SetDestination(waypoints[_waypointIndex].position);
        }
        else if (!_agent.hasPath)
        {
            _agent.SetDestination(waypoints[_waypointIndex].position);
        }

        return NodeState.Running;
    }

    // Utils
    void SetRandomSearchDestination()
    {
        Vector3 randomDir = Random.insideUnitSphere * 5f;
        randomDir += _lastSeenPosition;
        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            _agent.SetDestination(hit.position);
    }
}
