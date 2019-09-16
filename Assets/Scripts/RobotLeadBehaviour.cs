using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RobotLeadBehaviour : MonoBehaviour {

    private Rigidbody rigidBody;
    private LineRenderer lineRenderer;
    
    [SerializeField]
    private float rotationalTorque = 0.5f;
    [SerializeField]
    private float forceScale = 10;

    private Queue<Vector3> positionHistory = new Queue<Vector3>();
    [SerializeField]
    private int queueLimit = 10;

    private bool isUpdating;

    private void Awake() {
        
        rigidBody = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = queueLimit;
    }
    
    private void Start() {
        isUpdating = true;

        StartCoroutine(UpdateHistory());
    }

    private void FixedUpdate() {

        var updatedForce = Vector3.zero;

        if (Input.GetKey("up")) {
            updatedForce += transform.up*forceScale;;
        }

        if (Input.GetKey("left")) {
            rigidBody.AddTorque(transform.forward*rotationalTorque);
        }
        
        if (Input.GetKey("right")) {
            rigidBody.AddTorque(-transform.forward*rotationalTorque);
        }
        
        if (Input.GetKey("down")) {
            updatedForce += -transform.up*forceScale;
        }
        
        rigidBody.AddForce(updatedForce);
    }

    IEnumerator UpdateHistory() {

        while (isUpdating) {
            positionHistory.Enqueue(transform.position);
            
            UpdateQueue();
            
            yield return new WaitForSeconds(.1f);
        }
    }

    private void UpdateQueue() {

        if (!positionHistory.Any())
            return;
        
        while (positionHistory.Count > queueLimit) {
            positionHistory.Dequeue();
        } 
        
        var index = 0;
        foreach (var position in positionHistory){ 
            lineRenderer.SetPosition(index, position);
            index++;
        }
    }

    public Vector3 GetHistoryTail() {
        return positionHistory.Peek();
    }
}
