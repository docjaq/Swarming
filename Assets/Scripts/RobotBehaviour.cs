using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class RobotBehaviour : MonoBehaviour {

    private Rigidbody rigidBody;

    public int index = 0;
    
    [SerializeField] private float defaultForce = 10;
    [SerializeField] private float maxForce = 20;
    
    [SerializeField] private float defaultTorque = 0.5f;
    [SerializeField] private float maxTorque = 1;

    [SerializeField] private float deadZone = 0.1f;

    [SerializeField] private AnimationCurve approachVelocityCurve;
    
    [SerializeField] private AnimationCurve relaxationCurve;
    
    public RobotLeadBehaviour robotLeadBehaviour { get; set; }

    [SerializeField]
    private List<RobotBehaviour> neighbours;

    [SerializeField] private SphereCollider neighbourCollider;
    private float maxNeighbourDistance;
    
    private Vector3 adjustmentForce;

    [SerializeField] bool debug;
    [SerializeField] private LineRenderer forwardsDebugRenderer;
    [SerializeField] private LineRenderer adjustmentDebugRenderer;
    
    private void Awake() {
        rigidBody = GetComponent<Rigidbody>();
        neighbours = new List<RobotBehaviour>();
    }

    private void Start() {
        adjustmentForce = Vector3.zero;
        maxNeighbourDistance = neighbourCollider.radius;
    }
    
    private void FixedUpdate() {
        
        DirectFollow();        
    }

    private Vector3 followerVelocity = Vector3.zero;
    private Vector3 previousForce = Vector3.zero;
    
    //trivial move
    private void DirectFollow() {

        var toLead = DirectionToRobotLead();
        //var toLead = DirectionToRobotLeadHistory();
        var forwards = transform.up;
        var up = transform.forward;

        var normalisedDirectionalMagnitude = Vector3.Dot(forwards,toLead);
        
        //forwardsDebugRenderer.SetPosition(0, transform.localPosition);
        //forwardsDebugRenderer.SetPosition(1, toLead);

        var distanceToLead = Vector3.Distance(transform.position, robotLeadBehaviour.transform.position);
        var approachVelocityScale = approachVelocityCurve.Evaluate(distanceToLead);
        var forwardForce = forwards*normalisedDirectionalMagnitude*approachVelocityScale*defaultForce;
        
        //float newPosition = Mathf.SmoothDamp(transform.position.y, target.position.y, ref yVelocity, smoothTime);
       
        forwardForce += adjustmentForce*defaultForce;
        
        //var targetForce = Vector3.SmoothDamp(previousForce, forwardForce, ref followerVelocity, 0.3f);

        var angularForce = Vector3.SignedAngle(forwards, toLead, up);

        rigidBody.AddTorque(up*defaultTorque*angularForce);

        rigidBody.AddForce(forwardForce);

        previousForce = forwardForce;

        //ComputeAdjustmentVector();
    }

    private void Update() {
        
        adjustmentForce = Vector3.zero;
        var adjustmentVector = Vector3.zero;

        if (!neighbours.Any())
            return;

        foreach (var neighbour in neighbours) {

            if (neighbour == null)
                continue;
            
            var direction = neighbour.transform.position - transform.position;

            adjustmentVector += direction;
        }

        if (adjustmentVector == Vector3.zero)
            return;

        //Invert the magnitude
        var adjustmentMagnitude = maxNeighbourDistance-adjustmentVector.magnitude;
        var normalisedAdjustmentMagnitude = adjustmentMagnitude / maxNeighbourDistance;
        
        adjustmentVector = Vector3.Normalize(adjustmentVector);
        var scaledMagnitude = relaxationCurve.Evaluate(normalisedAdjustmentMagnitude);

        //Scale this because both element in the pair are both trying to move away from each other
        //scaledMagnitude /= 2;

        //not sure why I need to do this. Signs are flipping.
        scaledMagnitude = Math.Abs(scaledMagnitude);
        
        
        adjustmentVector *= scaledMagnitude*maxNeighbourDistance;

        if (index == 0)
            Debug.Log(adjustmentMagnitude);

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
        if (other.gameObject.layer == LayerMask.NameToLayer("SwarmRobot")) 
            neighbours.Add(other.gameObject.GetComponent<RobotBehaviour>());

    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("SwarmRobot"))
            neighbours.Remove(other.gameObject.GetComponent<RobotBehaviour>());
        
    }
}
