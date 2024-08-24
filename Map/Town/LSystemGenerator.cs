using System.Text;

public class LSystemGenerator
{
    Rule[] rules;
    string rootSentence;
    int iterationLimit;
    float chanceToIgnoreRule;
    
    public LSystemGenerator(Rule[] rules, string rootSentence, int iterationLimit, float chanceToIgnoreRule) {
        this.rules = rules;
        this.rootSentence = rootSentence;
        this.iterationLimit = iterationLimit;
        this.chanceToIgnoreRule = chanceToIgnoreRule;
    }

    public string generateSentence(string word = null) {

        if (word == null) {
            word = rootSentence;
        }

        return growRecursive(word);

    }

    private string growRecursive(string word, int iterationIndex = 0) {

        if (iterationIndex >= iterationLimit) {
            return word;
        }
        StringBuilder newWord = new StringBuilder();

        foreach(var c in word) {
            newWord.Append(c);
            processRulesRecursively(newWord, c, iterationIndex);
        }

        return newWord.ToString();

    }

    private void processRulesRecursively(StringBuilder newWord, char c, int iterationIndex) {
        foreach(var rule in rules) {
            if (rule.letter == c.ToString()) {
                if (UnityEngine.Random.value < chanceToIgnoreRule) {
                    return;
                }
                newWord.Append(growRecursive(rule.GetResults(), iterationIndex + 1));
            }
        }
    }

}
