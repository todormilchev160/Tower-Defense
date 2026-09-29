using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class RunningTroops : MonoBehaviour
{
    public float damage=2;
    private NavMeshAgent navMeshAgent;
    private Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshAgent=GetComponent<NavMeshAgent>();
        navMeshAgent.SetDestination(target.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
