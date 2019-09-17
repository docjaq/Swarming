using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class RobotBehaviour : MonoBehaviour {

    private Rigidbody rigidBody;

    public int index = 0;
    
    [SerializeField] private float defaultForce = 15;
    [SerializeField] private float forceVariance = 5;
    
    [SerializeField] private float defaultTorque = 0.5f;
    [SerializeField] private float torqueVariance = 0.25f;
    
    [SerializeField] private AnimationCurve approachVelocityCurve;
    
    [SerializeField] private AnimationCurve relaxationCurve;
    
    public RobotLeadBehaviour robotLeadBehaviour { get; set; }

//   
//    private List<RobotBehaviour> neighbours;

    [SerializeField] private RobotBehaviour neighbourClosest;

    [SerializeField] private SphereCollider neighbourCollider;
    private float maxNeighbourDistance;
    
    private Vector3 adjustmentForce;
    
    [SerializeField] private LineRenderer forwardsDebugRenderer;
    [SerializeField] private LineRenderer adjustmentDebugRenderer;
    
    private void Awake() {
        rigidBody = GetComponent<Rigidbody>();
        //neighbours = new List<RobotBehaviour>();
    }

    private void Start() {
        adjustmentForce = Vector3.zero;
        maxNeighbourDistance = neighbourCollider.radius;

        defaultForce += Random.Range(-forceVariance, forceVariance);
        defaultTorque += Random.Range(-torqueVariance, torqueVariance);
    }
    
    private void FixedUpdate() {
        
        DirectFollow();        
    }

    private Vector3 followerVelocity = Vector3.zero;
    private Vector3 previousForce = Vector3.zero;
    
    //trivial move
    private void DirectFollow() {

        //var toLead = DirectionToRobotLead();
        var toLead = DirectionToRobotLeadHistory();
        var forwards = transform.up;
        var up = transform.forward;

        var normalisedDirectionalMagnitude = Vector3.Dot(forwards,toLead);

        var distanceToLead = Vector3.Distance(transform.position, robotLeadBehaviour.transform.position);
        var approachVelocityScale = approachVelocityCurve.Evaluate(distanceToLead);
        var forwardForce = forwards*normalisedDirectionalMagnitude*approachVelocityScale*defaultForce;
        
       
        forwardForce += adjustmentForce*defaultForce;
        

        var angularForce = Vector3.SignedAngle(forwards, toLead, up);

        rigidBody.AddTorque(up*defaultTorque*angularForce);

        rigidBody.AddForce(forwardForce);

        previousForce = forwardForce;

        //ComputeAdjustmentVector();
    }

    private void Update() {
        
        adjustmentForce = Vector3.zero;
        var adjustmentVector = Vector3.zero;

        if (neighbourClosest == null)
            return;

        
        adjustmentVector = neighbourClosest.transform.position - transform.position;

        if (adjustmentVector == Vector3.zero)
            return;

        //Invert the magnitude
        var adjustmentMagnitude = maxNeighbourDistance-adjustmentVector.magnitude;
        var normalisedAdjustmentMagnitude = adjustmentMagnitude / maxNeighbourDistance;
        
        adjustmentVector = Vector3.Normalize(adjustmentVector);
        var scaledMagnitude = relaxationCurve.Evaluate(normalisedAdjustmentMagnitude);

        adjustmentVector *= scaledMagnitude*maxNeighbourDistance;
        
        adjustmentForce = -adjustmentVector;
    }

    private Vector3 DirectionToRobotLeadHistory() {
        var historyTail = robotLeadBehaviour.GetHistoryTail();
        return Vector3.Normalize(historyTail - transform.position);
    }

    private Vector3 DirectionToRobotLead() {
        return Vector3.Normalize(robotLeadBehaviour.transform.position - transform.position);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("SwarmRobot")) return;
        
        //Debug.Log("Swarm robot enter");
        
        var neighbour = other.gameObject.GetComponentInParent<RobotBehaviour>();

        UpdateClosest(neighbour);
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("SwarmRobot")) return;
        
        //Debug.Log("Swarm robot stay");
        
        var neighbour = other.gameObject.GetComponentInParent<RobotBehaviour>();
        UpdateClosest(neighbour);
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("SwarmRobot")) return;
        
        //Debug.Log("Swarm robot exit");
        
        var neighbour = other.gameObject.GetComponentInParent<RobotBehaviour>();

        if (neighbourClosest == neighbour)
            neighbourClosest = null;
    }

    private void UpdateClosest(RobotBehaviour neighbourCandidate) {

        if (neighbourClosest == null) {
            neighbourClosest = neighbourCandidate;
            return;
        }

        var distanceCandidate = Vector3.Distance(transform.position, neighbourCandidate.transform.position);
        var distanceCurrent = Vector3.Distance(transform.position, neighbourClosest.transform.position);

        if (distanceCandidate < distanceCurrent)
            neighbourClosest = neighbourCandidate;
    }
}
