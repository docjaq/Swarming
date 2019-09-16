using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotBehaviour : MonoBehaviour {

    private Rigidbody rigidBody;
    
    [SerializeField] private float defaultForce = 10;
    [SerializeField] private float maxForce = 20;
    
    [SerializeField] private float defaultTorque = 0.5f;
    [SerializeField] private float maxTorque = 1;

    public RobotLeadBehaviour robotLeadBehaviour { get; set; }

    private void Awake() {
        rigidBody = GetComponent<Rigidbody>();
    }
    
    private void FixedUpdate() {
        
        DirectFollow();        
    }
    
    //trivial move
    private void DirectFollow() {

        var toLead = DirectionToLeadRobot();
        var forwards = transform.up;
        var up = transform.forward;

        var directionalMagnitude = Vector3.Dot(forwards,toLead);

        var forwardForce = forwards*defaultForce*directionalMagnitude;

        var angularForce = Vector3.SignedAngle(forwards, toLead, up);

        rigidBody.AddTorque(up*defaultTorque*angularForce);

        rigidBody.AddForce(forwardForce);
    }

    private void GatherNeighbours() {
        
        
    }

    private Vector3 DirectionToLeadRobot() {
        return robotLeadBehaviour.transform.position - transform.position;
    }
}
