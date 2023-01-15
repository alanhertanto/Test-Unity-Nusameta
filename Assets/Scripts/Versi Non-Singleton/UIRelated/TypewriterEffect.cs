using System;
using System.Collections;
using System.Text;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    public float delay = 0.1f;
    public string fullText;

    public TextMeshProUGUI textComponent;
    private string currentText = "";

    private void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        fullText = textComponent.text;
    }
    
    public IEnumerator ShowText()
    {
        StringBuilder textBuilder = new StringBuilder();
        while (true)
        {
            for (int i = 0; i < fullText.Length; i++)
            {
                textBuilder.Append(fullText[i]);
                textComponent.text = textBuilder.ToString();
                yield return new WaitForSeconds(delay);
            }
            textBuilder.Clear();
        }
    }
}