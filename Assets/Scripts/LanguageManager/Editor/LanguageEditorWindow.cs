using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class LanguageEditorWindow : EditorWindow
{
    [SerializeField]
    private List<LanguageSO> languages = new List<LanguageSO>();
    private Vector2 scrollPos;
    private string newKey = "";

    [MenuItem("Window/Language Editor")]
    public static void ShowWindow() {
        GetWindow<LanguageEditorWindow>("Language Editor");
    }

    private void OnEnable() {
        LoadAllLanguages();
    }

    private void LoadAllLanguages() {
        if (languages.Count == 0) {
            string[] guids = AssetDatabase.FindAssets("t:LanguageSO");

            foreach (string guid in guids) {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                LanguageSO lang = AssetDatabase.LoadAssetAtPath<LanguageSO>(path);
                languages.Add(lang);
            }
        }
    }

    private void OnGUI() {
        // refresh languages
        if (GUILayout.Button("Refresh Languages")) {
            LoadAllLanguages();
        }
        
        GUILayout.Space(10);

        // add new key
        EditorGUILayout.BeginHorizontal();
        newKey = EditorGUILayout.TextField("New Key:", newKey);
        if (GUILayout.Button("Add Key"))
        {
            AddKeyToAllLanguages(newKey);
            newKey = "";
        }
        EditorGUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        bool removeKey = false;
        string keyToRemove = "";
        
        // go through each language to display the keys
        foreach (var lang in languages) {
            if (lang == null) continue;

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(lang.name, EditorStyles.boldLabel);
            var keys = lang.languageDictionary.Keys.ToList();
            
            // go through each key in the language
            foreach (var key in keys) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(key.ToString(), GUILayout.Width (150));

                string currentValue = lang.languageDictionary[key];
                string newValue = EditorGUILayout.TextField(currentValue);

                // if value has changed (text field has been edited)
                if (currentValue != newValue) {
                    Undo.RecordObject(lang, "Edit Language Value"); // save this as an undo step so it can be undone any time
                    lang.languageDictionary[key] = newValue;
                    EditorUtility.SetDirty(lang);
                }

                // Delete button
                if (GUILayout.Button("X", GUILayout.Width(25))) {
                    removeKey = true;
                    keyToRemove = key;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (removeKey) {
                RemoveKeyFromAllLanguages(keyToRemove);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();
    }
    
    private void AddKeyToAllLanguages(string newKey) {
        if (string.IsNullOrEmpty(newKey)) return;

        // add the key to each language
        foreach (var lang in languages) {
            if (lang == null) continue;

            if (!lang.languageDictionary.ContainsKey(newKey)) {
                Undo.RecordObject(lang, "Add Language Key");
                lang.languageDictionary[newKey] = "";
                EditorUtility.SetDirty(lang);
            }
        }
    }

    private void RemoveKeyFromAllLanguages(string key) {
        if (string.IsNullOrEmpty(key)) return;
        
        // remove the key from all languages
        foreach (var lang in languages) {
            if (lang == null) continue;

            if (lang.languageDictionary.ContainsKey(key)) {
                Undo.RecordObject(lang, "Remove Language Key");
                lang.languageDictionary.Remove(key);
                EditorUtility.SetDirty(lang);
            }
        }
    }
}
