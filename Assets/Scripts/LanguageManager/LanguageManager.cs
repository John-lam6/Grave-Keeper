using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public LanguageSO currentLanguage;

    public void SetLanguage(LanguageSO language) {
        currentLanguage = language;
    }
}