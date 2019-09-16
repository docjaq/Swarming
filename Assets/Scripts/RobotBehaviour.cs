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

    [SerializeField] private float deadZone = 0.1f;

    [SerializeField] private AnimationCurve approachVelocityCurve;
    
    public RobotLeadBehaviour robotLeadBehaviour { get; set; }

    [SerializeField]
    private List<RobotBehaviour> neighbours;

    
    
    private void Awake() {
        rigidBody = GetComponent<Rigidbody>();
        neighbours = new List<RobotBehaviour>();
    }
    
    private void FixedUpdate() {
        
        DirectFollow();        
    }
    
    //trivial move
    private void DirectFollow() {

        var toLead = DirectionToLeadRobot();
        var forwards = transform.up;
        var up = transform.forward;

        var normalisedDirectionalMagnitude = Vector3.Dot(forwards,toLead);

        var distanceToLead = Vector3.Distance(transform.position, robotLeadBehaviour.transform.position);
        //distanceToLead = Mathf.Max(distanceToLead - deadZone, 0);
        var approachVelocityScale = approachVelocityCurve.Evaluate(distanceToLead);
        
        var forwardForce = forwards*defaultForce*normalisedDirectionalMagnitude*approachVelocityScale;
        
        var angularForce = Vector3.SignedAngle(forwards, toLead, up);

        rigidBody.AddTorque(up*defaultTorque*angularForce);

        rigidBody.AddForce(forwardForce);
    }

    private void GatherNeighbours() {
        
        
    }

    private Vector3 DirectionToLeadRobot() {
        return Vector3.Normalize(robotLeadBehaviour.transform.position - transform.position);
    }

    private void OnTriggerEnter(Collider other) {
        neighbours.Add(other.gameObject.GetComponent<RobotBehaviour>());
    }

    private void OnTriggerExit(Collider other) {
        neighbours.Remove(other.gameObject.GetComponent<RobotBehaviour>());
    }
}
