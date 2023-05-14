using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour {
    [SerializeField] private PointCloud pointCloud;

    private float nodeLength = 1;
    private float diagonalNodeDistance = Mathf.Sqrt(2); // Diagonal distance between each node

    private void Start() {
        nodeLength = pointCloud.GetScale();
        diagonalNodeDistance = Mathf.Sqrt(2 * nodeLength);
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

    private float DiagonalDistance(Vector2 a, Vector2 b) {
        float dX = Mathf.Abs(a.x - b.x);
        float dY = Mathf.Abs(a.y - b.y);

        // return nodeLength * (dX + dY) + (diagonalNodeDistance - (2 * nodeLength)) * Mathf.Min(new float[] { dX, dY });
        return Vector2.Distance(a, b);
    }

    /*private List<Node> RegeneratePath(Node start, Node current) {

    }*/
    public void GeneratePath(Vector2 start, Vector2 goal) {
        int hasbeendone = 0;
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();

        // Initialize starting node
        Node startingNode = new Node(start); // Node that the walker is currently on
        startingNode.g = 0;
        startingNode.f = DiagonalDistance(start, goal);
        openList.Add(startingNode);

        Node q = startingNode; // Current node

        while (openList.Count > 0) {
            if (hasbeendone > 50) {
                return;
            }

            foreach (Node node in openList) { // Find the node with the smallest f
                if (node.f < q.f || node.f == q.f && node.h < q.h) {
                    q = node;
                }
            }

            openList.Remove(q); // Remove q from the open list
            closedList.Add(q);
            pointCloud.DrawPoint(pointCloud.PointToWorld(q.position), Color.grey);

            if (q.position.Equals(goal)) {
                RetracePath(startingNode, q);
                return;
            }

            float qX = q.position.x;
            float qY = q.position.y;

            // Note: Can be optimized because we are generating successors even if we have already reached the goal
            List<Node> successors = new List<Node>();
            successors.Add(new Node(new Vector2(qX, qY + nodeLength))); // North
            successors.Add(new Node(new Vector2(qX - nodeLength, qY + nodeLength))); // Northwest
            successors.Add(new Node(new Vector2(qX + nodeLength, qY + nodeLength))); // Northeast
            successors.Add(new Node(new Vector2(qX, qY - nodeLength))); // South
            successors.Add(new Node(new Vector2(qX - nodeLength, qY - nodeLength))); // Southwest
            successors.Add(new Node(new Vector2(qX + nodeLength, qY - nodeLength))); // Southeast
            successors.Add(new Node(new Vector2(qX - nodeLength, qY))); // West
            successors.Add(new Node(new Vector2(qX + nodeLength, qY))); // East

            /*if (hasbeendone >= 4) {
                //pointCloud.DrawDebugPoint(successor.position, Color.green, successor.parent.parent.Equals(successor).ToString());
            foreach (Node successor in successors) {
                successor.g = DiagonalDistance(successor.position, q.position) + q.g;
                successor.h = DiagonalDistance(successor.position, goal);
                successor.f = successor.g + successor.h;
                pointCloud.DrawPoint(successor.position, Color.green);
            }
                return;
            } else {*/
                foreach (Node successor in successors) {
                    successor.g = DiagonalDistance(successor.position, q.position) + q.g;
                    successor.h = DiagonalDistance(successor.position, goal);
                    successor.f = successor.g + successor.h;
                }
            //}

            foreach (Node successor in successors) {
                if (closedList.Contains(successor) || pointCloud.IsObstacle(pointCloud.PointToWorld(successor.position))) {
                    continue;
                }

                // float tentativeG = q.g + DiagonalDistance(successor.position, q.position);
                float tentativeG = q.g + 1;
                // pointCloud.DrawDebugPoint(successor.position, Color.green, Mathf.Round(successor.g).ToString() + "\n" + Mathf.Round(tentativeG).ToString());
                // successor.f = successor.g + successor.h;

                if (!openList.Contains(successor) || tentativeG < successor.g) {
                    successor.g = tentativeG;
                    successor.h = DiagonalDistance(successor.position, goal);
                    successor.f = successor.g + successor.h;
                    successor.SetParent(q);
                    // pointCloud.DrawDebugPoint(successor.position, Color.green, Mathf.Round(successor.f).ToString());
                    
                    if (!openList.Contains(successor)) {
                        openList.Add(successor);
                        pointCloud.DrawDebugPoint(pointCloud.PointToWorld(successor.position), Color.green, Mathf.Round(successor.f).ToString());
                    }
                }
                
                /*bool successorSkipped = false;
                // Conditions to skip successors
                foreach (Node n in openList) {
                    if (n.position == successor.position) {
                        if (n.f < successor.f) {
                            successorSkipped = true;
                        }
                    }
                }

                foreach (Node n in closedList) {
                    if (n.position == successor.position && n.f < successor.f) {
                        successorSkipped = true;
                    }
                }

                if (!successorSkipped) {
                    openList.Add(q); // Only do this if break never happened
                }*/
            }

            // Push q on closed list
            // closedList.Add(q);
            hasbeendone++;
        }


    }

    private void RetracePath(Node startNode, Node endNode) {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        foreach (Node n in path) {
            pointCloud.DrawPoint(pointCloud.PointToWorld(n.position), Color.cyan);
        }
    }

    /*foreach (Node n in path) {
            pointCloud.DrawPoint(n.position, Color.cyan);
            Debug.Log(n.position);
        }*/

    public void RunAStar() {
        GeneratePath(pointCloud.getWalkerPointPosition(), pointCloud.getWatchPointPosition());
    }

}
