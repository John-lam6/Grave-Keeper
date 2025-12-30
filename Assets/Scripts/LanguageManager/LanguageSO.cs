using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Language", order = 1)]
public class LanguageSO : ScriptableObject {
    
    public StringStringDictionary languageDictionary;

    private void OnEnable() {
        if (languageDictionary == null) {
            languageDictionary = new StringStringDictionary();
        }
    }
    
    public string GetValue(string key) {
        // if the key exists in the dictionary, return it
        if (languageDictionary.ContainsKey(key)) {
            return languageDictionary[key];
        }
        
        Debug.Log($"Language dictionary does not contain this key '{key}'");
        return null;
    }
}