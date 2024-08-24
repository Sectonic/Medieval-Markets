using UnityEngine;
using System.Collections.Generic;
using System;

public class PathGenerator {

    private Rule[] rules;
    private string rootSentence;
    private int iterationLimit;
    private float changeToIgnoreRule;

    private LSystemGenerator ls;

    private int length;
    private int lengthRemove;

    public int Length
    {
        get
        {
            if (length > 2)
            {
                return length;
            }
            else
            {
                return 3;
            }
        }
        set => length = value;
    }

    private enum EncodingLetters
    {
        unknown = '1',
        save = '[',
        load = ']',
        draw = 'F',
        right = '+',
        left = '-'
    }

    public PathGenerator(Rule[] rules, string rootSentence, int iterationLimit, 
                         float changeToIgnoreRule, int length, int lengthRemove)
    {
        this.rules = rules;
        this.rootSentence = rootSentence;
        this.iterationLimit = iterationLimit;
        this.changeToIgnoreRule = changeToIgnoreRule;
        this.length = length;
        this.lengthRemove = lengthRemove;
    }

    public List<Vector2Int> generateTownPath(Vector2Int townCenter) {
        ls = new LSystemGenerator(rules, rootSentence, iterationLimit, changeToIgnoreRule);
        string sequence = ls.generateSentence();
        return createSequence(sequence, townCenter);
    }

    List<Vector2Int> createSequence(string sequence, Vector2Int townCenter) {

        List<Vector2Int> points = new List<Vector2Int>();

        Stack<AgentParameters> savePoints = new Stack<AgentParameters>();
        Vector2Int currentPosition = townCenter;
        Vector2Int direction = Vector2Int.up;
        Vector2Int tempPosition = townCenter;

        foreach(char letter in sequence) {

            EncodingLetters encoding = (EncodingLetters) letter;
            switch (encoding) {
                case EncodingLetters.save:

                    savePoints.Push(new AgentParameters(currentPosition, direction, length));
                    break;

                case EncodingLetters.load:

                    if (savePoints.Count > 0) {
                        AgentParameters lastPoint = savePoints.Pop();
                        currentPosition = lastPoint.position;
                        direction = lastPoint.direction;
                        length = lastPoint.length;
                    } else {
                        throw new Exception("Dont have save point in stack.");
                    }
                    break;

                case EncodingLetters.draw:

                    tempPosition = currentPosition;
                    currentPosition += direction * length;
                    MarkPath(points, tempPosition, currentPosition);
                    length -= lengthRemove;

                    break;

                case EncodingLetters.right:

                    direction = new Vector2Int(direction.y, -direction.x);
                    break;

                case EncodingLetters.left:

                    direction = new Vector2Int(-direction.y, direction.x);
                    break;

            }

        }

        return points;
        
    }

    private void MarkPath(List<Vector2Int> points, Vector2Int start, Vector2Int end)
    {
        int xStart = Mathf.RoundToInt(start.x);
        int yStart = Mathf.RoundToInt(start.y);
        int xEnd = Mathf.RoundToInt(end.x);
        int yEnd = Mathf.RoundToInt(end.y);

        HashSet<Vector2Int> pointSet = new HashSet<Vector2Int>(points);

        if (xStart == xEnd)
        {
            // Vertical line
            int yMin = Mathf.Min(yStart, yEnd);
            int yMax = Mathf.Max(yStart, yEnd);
            for (int y = yMin; y <= yMax; y++)
            {
                Vector2Int point = new Vector2Int(xStart, y);
                if (!pointSet.Contains(point))
                {
                    points.Add(point);
                    pointSet.Add(point);
                }
            }
        }
        else if (yStart == yEnd)
        {
            // Horizontal line
            int xMin = Mathf.Min(xStart, xEnd);
            int xMax = Mathf.Max(xStart, xEnd);
            for (int x = xMin; x <= xMax; x++)
            {
                Vector2Int point = new Vector2Int(x, yStart);
                if (!pointSet.Contains(point))
                {
                    points.Add(point);
                    pointSet.Add(point);
                }
            }
        }
    }

}