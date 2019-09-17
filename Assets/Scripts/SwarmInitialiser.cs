using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Random = UnityEngine.Random;

public class SwarmInitialiser : MonoBehaviour {

    [SerializeField] private RobotLeadBehaviour robotLeadBehaviour;
    [SerializeField] private int swarmSize;

    [SerializeField] private RobotBehaviour robotBehaviourPrefab;
    private void Start() {

        var camera = Camera.main;
        
        var halfHeight = camera.orthographicSize - 1;
        var halfWidth = camera.aspect * halfHeight - 1;
        
        for (var i = 0; i < swarmSize; i++) {
            var robotBehaviour = Instantiate(robotBehaviourPrefab, transform);
            robotBehaviour.robotLeadBehaviour = robotLeadBehaviour;
            robotBehaviour.index = i;
            robotBehaviour.transform.position = new Vector3(Random.Range(-halfWidth, halfWidth), Random.Range(-halfHeight, halfHeight), 0);
        }
    }
}
