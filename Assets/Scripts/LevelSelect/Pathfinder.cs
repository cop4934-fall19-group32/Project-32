using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
	/**
	 * Function to handle user RouteTo request (clicking on map node)
	 * @param targetNode The node to route to
	 */
	public Queue<MapNode> RouteTo(MapNode currentNode, MapNode targetNode) {
		//Dictionaries used to record graph data
		Dictionary<MapNode, float> costs = new Dictionary<MapNode, float>();
		Dictionary<MapNode, MapNode> parentDictionary = new Dictionary<MapNode, MapNode>();

		//Visited set to prevent multiple visitation
		HashSet<MapNode> visited = new HashSet<MapNode>();

		//Horizon of nodes visible but yet to be visited
		PriorityQueue<MapNode> horizon = new PriorityQueue<MapNode>();

		//Initialize graph data as viewed from start node
		costs[currentNode] = 0;
		parentDictionary[currentNode] = currentNode;
		horizon.Push(currentNode, 0);

		var targetPosition = targetNode.transform.position;

		//A* Pathfinding algorithm
		while (!horizon.IsEmpty) {
			MapNode curr = horizon.Top;
			horizon.Pop();

			if (curr == targetNode) {
				break;
			}

			RelaxCosts(curr, costs, parentDictionary);

			var currPosition = curr.transform.position;

			foreach (var neighbor in curr.AdjacencyList) {
				if (!visited.Contains(neighbor) && !neighbor.Locked) {
					var neighborPosition = neighbor.transform.position;
					var cost = CalculateCost(currPosition, neighborPosition);
					horizon.Push(neighbor, cost + CalcHeuristic(neighborPosition, targetPosition));
					visited.Add(neighbor);
				}
			}
		}

		Debug.Log("Target Reachable: " + visited.Contains(targetNode));

		if (visited.Contains(targetNode)) {
			return ReconstructShortestPath(targetNode, parentDictionary);
		}
		else {
			return new Queue<MapNode>();
		}

	}

	/**
	 * Upon a succesful run of the A* pathfinding algorithm, the shortest path is reconstructed and
	 * send to the Level Select Character as a queue of waypoints
	 */
	private Queue<MapNode> ReconstructShortestPath(MapNode targetNode, Dictionary<MapNode, MapNode> parentDictionary) {
		var curr = targetNode;
		Stack<MapNode> pathStack = new Stack<MapNode>();
		//The start node recorded it's parent as itself in the initialization phase
		/**
		 * The start node is ultimately the parent of all paths in the graph, so walking backwards
		 * from the target node will always give the shortest path from Target -> Start
		 */
		while (parentDictionary[curr] != curr) {
			pathStack.Push(curr);
			curr = parentDictionary[curr];
		}

		//Reverse path, and send it to level select character for navigation
		var pathArray = pathStack.ToArray();
		Queue <MapNode> path = new Queue<MapNode>(pathArray);

		return path;
	}

	/**
	 * Function to update shortest-cost dictionary after visiting a new node in pathfinding
	 * @param curr The current node being visited in the pathfinding process
	 * @param costs Reference to the current cost dictionary
	 * @param parents Reference to the current parents dictionary
	 */
	private void RelaxCosts(MapNode curr, Dictionary<MapNode, float> costs, Dictionary<MapNode, MapNode> parents) {
		//Attempt to lower cost estimates for all neighboring nodes
		foreach (var neighbor in curr.AdjacencyList) {
			var edgeCost = CalculateCost(curr.transform.position, neighbor.transform.position);

			if (costs.ContainsKey(neighbor)) {
				//If neigbhor already has a cost estimate, attemp to relax that cost
				if (costs[curr] + edgeCost < costs[neighbor]) {
					costs[neighbor] = costs[curr] + edgeCost;
					parents[neighbor] = curr;

				}
				else {
					continue;
				}
			}
			else {
				//Estimate cost must be current cost
				costs[neighbor] = costs[curr] + edgeCost;
				parents[neighbor] = curr;
			}
		}
	}

	/**
	 * Cost calculating function for the A* pathfinding algorithm found in RouteTo
	 * @param curr The position of the current node
	 * @param next The position of the neighbor node being considered
	 * @return The cost to travel from curr to next
	 */
	private float CalculateCost(Vector3 curr, Vector3 next) {
		//Cost metric is simply the square distance between nodes
		return (curr - next).sqrMagnitude;
	}

	/**
	 * Function to calculate the A* heuristic cost for a target node 
	 * @param next The neighbor node being considered
	 * @param target The target node for the pathfinding operation
	 * @return The heuristic cost associated with selecting the neigbor node
	 */
	private float CalcHeuristic(Vector3 next, Vector3 target) {
		//For the A* heuristic, simply add the the square distance from the considered node
		//to the target node. This will cause the algorithm to prefer nodes that tend
		//to lead us closer to the target.
		return (next - target).sqrMagnitude;
	}
}
