using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderPathing : MonoBehaviour
{
    private Transform player;
    public Transform endPoint;
    [SerializeField] private List<Transform> currentNodes;
    public Transform targetLadder;
    private Vector2 destination;
    [SerializeField] private float climbSpeed = 2f;
    public bool endReached = false;
    private bool exiting = false;
    public bool onLadder = false;

    void OnEnable(){
        onLadder = false;
        exiting = false;
        endReached = false;
        endPoint = null;
        currentNodes = new List<Transform>();
        player = GameObject.FindWithTag("Player").transform;
    }

    void FixedUpdate(){

        if(endPoint != null){

            GetClosestNode();
            if(transform.position.x != targetLadder.position.x && !endReached && !exiting){
                
                onLadder = false;
                destination = new Vector2(targetLadder.position.x, transform.position.y);
                transform.position = Vector2.MoveTowards(transform.position, destination, climbSpeed * Time.deltaTime);

            } else if(Mathf.Abs(transform.position.y - endPoint.position.y) != 0 && !exiting){
                
                onLadder = true;
                destination = new Vector2(transform.position.x, endPoint.position.y);
                transform.position = Vector2.MoveTowards(transform.position, destination, climbSpeed * Time.deltaTime);    

            } else{
                
                if(Mathf.Abs(transform.position.x - endPoint.position.x) != 0){
                    onLadder = true;
                    exiting = true;
                    destination = new Vector2(endPoint.position.x, transform.position.y);
                    transform.position = Vector2.MoveTowards(transform.position, destination, climbSpeed * Time.deltaTime); 
                } else{
                    onLadder = false;
                    endReached = true;
                }
            }
        }

    }

    public void GrabNodes(GameObject ladder){
        targetLadder = ladder.transform;
        endReached = false;

        for(int i = 0; i < targetLadder.childCount; i++){
            currentNodes.Add(targetLadder.GetChild(i));
        }
        GetClosestNode();
    }

    private void GetClosestNode(){

        endPoint = currentNodes[0];

        float playerDistance = Vector2.Distance(player.position, currentNodes[0].position);
        float bestEnd = playerDistance;

        for(int i = 1; i < currentNodes.Count; i++){
            playerDistance = Vector2.Distance(player.position, currentNodes[i].position);

            if(playerDistance <= bestEnd){
                bestEnd = playerDistance;
                endPoint = currentNodes[i];

            }
        }
    }
}
