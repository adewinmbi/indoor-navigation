using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour {
    [SerializeField] private PointCloud pointCloud;

    private float nodeLength = 1;
    private List<Node> currentPath = new List<Node>();
    private int currentNode = 0;

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

            if (q.position.Equals(goal)) {
                return RetracePath(startingNode, q);
            } else {
                foreach (Node n in GetSuccessors(new Node(goal))) {
                    if (q.position.Equals(n.position)) {
                        RetracePath(startingNode, q);
                        return null;
                    }
                }
            }

            List<Node> successors = GetSuccessors(q);
            /*float qX = q.position.x;
            float qY = q.position.y;

            // Note: Can be optimized because we are generating successors even if we have already reached the goal. Or, generate neighbor on grid generation.
            List<Node> successors = new List<Node>();
            successors.Add(new Node(new Vector2(qX, qY + nodeLength))); // North
            successors.Add(new Node(new Vector2(qX - nodeLength, qY + nodeLength))); // Northwest
            successors.Add(new Node(new Vector2(qX + nodeLength, qY + nodeLength))); // Northeast
            successors.Add(new Node(new Vector2(qX, qY - nodeLength))); // South
            successors.Add(new Node(new Vector2(qX - nodeLength, qY - nodeLength))); // Southwest
            successors.Add(new Node(new Vector2(qX + nodeLength, qY - nodeLength))); // Southeast
            successors.Add(new Node(new Vector2(qX - nodeLength, qY))); // West
            successors.Add(new Node(new Vector2(qX + nodeLength, qY))); // East*/

            foreach (Node successor in successors) {
                successor.g = DistanceBetween(successor.position, q.position) + q.g;
                successor.h = DistanceBetween(successor.position, goal);
                successor.f = successor.g + successor.h;
            }

            foreach (Node successor in successors) {
                if (closedList.Contains(successor) || pointCloud.IsObstacle(pointCloud.PointToWorld(successor.position))) {
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

    public bool MoveToNextPoint(float speed) {
        float step = speed * Time.deltaTime;
        if (currentPath == null) {
            Debug.Log("try harder");
            return false;
        }

        Vector2 targetVec2 = pointCloud.PointToWorld(currentPath[currentNode].position);
        Vector3 target = new Vector3(targetVec2.x, transform.position.y, targetVec2.y);
        if (transform.position.Equals(target)) {
            currentNode++;
            return true;
        } else {
            transform.position = Vector3.MoveTowards(transform.position, target, step);
            return false;
        }
    }

    public void GeneratePath() {
        pointCloud.RemoveAllPoints("AStarPath");
        Algorithm(pointCloud.getWalkerPointPosition(), pointCloud.getWatchPointPosition());
        currentNode = 0;
        Debug.Log("Path generated!");
    }

}
