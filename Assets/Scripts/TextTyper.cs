using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextTyper : MonoBehaviour
{
    public GameObject panelObject;
    public GameObject textWindowMask;
    public TextMeshProUGUI textMeshPro;
    bool blinkyBoolFlag;
    bool isTyping;
    int counter;
    int totalVisableCharacters;
    int lengthOfTextHolderList;
    int textHolderListIndex;
    public TextHolder textHolder;
    string promptString;
    public Coroutine CORBlinkyThing;
    public float previousTextYSize;
    Vector2 textYBounds;
    public void Start()
    {
        textMeshPro = gameObject.GetComponent<TextMeshProUGUI>() ?? gameObject.AddComponent<TextMeshProUGUI>();

        promptString = "cmd.tapScreen";
        counter = 0;
        textHolder = textHolder.GetComponent<TextHolder>();
        lengthOfTextHolderList = textHolder.narrativeParagraphList.Count;
        textHolderListIndex = 0;
        textMeshPro.maxVisibleCharacters = 0;
        blinkyBoolFlag = false;
        isTyping = false;
        previousTextYSize = 0;

        SetUpTextPerimeters();
        EnablePromptString();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isTyping)
            {
                isTyping = true;
                AddParagraphToTextFeed();
                StartCoroutine(Type());
            } else if (isTyping)
            {
                counter = totalVisableCharacters;
            }
        }
    }

    public IEnumerator Type()
    {
        while (isTyping)
        {
            textMeshPro.maxVisibleCharacters = counter;

            counter++;

            if (counter >= totalVisableCharacters + 1)
            {
                isTyping = false;
                FinishTyping();
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    public void FinishTyping()
    {
        isTyping = false;
        EnablePromptString();
    }

    public void AddParagraphToTextFeed()
    {
        DisablePromptString();
        textMeshPro.text += textHolder.narrativeParagraphList[textHolderListIndex];
        textMeshPro.text += "\n\n";
        textMeshPro.ForceMeshUpdate();
        totalVisableCharacters = textMeshPro.textInfo.characterCount;
        textHolderListIndex++;
        if (textHolderListIndex > textHolder.narrativeParagraphList.Count - 1)
        {
            textHolderListIndex = 0;
        }
        var height = textMeshPro.textBounds.size.y;
        var displayHeightMax = textWindowMask.GetComponent<RectTransform>().sizeDelta.y;
        if (height > displayHeightMax)
        {
            if (height > previousTextYSize)
            {
                var yPOStoUIHeightRatio = textMeshPro.GetComponent<RectTransform>().position.y / height;
                var heightGap = height - previousTextYSize;
                var yPOSAdjust = heightGap * yPOStoUIHeightRatio;
                gameObject.GetComponent<RectTransform>().position = new Vector3(textMeshPro.GetComponent<RectTransform>().position.x, textMeshPro.GetComponent<RectTransform>().position.y + yPOSAdjust, 0);
                gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x, gameObject.GetComponent<RectTransform>().sizeDelta.y + heightGap);
            }
        }
        previousTextYSize = height;
    }

    public void EnablePromptString()
    {
        textMeshPro.text += promptString;
        textMeshPro.maxVisibleCharacters += promptString.Length;
        textMeshPro.ForceMeshUpdate();
        CORBlinkyThing = StartCoroutine(BlinkyInputThing());
    }

    public void DisablePromptString()
    {
        StopCoroutine(CORBlinkyThing);
        if (blinkyBoolFlag)
        {
            textMeshPro.text = textMeshPro.text.Substring(0, textMeshPro.text.Length - 1);
            textMeshPro.maxVisibleCharacters--;
            blinkyBoolFlag = false;
        }
        textMeshPro.text = textMeshPro.text.Substring(0, textMeshPro.text.Length - promptString.Length);
        textMeshPro.maxVisibleCharacters -= promptString.Length;
        textMeshPro.ForceMeshUpdate();
    }

    public IEnumerator BlinkyInputThing()
    {
        while (true)
        {
            if (!blinkyBoolFlag)
            {
                blinkyBoolFlag = true;
                textMeshPro.text += "_";
                textMeshPro.maxVisibleCharacters++;
            } else
            {
                blinkyBoolFlag = false;
                textMeshPro.text = textMeshPro.text.Substring(0, textMeshPro.text.Length - 1);
                textMeshPro.maxVisibleCharacters--;
            }
            textMeshPro.ForceMeshUpdate();
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void SetUpTextPerimeters()
    {
        var bounderTransform = panelObject.GetComponent<RectTransform>();
        var gap = 32;
        var bounderWidth = bounderTransform.rect.width;
        var bounderHeight = bounderTransform.rect.height;
        textWindowMask.GetComponent<RectTransform>().position = new Vector3(bounderTransform.position.x, bounderTransform.position.y, 0);
        textWindowMask.GetComponent<RectTransform>().sizeDelta = new Vector2(bounderWidth - gap, bounderHeight - gap);
        gameObject.GetComponent<RectTransform>().position = new Vector3(bounderTransform.position.x, bounderTransform.position.y, 0);
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(bounderWidth-gap, bounderHeight-gap);
    }
}
