using System.Collections.Generic;
public class DialogueNode
{
    public string speakerName;
    public string dialogueText;

  
    public Dictionary<string, DialogueNode> nexts;

    public DialogueNode(string text, string speaker = "Boss")
    {
        dialogueText = text;
        speakerName = speaker;
        nexts = new Dictionary<string, DialogueNode>();
    }

    public void AddChoice(string choiceText, DialogueNode nextNode)
    {
        nexts.Add(choiceText, nextNode);
    }
}

public class DialogueTree
{
    public DialogueNode rootNode;
}