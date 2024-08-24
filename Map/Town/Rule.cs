using UnityEngine;

[CreateAssetMenu(fileName = "Rule", menuName = "Procedural Generation/Rule")]
public class Rule : ScriptableObject 
{

    public string letter;

    [SerializeField]
    private string[] results;   
    [SerializeField]
    private bool randomResult = false;

    public string GetResults() {
        if (!randomResult) return results[0];
        int randomIndex = Random.Range(0, results.Length);
        return results[randomIndex];
    } 

}