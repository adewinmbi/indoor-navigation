/*using System.Collections;
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

        return nodeLength * (dX + dY) + (diagonalNodeDistance - (2 * nodeLength)) * Mathf.Min(new float[] {dX, dY});
    }

    public void GeneratePath(Vector2 goal) {
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();

        // Initialize starting node
        Node startingNode = new Node(); // Node that the walker is currently on
        startingNode.f = 0;
        openList.Add(startingNode);

        while (openList.Count != 0) {
            Node q = startingNode; // Value with smallest f

            foreach (Node node in openList) { // Find the node with the smallest f
                if (node.f < q.f) {
                    q = node;
                }
            }

            openList.Remove(q);
            float qX = q.position.x;
            float qY = q.position.y;

            List<Node> successors = new List<Node>();
            successors.Add(new Node(new Vector2(qX, qY + nodeLength))); // North
            successors.Add(new Node(new Vector2(qX - nodeLength, qY + nodeLength))); // Northwest
            successors.Add(new Node(new Vector2(qX + nodeLength, qY + nodeLength))); // Northeast
            successors.Add(new Node(new Vector2(qX, qY - nodeLength))); // South
            successors.Add(new Node(new Vector2(qX - nodeLength, qY - nodeLength))); // Southwest
            successors.Add(new Node(new Vector2(qX + nodeLength, qY - nodeLength))); // Southeast
            successors.Add(new Node(new Vector2(qX - nodeLength, qY))); // West
            successors.Add(new Node(new Vector2(qX + nodeLength, qY))); // East

            foreach (Node n in successors) {
                n.SetParent(q);
                if ()
            }

            *//*for (int i = 8; i >= 1; i--) { // Generate the 8 successors
                Debug.Log(i);

                

                *//*Node newNode;
                switch (i) {
                    case 1:
                        newNode = new Node(new Vector2(qX, qY + nodeLength)); // North
                        break;

                    case 2:
                        newNode = new Node(new Vector2(qX - nodeLength, qY + nodeLength)); // Northwest
                        break;

                    case 3:
                        newNode = new Node(new Vector2(qX + nodeLength, qY + nodeLength)); // Northeast
                        break;

                    case 4:
                        newNode = new Node(new Vector2(qX, qY - nodeLength)); // South
                        break;

                    case 5:
                        newNode = new Node(new Vector2(qX - nodeLength, qY - nodeLength)); // Southwest
                        break;

                    case 6:
                        newNode = new Node(new Vector2(qX + nodeLength, qY - nodeLength)); // Southeast
                        break;

                    case 7:
                        newNode = new Node(new Vector2(qX - nodeLength, qY)); // West
                        break;

                    case 8:
                        newNode = new Node(new Vector2(qX + nodeLength, qY)); // East
                        break;

                    default:
                        newNode = new Node();
                        break;
                }*//*
            }*//*
        }
    }

}
*/