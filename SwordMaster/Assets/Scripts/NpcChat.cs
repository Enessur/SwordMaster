using System;
using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;
using UnityEngine.UI;

public class NpcChat : MonoBehaviour
{
    public Text npcChat;
    private string[] dialogueList;
    private bool _isPlayerDetected;
    public GameObject npcChatCanvas;
    private int dialogueIndex;

    private void Start()
    {
        dialogueIndex = 0;
        npcChat.text = "";
        _isPlayerDetected = false;
        npcChatCanvas.SetActive(false);
        dialogueList = new string[]
        {
            "Hi there ! Are you wondering where is everyone ?",
            "They were kidnapped by monsters.",
            "But you young man, I think you can save everyone!",
            "But first you need to find equipment from houses.",
            "When you are ready, defeat all the enemies and enter the cave.",
            "Good Luck.",
        };
    }

    private void Update()
    {
        Chat();
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            _isPlayerDetected = true;
            npcChatCanvas.SetActive(true);
        }

        else
        {
            _isPlayerDetected = false;
            npcChatCanvas.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        _isPlayerDetected = false;
        npcChatCanvas.SetActive(false);
    }

    private void Chat()
    {
        if (!_isPlayerDetected) return;
        npcChat.text = dialogueList[dialogueIndex];
        if (!Input.GetMouseButtonDown(0)) return;
        npcChat.text = dialogueList[dialogueIndex];
        dialogueIndex++;
        if (dialogueIndex >= dialogueList.Length)
        {
            dialogueIndex = 0;
        }
    }
}