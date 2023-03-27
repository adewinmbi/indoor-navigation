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

        public Node() {
            f = 0; // g + h
            g = 0; // Cost to move from starting node to this node
            h = 0; // Cost to move from this node to final destination
        }

        public Node(Vector2 position) {
            this.position = position;
        }
    }

    private static float DiagonalDistance(Vector2 a, Vector2 b, float nodeLength, float diagonalNodeDistance) {
        float dX = Mathf.Abs(a.x - b.x);
        float dY = Mathf.Abs(a.y - b.y);

        return nodeLength * (dX + dY) + (diagonalNodeDistance - (2 * nodeLength)) * Mathf.Min(new float[] {dX, dY});
    }

    public void GeneratePath() {
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

            for (int i = 8; i >= 1; i--) { // Generate the 8 successors
                Debug.Log(i);

                Node newNode;
                switch (i) { // This shouldn't be plus 1, it should be plus nodeLength. Right now it doesn't matter because nodeLength is 1.
                    case 1:
                        newNode = new Node(new Vector2(qX, qY + 1)); // North
                        break;

                    case 2:
                        newNode = new Node(new Vector2(qX - 1, qY + 1)); // Northwest
                        break;

                    case 3:
                        newNode = new Node(new Vector2(qX + 1, qY + 1)); // Northeast
                        break;

                    case 4:
                        newNode = new Node(new Vector2(qX, qY - 1)); // South
                        break;

                    case 5:
                        newNode = new Node(new Vector2(qX - 1, qY - 1)); // Southwest
                        break;

                    case 6:
                        newNode = new Node(new Vector2(qX + 1, qY - 1)); // Southeast
                        break;

                    case 7:
                        newNode = new Node(new Vector2(qX - 1, qY)); // West
                        break;

                    case 8:
                        newNode = new Node(new Vector2(qX + 1, qY)); // East
                        break;

                    default:
                        newNode = new Node();
                        break;
                }


            }
        }
    }

}
