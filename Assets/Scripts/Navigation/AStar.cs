using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour {
    [SerializeField] private PointCloud pointCloud;

    private float nodeLength = 1;
    private float diagonalNodeDistance = Mathf.Sqrt(2); // Diagonal distance between each node

    private void Start() {
        nodeLength = pointCloud.GetResolutionDecreaseFactor();
        diagonalNodeDistance = Mathf.Sqrt(2 * nodeLength);
    }

    private class Node {
        public float f, g, h;
        public Node parent;
        public Vector2 position;

        public Node(Vector2 position, Node parent) {
            this.position = position;
            this.parent = parent;

            f = 0; // g + h
            g = 0; // Cost to move from starting node to this node
            h = 0; // Cost to move from this node to final destination
        }
    }

    private static float DiagonalDistance(Vector2 a, Vector2 b, float nodeLength, float diagonalNodeDistance) {
        float dX = Mathf.Abs(a.x - b.x);
        float dY = Mathf.Abs(a.y - b.y);

        return nodeLength * (dX + dY) + (diagonalNodeDistance - (2 * nodeLength)) * Mathf.Min(new float[] { dX, dY });
    }

    public void GeneratePath(Vector2 start, Vector2 goal) {
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();

        // Initialize starting node
        Node startingNode = new Node(start, null); // Node that the walker is currently on
        startingNode.f = 0;
        openList.Add(startingNode);

        Node q = startingNode; // Value with smallest f
        while (openList.Count != 0) {

            foreach (Node node in openList) { // Find the node with the smallest f
                if (node.f < q.f) {
                    q = node;
                }
            }

            openList.Remove(q); // Pop q off the open list
            float qX = q.position.x;
            float qY = q.position.y;

            List<Node> successors = new List<Node>();
            successors.Add(new Node(new Vector2(qX, qY + nodeLength), q)); // North
            successors.Add(new Node(new Vector2(qX - nodeLength, qY + nodeLength), q)); // Northwest
            successors.Add(new Node(new Vector2(qX + nodeLength, qY + nodeLength), q)); // Northeast
            successors.Add(new Node(new Vector2(qX, qY - nodeLength), q)); // South
            successors.Add(new Node(new Vector2(qX - nodeLength, qY - nodeLength), q)); // Southwest
            successors.Add(new Node(new Vector2(qX + nodeLength, qY - nodeLength), q)); // Southeast
            successors.Add(new Node(new Vector2(qX - nodeLength, qY), q)); // West
            successors.Add(new Node(new Vector2(qX + nodeLength, qY), q)); // East

            foreach (Node successor in successors) {
                if (successor.position == goal) {
                    // End search because the goal is reached
                    return;
                }

                successor.g = q.g + DiagonalDistance(successor.position, q.position, nodeLength, diagonalNodeDistance);
                successor.f = successor.g + successor.h;

                // Conditions to skip successors
                foreach (Node n in openList) {
                    if (n.position == successor.position) {
                        if (n.f < successor.f) {
                            break;
                        }
                    }
                }

                foreach (Node n in closedList) {
                    if (n.position == successor.position) {
                        if (n.f < successor.f) {
                            break;
                        }
                    }
                }

            }

            // Push q on closed list
            closedList.Add(q);
        }

        foreach (Node n in closedList) {
            pointCloud.DrawPoint(n.position, Color.cyan);
        }
    }

    public void Debug() {
        GeneratePath(pointCloud.getWalkerPointPosition(), pointCloud.getWatchPointPosition());
    }

}
