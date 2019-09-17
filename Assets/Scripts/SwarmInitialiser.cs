using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SwarmInitialiser : MonoBehaviour {
   
    [SerializeField] private RobotLeadBehaviour robotLeadBehaviour;
    [SerializeField] private int swarmSize;

    [SerializeField] private RobotBehaviour robotBehaviourPrefab;

    private Stack<RobotBehaviour> activeSwarmRobots;

    private float halfHeight;
    private float halfWidth;
    
    //Annoying coupling due to bug in 2019.2.5 (dynamic values are not working)
    [SerializeField] private Slider swarmSizeSlider;
    
    private void Start() {
        
        activeSwarmRobots = new Stack<RobotBehaviour>();
        
        var camera = Camera.main;
        
        halfHeight = camera.orthographicSize - 1;
        halfWidth = camera.aspect * halfHeight - 1;
    }
    
    public void SwarmSizeUpdated() {
        UpdateNumberOfRobots((int)swarmSizeSlider.value);
    }

    private void UpdateNumberOfRobots(int updateNumRobots) {

        var difference = updateNumRobots - activeSwarmRobots.Count;

        if (difference == 0)
            return;

        if (difference < 0) {

            for (var i = 0; i < Mathf.Abs(difference); i++) {

                var robotToRemove = activeSwarmRobots.Pop();
                Destroy(robotToRemove.gameObject);
            }
        } else {
            for (var i = 0; i < difference; i++) {
                SpawnRobot();
            }
        }

    }

    private void SpawnRobot() {
        
        var robotBehaviour = Instantiate(robotBehaviourPrefab, transform);
        robotBehaviour.robotLeadBehaviour = robotLeadBehaviour;
        robotBehaviour.transform.position = new Vector3(Random.Range(-halfWidth, halfWidth), Random.Range(-halfHeight, halfHeight), 0);
        activeSwarmRobots.Push(robotBehaviour);
    }
}
