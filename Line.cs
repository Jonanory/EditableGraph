using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditableGraph
{
    public class Line : MonoBehaviour
    {

        public Node node1;
        public Node node2;

        public static Line currentlyDrawing = null;

        private LineRenderer line;
        public bool isSetting = false;

        void Start()
        {
            line = this.gameObject.GetComponent<LineRenderer>();
            Graph.allLines.Add(this);
        }

        private void OnDestroy()
        {
            if (node1 != null && node2 != null)
            {
                node1.connectedNodes.Remove(node2);
                node2.connectedNodes.Remove(node1);
            }

            Graph.allLines.Remove(this);
        }

        void Update()
        {
            if (node2 != null)
            {
                DrawBetweenPoints(node1.transform.position, node2.transform.position);
            }
            else
            {
                DrawBetweenPoints(node1.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }

            if (isSetting)
            {
                if (Input.GetMouseButtonUp(1))
                {
                    Set();
                }
            }
        }

        void DrawBetweenPoints(Vector2 a, Vector2 b)
        {
            line.SetPosition(0, a);
            line.SetPosition(1, b);
        }

        // Connect two attached nodes, or delete if no endpoint
        void Set()
        {
            bool foundNode = false;
            foreach (Node _node in Graph.allNodes)
            {
                if (_node.isHovering == false) continue;

                if (node1 != _node && node1.connectedNodes.Contains(_node) == false)
                {
                    _node.CreateConnection(node1);
                    node2 = _node;
                    currentlyDrawing = null;
                    foundNode = true;
                    isSetting = false;
                    break;
                }
            }
            if (foundNode == false)
            {
                currentlyDrawing = null;
                Destroy(gameObject);
            }
        }
    }
}
