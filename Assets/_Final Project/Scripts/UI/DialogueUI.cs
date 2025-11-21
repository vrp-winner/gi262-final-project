using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEditor;
using UnityEngine.SceneManagement;


public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    public DialogueTree tree;
    public DialogueNode currentNode;


    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;


    [Header("Manual Buttons")]
    [SerializeField] private GameObject choiceButton1;
    [SerializeField] private GameObject choiceButton2;
    [SerializeField] private GameObject skipButton;
    

    private Button button1;
    private TextMeshProUGUI button1_Text;
    private Button button2;
    private TextMeshProUGUI button2_Text;
    private Button skip;
    

    private Action onDialogueFinishedCallback;
    private List<string> currentChoiceKeys;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); }
        else { Instance = this; }

        string currentScene = SceneManager.GetActiveScene().name;
      

        if (currentScene == "EndingScene") // µ√«® Õ∫«Ë“™◊ËÕ©“°®∫µ√ß°—∫∑’Ë§ÿ≥µ—Èß‰«È‰À¡
        {
            LoadEndingConversation(); // ‚À≈¥∫∑©“°®∫
        }
        else
        {
            LoadConversation(); 
        }

        if (choiceButton1 != null)
        {
            button1 = choiceButton1.GetComponent<Button>();
            button1_Text = choiceButton1.GetComponentInChildren<TextMeshProUGUI>();
        }
        if (choiceButton2 != null)
        {
            button2 = choiceButton2.GetComponent<Button>();
            button2_Text = choiceButton2.GetComponentInChildren<TextMeshProUGUI>();
        }
        if (skipButton != null)
        {
            skip = skipButton.GetComponentInChildren<Button>();
        }

        dialoguePanel.SetActive(false);
    }
    private void LoadEndingConversation()
    {
        tree = new DialogueTree();    
       DialogueNode MomEndLine = new DialogueNode("Hmph. I thought you ainít got nothiní good in youÖ but that ass sure was fast. Next time you ainít goní be so lucky.", "Mom");

       DialogueNode SonReply = new DialogueNode("(Smile and wipe your sweat)", "Jeab");

        tree.rootNode = MomEndLine;

        MomEndLine.AddChoice("", SonReply);
    }

        private void LoadConversation()
    {
        //    Mom: Mhm.Look who finally showed up.
        //Son: UhÖ Ma, didnít you go get groceries?
        //Mom: Oh, I was gonna.But guess what? I ainít got no damn groceries.You know why?
        //Choices(1) -
        //        1.Son: ÖWhy ?
        //        2.Son : (Shake you head)
        //Mom: ëCause I decided Iíd go get ëem with you.So I called your bus driver ó see where you at ó and you know what he said?
        //Choices(2) -
        //        1.Son: ÖWhat he said ?
        //        2.Son : You - You were picking me up.
        //***Reply - 1.Mom: He said, ìHe told me, Iíll go get my own ass home.î You got anything you wanna say to me right now?

        //        2.Mom: You are damn right. I was picking you up? Yoí know your damn ass you lied. Now tell me where were you?
        //Son: Uh, Ió I wasó
        //Mom: Watch yoí mouth before you say somethiní stupid.Gimme one good reason I shouldnít take this slipper and bust it upside your head.
        //Choices -
        //        1.Son: Ió I was helpiní an old lady cross the street!
        //        2.Son: IÖ I was at the basketball court, playing with my friends.
        //Reply - 1.Mom: Two hours, huh? Two damn hours to cross a street? Boy, get your lyiníass over here!

        //        2.Mom: Oh boy, you did not just lie to me. You? Playing basketball? Get your damn ass over here right now
        //        (Son starts running)
        //Mom: Donít you run from me! Aunt Somsri said she saw your trifliní ass at the arcade!
        //An old lady crossiní the road? In the arcade? What road, huh? The one between the claw machine and the soda counter? You better bring your stupid ass back here before I show you what groceries feel like!

        tree = new DialogueTree();
        //∫∑∑”¥“
        DialogueNode MomLine1 = new DialogueNode("Mhm.Look who finally showed up.", "Mom");
        DialogueNode SonLine1 = new DialogueNode("UhÖ Ma, didnít you go get groceries?", "Jeab");
        DialogueNode SonLine2 = new DialogueNode(" Uh, I.. I was", "Jeab");
        DialogueNode MomLine2 = new DialogueNode("Donít you run from me! Aunt Somsri said she saw your trifliní ass at the arcade!", "Mom");
        DialogueNode MomLine3End = new DialogueNode("An old lady crossiní the road? In the arcade? What road, huh? The one between the claw machine and the soda counter? You better bring your stupid ass back here before I show you what groceries feel like!", "Mom");
        //∫∑∑’ËµÈÕßµÕ∫
        DialogueNode player_Reply1 = new DialogueNode("Oh, I was gonna.But guess what? I ainít got no damn groceries.You know why?", "Mom");
        DialogueNode player_Reply2 = new DialogueNode("Cause I decided Iíd go get ëem with you.So I called your bus driver ó see where you at ó and you know what he said?", "Mom");
        DialogueNode player_Reply3 = new DialogueNode("Watch yoí mouth before you say somethiní stupid.Gimme one good reason I shouldnít take this slipper and bust it upside your head. ", "Mom");
        
        //∂È“µÕ∫ 1 ·¡Ë®–µÕ∫
        DialogueNode MomLine_IfChoice1_1 = new DialogueNode("He said, ìHe told me, Iíll go get my own ass home.î You got anything you wanna say to me right now?", "Mom");
        DialogueNode MomLine_IfChoice1_2 = new DialogueNode("Two hours, huh? Two damn hours to cross a street? Boy, get your lyiníass over here!", "Mom");
        //∂È“µÕ∫ 2 ·¡Ë®–µÕ∫
        DialogueNode MomLine_IfChoice2_1 = new DialogueNode("You are damn right. I was picking you up? Yoí know your damn ass you lied. Now tell me where were you?", "Mom");
        DialogueNode MomLine_IfChoice2_2 = new DialogueNode("Oh boy, you did not just lie to me. You? Playing basketball? Get your damn ass over here right now", "Mom");

      
        
        tree.rootNode = MomLine1;
        // Mom 1 -> Son 1 -> player_r1 -> sonChoice1_r1 , -> player_r2 -> sonChoice_r1 -> MomLine_IfChoice1_1 -> SonLine2 -> player_r3 -> sonChoice1_r2 ->  MomLine_IfChoice1_2 -> MomLine2 -> MomLine3End
        //                                sonChoice2_r1                   sonChoice_r2 -> MomLine_IfChoice2_1                             sonChoice2_r2 ->  MomLine_IfChoice2_2
        MomLine1.AddChoice("", SonLine1);
        SonLine1.AddChoice("", player_Reply1);
        player_Reply1.AddChoice("ÖWhy ?", player_Reply2);
        player_Reply1.AddChoice("(Shake you head)",player_Reply2);
        player_Reply2.AddChoice("ÖWhat he said ?",MomLine_IfChoice1_1);
        player_Reply2.AddChoice("You - You were picking me up.", MomLine_IfChoice1_2);
        MomLine_IfChoice1_1.AddChoice("",SonLine2);
        MomLine_IfChoice1_2.AddChoice("",SonLine2);
        SonLine2.AddChoice("", player_Reply3);
        player_Reply3.AddChoice("I.. I was helpiní an old lady cross the street!",MomLine_IfChoice2_1);
        player_Reply3.AddChoice("I.. I was at the basketball court, playing with my friends.",MomLine_IfChoice2_2);
        MomLine_IfChoice2_1.AddChoice("",MomLine2);
        MomLine_IfChoice2_2.AddChoice("",MomLine2);
        MomLine2.AddChoice("", MomLine3End);




    }

    public void StartDialogue(Action onDialogueFinished)
    {
        this.onDialogueFinishedCallback = onDialogueFinished;
        currentNode = tree.rootNode;

        dialoguePanel.SetActive(true);
        ShowNode(currentNode);
    }

    private void ShowNode(DialogueNode node)
    {
        currentNode = node;
        speakerNameText.text = node.speakerName;
        dialogueText.text = node.dialogueText;

        button1.onClick.RemoveAllListeners();
        button2.onClick.RemoveAllListeners();
        skip.onClick.RemoveAllListeners();

        choiceButton1.SetActive(false);
        choiceButton2.SetActive(false);
        skipButton.SetActive(false);

        currentChoiceKeys = new List<string>(node.nexts.Keys);

        if (currentChoiceKeys.Count == 0)
        {
            skipButton.SetActive(true);
            skip.onClick.AddListener(() =>
            {
                EndDialogue();
            });
            
            //choiceButton1.SetActive(true); 
            //button1_Text.text = currentChoiceKeys[0];
            //button1.onClick.AddListener(() =>
            //{ 
            //    SelectChoice(0); 
            //});
        }
        else if (currentChoiceKeys.Count == 1)
        {
            skipButton.SetActive(true);
            skip.onClick.AddListener(() =>
            {
                SelectChoice(0);
            });
        }
        else
        {
            choiceButton1.SetActive(true);
            choiceButton2.SetActive(true);

            button1_Text.text = currentChoiceKeys[0];
            button1.onClick.AddListener(() => 
            {
                SelectChoice(0);
            });
            button2_Text.text = currentChoiceKeys[1];
            button2.onClick.AddListener(() =>
            {
                SelectChoice(1);
            });
        }


        //if (currentChoiceKeys.Count > 1)
        //{
        //    choiceButton2.SetActive(true);
        //    button2_Text.text = currentChoiceKeys[1]; 
        //    button2.onClick.AddListener(() => { 
        //        SelectChoice(1); 
        //    });
        //}
    }


    public void SelectChoice(int index)
    {
        if (index < 0 || index >= currentChoiceKeys.Count) return;
        string choiceKey = currentChoiceKeys[index];
        DialogueNode nextNode = currentNode.nexts[choiceKey];
        ShowNode(nextNode);
        //if (nextNode.nexts.Count == 0)
        //{
        //    ShowNode(nextNode);
        //    EndDialogue();
        //}
        //else
        //{
        //    ShowNode(nextNode);
        //}
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        if (onDialogueFinishedCallback != null)
        {
            onDialogueFinishedCallback.Invoke();
            onDialogueFinishedCallback = null;
        }
    }
}