using UnityEngine;

public class AgentParameters {

    public Vector2Int position;
    public Vector2Int direction;
    public int length;

    public AgentParameters(Vector2Int position, Vector2Int direction, int length) {
        this.position = position;
        this.direction = direction;
        this.length = length;
    }

}