using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditableGraph
{
    public class Node : MonoBehaviour {

        public List<Node> connectedNodes = new List<Node>();

        public static float personalSpaceRadius = 0.7f;
        public bool isDragging = false;
        public bool isHovering = false;

        Vector3 amountToMove;

        void Start () {
            Graph.allNodes.Add(this);
        }

        private void OnDestroy()
        {
            Graph.allNodes.Remove(this);
        }

        void Update () {

            // Dragging node with mouse
            if(Input.GetMouseButtonDown(0))
            {
                if(isHovering)
                {
                    isDragging = true;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (isDragging)
                {
                    isDragging = false;
                }
            }
            if (isDragging)  Drag();


            // Create new line between nodes
            if(Input.GetMouseButtonDown(1))
            {
                if(isHovering)
                {
                    CreateLine();
                }
            }
        }

        // Adds node to connected node, and vice versa
        public void CreateConnection(Node newConnection)
        {
            newConnection.connectedNodes.Add(this);
            connectedNodes.Add(newConnection);
        }

        void CreateLine()
        {
            if (Line.currentlyDrawing == null)
            {
                GameObject newLine = Instantiate(Graph.master.LinePrefab);
                Line lineScript = newLine.GetComponent<Line>();
                lineScript.node1 = this;
                lineScript.isSetting = true;
                Line.currentlyDrawing = lineScript;
            }
        }

        public void DetermineMovement()
        {
            if (isDragging) return;
            amountToMove = Vector3.zero;

            // Move closer to the center if too far away
            Vector2 movementToCenter = Graph.master.transform.position - transform.position;
            Vector2 normalToCenter = Vector3.Normalize(movementToCenter);
            float distanceToCenter = movementToCenter.magnitude;

            if(distanceToCenter > 0.8f * Graph.master.radius )
            {
                amountToMove = normalToCenter * (distanceToCenter - Graph.master.radius * 0.8f);
            }

            // Move Closer to connected nodes, and further away from unconnected nodes
            foreach (Node _node in Graph.allNodes)
            {
                if (_node != this)
                {
                    Vector3 difference = _node.transform.position - transform.position;
                    Vector3 normal = Vector3.Normalize(difference);
                    float distance = difference.magnitude;

                    if (connectedNodes.Contains(_node))
                    {
                        float change = distance - 2 * personalSpaceRadius;
                        amountToMove += normal * change;
                    }
                    else if (distance < 3f)
                    {
                        float change = -Mathf.Pow(2, -distance);
                        amountToMove += normal * change;
                    }
                }
            }
        }

        void Drag()
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePosition.x, mousePosition.y, transform.position.z);
        }

        // Move under influence of other nodes
        public void Move( )
        {
            if (isDragging) return;
            amountToMove *= Time.deltaTime;
            transform.position += amountToMove;
        }

        void OnMouseOver()
        {
            isHovering = true;
        }

        void OnMouseExit()
        {
            isHovering = false;
        }
    }
}
