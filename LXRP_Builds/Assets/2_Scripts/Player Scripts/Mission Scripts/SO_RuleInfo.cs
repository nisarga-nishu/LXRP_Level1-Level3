using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rule 1", menuName = "ScriptableObjects/RuleInfo", order = 1)]
public class SO_RuleInfo : ScriptableObject
{
    public int ruleNo;
    public string ruleText;
    public bool isCorrect;
    public string CorrectText;
    public bool answerIsTrue;

    private bool isSelected = false;
    public bool IsSelected { get => isSelected; set => isSelected = value; }
}
