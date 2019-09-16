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

        for (var i = 0; i < swarmSize; i++) {
            var robotBehaviour = Instantiate(robotBehaviourPrefab, transform);
            robotBehaviour.robotLeadBehaviour = robotLeadBehaviour;
            robotBehaviour.index = i;
            robotBehaviour.transform.position = new Vector3(Random.Range(-14, 14), Random.Range(-9, 9), 0);
        }
    }
}
