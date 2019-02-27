using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditableGraph
{
    public class Graph : MonoBehaviour
    {
        public static Graph master;

        public static List<Node> allNodes = new List<Node>();
        public static List<Line> allLines = new List<Line>();

        [HideInInspector]
        public Line currentlyDrawingLine = null;

        public GameObject LinePrefab;

        // If nodes are outside the radius, they will move towards the center 
        public float radius = 5;

        bool drawingLine = false;
        Vector2 cutLineStart;
        Vector2 cutLineEnd;
        LineRenderer cutLine;

        private void Awake()
        {
            if (master == null)
            {
                master = this;
            }
            else if (master != this)
            {
                Destroy(gameObject);
            }

            cutLine = GetComponent<LineRenderer>();
        }

        void Update()
        {
            if (drawingLine)
            {
                cutLine.SetPosition(1, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }

            foreach (Node _node in allNodes)
            {
                _node.DetermineMovement();
            }
            foreach (Node _node in allNodes)
            {
                _node.Move();
            }
        }

        public void OnMouseDown()
        {
            drawingLine = true;
            cutLine.enabled = true;
            cutLineStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            cutLine.SetPosition(0, cutLineStart);
            cutLine.SetPosition(1, cutLineStart);
        }

        public void OnMouseUp()
        {
            cutLineEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            drawingLine = false;
            cutLine.enabled = false;
            foreach (Line _line in allLines)
            {
                if (LineCrosses(cutLineStart, cutLineEnd, _line.node1.transform.position, _line.node2.transform.position))
                {
                    Destroy(_line.gameObject);
                }
            }
        }

    // Determine if two lines, with given endpoints, cross each other
        public bool LineCrosses(Vector2 aStart, Vector2 aEnd, Vector2 bStart, Vector2 bEnd)
        {
            Vector2 aNorm = Vector2.Perpendicular(aEnd - aStart);
            Vector2 bNorm = Vector2.Perpendicular(bEnd - bStart);

            if (Vector2.Dot(bStart - aStart, aNorm) * Vector2.Dot(bEnd - aStart, aNorm) > 0) return false;
            if (Vector2.Dot(aStart - bStart, bNorm) * Vector2.Dot(aEnd - bStart, bNorm) > 0) return false;
            return true;
        }
    }

}