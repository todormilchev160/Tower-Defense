using UnityEngine;
using UnityEngine.AI;
public class EnemyWalk : MonoBehaviour 
{ 
    [SerializeField] private string destinationTag = "EnemyDestination";
     private NavMeshAgent agent; 
     private Transform destination; 
     void Start() 
     { 
        agent = GetComponent<NavMeshAgent>(); 
        GameObject destinationObject = GameObject.FindGameObjectWithTag(destinationTag); 
        if (destinationObject != null) 
        {
             destination = destinationObject.transform; 
             agent.SetDestination(destination.position); 
        }
    } 
public void ResumeMovement() 
{
     if (destination == null) return; 
     agent.isStopped = false; 
     agent.SetDestination(destination.position); 
} 
}