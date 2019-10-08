﻿using UnityEngine;
using System.Collections;
using Pathfinding;

public class WanderingDestinationSetter : MonoBehaviour
{
	public float radius = 20;

	IAstarAI ai;

	void Start()
	{
		ai = GetComponent<IAstarAI>();
	}

	Vector3 PickRandomPoint()
	{
		var point = Random.insideUnitSphere * radius;
        point += ai.position;
        return point;
	}

	void Update()
	{
		// Update the destination of the AI if
		// the AI is not already calculating a path and
		// the ai has reached the end of the path or it has no path at all
		if (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath))
		{
			ai.destination = PickRandomPoint();
			ai.SearchPath();
		}
	}
}