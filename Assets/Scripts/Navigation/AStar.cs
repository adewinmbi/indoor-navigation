using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour {
    [SerializeField] private PointCloud pointCloud;

    private float nodeLength = 1;
    private List<Node> currentPath = new List<Node>();
    private int currentNode = 1;
    /* Right now, there is no optimization with path generation. Because of this, 
     * a new path is generated every single time before moving to the next node, 
     * so the next node (currentNode) will always be the first one.*/

    private void Start() {
        nodeLength = pointCloud.GetScale();
    }

    private class Node { // Dont't need to override GetHashCode() because lists aren't hash-based data structures
        public float f, g, h;
        public Node parent;
        public Vector2 position;

        public Node(Vector2 position) {
            this.position = position;
            this.parent = null;

            f = 0; // g + h
            g = 0; // Cost to move from starting node to this node
            h = 0; // Cost to move from this node to final destination
        }

        public override bool Equals(object obj) {
            if (obj == null) return false;
            Node objAsNode = obj as Node;
            return Equals(objAsNode);
        }

        public bool Equals(Node other) {
            if (other == null) return false;
            return (this.position.Equals(other.position));
        }

        public void SetParent(Node parent) {
            this.parent = parent;
        }
    }

    private float DistanceBetween(Vector2 a, Vector2 b) {
        return Vector2.Distance(a, b);
    }

    // Get neighbors of a node
    private List<Node> GetSuccessors(Node parent) {
        float parentX = parent.position.x;
        float parentY = parent.position.y;

        // Note: Can be optimized because we are generating successors even if we have already reached the goal. Or, generate neighbor on grid generation.
        List<Node> successors = new List<Node>();
        successors.Add(new Node(new Vector2(parentX, parentY + nodeLength))); // North
        successors.Add(new Node(new Vector2(parentX - nodeLength, parentY + nodeLength))); // Northwest
        successors.Add(new Node(new Vector2(parentX + nodeLength, parentY + nodeLength))); // Northeast
        successors.Add(new Node(new Vector2(parentX, parentY - nodeLength))); // South
        successors.Add(new Node(new Vector2(parentX - nodeLength, parentY - nodeLength))); // Southwest
        successors.Add(new Node(new Vector2(parentX + nodeLength, parentY - nodeLength))); // Southeast
        successors.Add(new Node(new Vector2(parentX - nodeLength, parentY))); // West
        successors.Add(new Node(new Vector2(parentX + nodeLength, parentY))); // East

        return successors;
    }

    // Get neighbors of a vector2
    private List<Vector2> GetSuccessors(Vector2 parent) {
        float parentX = parent.x;
        float parentY = parent.y;

        List<Vector2> successors = new List<Vector2>();
        successors.Add(new Vector2(parentX, parentY + 1)); // North
        successors.Add(new Vector2(parentX - 1, parentY + 1)); // Northwest
        successors.Add(new Vector2(parentX + 1, parentY + 1)); // Northeast
        successors.Add(new Vector2(parentX, parentY - 1)); // South
        successors.Add(new Vector2(parentX - 1, parentY - 1)); // Southwest
        successors.Add(new Vector2(parentX + 1, parentY - 1)); // Southeast
        successors.Add(new Vector2(parentX - 1, parentY)); // West
        successors.Add(new Vector2(parentX + 1, parentY)); // East

        return successors;
    }

    /// <summary>
    /// Finds neighbors of all points, not including the points themselves.
    /// </summary>
    /// <param name="points">Points to find neighbors of.</param>
    /// <returns></returns>
    private List<Vector2> GetSuccessors(List<Vector2> points) {
        List<Vector2> listSuccessors = new List<Vector2>();

        foreach (Vector2 vec in points) {
            List<Vector2> pointSuccessors = GetSuccessors(vec);

            foreach (Vector2 pointSuccessor in pointSuccessors) {
                if (!listSuccessors.Contains(pointSuccessor) && !points.Contains(pointSuccessor)) {
                    listSuccessors.Add(pointSuccessor);
                    // pointCloud.DrawPoint(pointSuccessor, Color.magenta, "AStarPath");
                }
            }
        }

        return listSuccessors;
    }

    private List<Node> Algorithm(Vector2 start, Vector2 goal) {
        int maxIterations = 99; // Arbitrary value
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();

        // Initialize starting node
        Node startingNode = new Node(start); // Node that the walker is currently on
        startingNode.g = 0;
        startingNode.f = DistanceBetween(start, goal);
        openList.Add(startingNode);

        Node q = startingNode; // Current node

        while (openList.Count > 0) {
            if (maxIterations < 0) {
                return null;
            }

            q = openList[0];
            foreach (Node node in openList) { // Find the node with the smallest f
                if (node.f < q.f || node.f == q.f && node.h < q.h) {
                    q = node;
                }
            }

            openList.Remove(q); // Remove q from the open list
            closedList.Add(q);
            pointCloud.DrawPoint(pointCloud.PointToWorld(q.position), Color.grey, "AStarPath");

            /*Generate 2-unit wide buffer around goal area. This is because an 
             * obstacle buffer is generated around all obstacles, and the lidar 
             * cannot tell which obstacles are part of the goal.*/
            Vector2 pointGoal = pointCloud.PointToWorld(goal);
            List<Vector2> goalBuffer = GetSuccessors(pointGoal);
            List<Vector2> goalBufferBuffer = GetSuccessors(goalBuffer);
            foreach (Vector2 vec in goalBufferBuffer) {
                pointCloud.DrawPoint(vec, Color.magenta, "AStarPath");
            }

            // Make into or statement
            if (q.position.Equals(goal)) {
                return RetracePath(startingNode, q);
            } 

            if (goalBufferBuffer.Contains(pointCloud.PointToWorld(q.position))) {
                return RetracePath(startingNode, q);
            }

            List<Node> successors = GetSuccessors(q);
            foreach (Node successor in successors) {
                successor.g = DistanceBetween(successor.position, q.position) + q.g;
                successor.h = DistanceBetween(successor.position, goal);
                successor.f = successor.g + successor.h;
            }

            // Variables to check collision with walls
            List<Vector2> hitPoints = pointCloud.GetHitPoints();
            List<Vector2> obstacleBuffer = GetSuccessors(hitPoints);

            foreach (Node successor in successors) {
                if (closedList.Contains(successor) 
                    || hitPoints.Contains(pointCloud.PointToWorld(successor.position))
                    || obstacleBuffer.Contains(pointCloud.PointToWorld(successor.position))) {
                    continue;
                }

                float tentativeG = q.g + 1;

                if (!openList.Contains(successor) || tentativeG < successor.g) {
                    successor.g = tentativeG;
                    successor.f = successor.g + successor.h;
                    successor.SetParent(q);
                    
                    if (!openList.Contains(successor)) {
                        openList.Add(successor);
                        pointCloud.DrawPoint(pointCloud.PointToWorld(successor.position), Color.green, "AStarPath");
                    }
                }
            }

            maxIterations--;
        }

        return null;
    }

    private List<Node> RetracePath(Node startNode, Node endNode) {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        foreach (Node n in path) {
            pointCloud.DrawPoint(pointCloud.PointToWorld(n.position), Color.cyan, "AStarPath");
        }

        currentPath = path;
        return path;
    }

    /// <summary>
    /// Returns true when next point is reached.
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    public bool MoveToNextPoint(float speed) {
        float step = speed * Time.deltaTime;
        if (currentPath == null) {
            Debug.Log("try harder");
            return false;
        }

        if (currentPath.Count <= 1) {
            Debug.Log("Path size is less than 1!");
            return true;
        }
        Vector2 targetVec2 = pointCloud.PointToWorld(currentPath[currentNode].position);
        Vector3 target = new Vector3(targetVec2.x, transform.position.y, targetVec2.y);

        if (transform.position.Equals(target)) {
            return true;
        } else {
            Quaternion rotation = Quaternion.LookRotation(target - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, step);
            transform.position = Vector3.MoveTowards(transform.position, target, step);
            return false;
        }
    }

    public void GeneratePath() {
        pointCloud.RemoveAllPoints("AStarPath");
        Algorithm(pointCloud.getWalkerPointPosition(), pointCloud.getWatchPointPosition());
    }

}
