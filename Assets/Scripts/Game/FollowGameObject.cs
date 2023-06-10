using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowGameObject : MonoBehaviour
{
    public enum Axis
    {
        X = 1,
        Y = 2,
        Z = 4
    }

    public GameObject followedGameObject;
    public GameObject gameObjectToFollow;
    public List<Axis> followAxises;

    private Axis ChosenAxis;


    private void Awake()
    {
        foreach (var axis in followAxises)
            ChosenAxis |= axis;
    }

    private void Update()
    {
        Vector3 followedGOPosition = followedGameObject.transform.position;
        if (ChosenAxis.HasFlag(Axis.X))
            followedGOPosition.x = gameObjectToFollow.transform.position.x;
        if (ChosenAxis.HasFlag(Axis.Y))
            followedGOPosition.y = gameObjectToFollow.transform.position.y;
        if (ChosenAxis.HasFlag(Axis.Z))
            followedGOPosition.z = gameObjectToFollow.transform.position.z;
        followedGameObject.transform.position = followedGOPosition;
    }
}
