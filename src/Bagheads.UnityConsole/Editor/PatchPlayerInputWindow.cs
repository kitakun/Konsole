using Bagheads.UnityConsole.Utils;
using Bagheads.UnityConsole.Utils.SimpleJSON;

using System;

using UnityEditor;

using UnityEngine;
#if KONSOLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Bagheads.UnityConsole.Editor
{
    public class PatchPlayerInputWindow : EditorWindow
    {
#if KONSOLE_INPUT_SYSTEM
        public InputActionAsset SelectedInputAction;
        private string _lastResult;

        [MenuItem("Window/Konsole+InputSystem")]
        public static void ShowWindow()
        {
            var window = GetWindow(typeof(PatchPlayerInputWindow), true, "Patch PlayerInput");
            window.minSize = new Vector2(420, 270);
        }

        protected void OnGUI()
        {
            var richTextStyle = new GUIStyle
            {
                richText = true
            };

            GUILayout.Label("<size=18>Tutorial for <b><color=#008080ff>PlayerInput</color></b> module</size>", richTextStyle);

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Simple injection without your involving is very difficult for <b>PlayerInput</b>.", richTextStyle);
            GUILayout.Label("So, please, select your control schema at the bottom and press <b>Patch</b>.", richTextStyle);
            GUILayout.Label("It will add one more action which will open/close console.", richTextStyle);
            GUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Asset");
            SelectedInputAction = EditorGUILayout.ObjectField(SelectedInputAction, typeof(InputActionAsset), true) as InputActionAsset;
            if (SelectedInputAction == null)
            {
                _lastResult = string.Empty;
            }

            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(_lastResult))
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label(_lastResult, richTextStyle);
                GUILayout.EndVertical();
            }

            GUILayout.FlexibleSpace();
            GUI.enabled = SelectedInputAction != null;
            EditorGUILayout.BeginHorizontal();
            string textToShowOnButton;
            if (SelectedInputAction != null)
            {
                if (SelectedInputAction.actionMaps.Count == 0)
                {
                    textToShowOnButton = "No any actions";
                    GUI.enabled = false;
                }
                else
                {
                    var initialAction = SelectedInputAction.actionMaps[0];
                    var foundedAction = initialAction.FindAction(PlayerInputUtils.KONSOLE_ACTION_NAME);
                    if (foundedAction == null)
                    {
                        textToShowOnButton = "Patch!";
                    }
                    else
                    {
                        GUI.enabled = false;
                        textToShowOnButton = "Already patched.";
                    }
                }
            }
            else
            {
                textToShowOnButton = "Patch!";
            }

            if (GUILayout.Button(textToShowOnButton, GUILayout.Height(30)))
            {
                try
                {
                    var path = AssetDatabase.GetAssetPath(SelectedInputAction);
                    var stringContent = System.IO.File.ReadAllText(path);
                    var rootObject = JSONNode.Parse(stringContent);
                    if (rootObject != null)
                    {
                        var mapsJObject = rootObject["maps"];
                        var mapsArray = mapsJObject.AsArray;

                        _lastResult = string.Empty;

                        var bindingNode = JSONNode.Parse("{\"name\": \"\", \"id\": \"c7b6e324-00e8-48a9-a886-b1c45b18d664\", \"path\": \"<Keyboard>/backquote\", \"interactions\": \"\", \"processors\": \"\", \"groups\": \"\", \"action\": \"ToggleConsole\", \"isComposite\": false, \"isPartOfComposite\": false }");
                        var actionNode = JSONNode.Parse("{ \"name\": \"ToggleConsole\", \"type\": \"Button\", \"id\": \"28f5f2bd-810d-4181-b392-7e2ad4866d1f\", \"expectedControlType\": \"Button\", \"processors\": \"\", \"interactions\": \"\", \"initialStateCheck\": false }");
                        var inConsoleMap = JSONNode.Parse(@"{
                            'name': 'Bagheads.Konsole',
                            'id': 'b9d19517-6e90-4f5f-bc3b-8eaa6cedfb3e',
                            'actions': [
                                {
                                    'name': 'ToggleConsole',
                                    'type': 'Button',
                                    'id': '28f5f2bd-810d-4181-b392-7e2ad4866d1f',
                                    'expectedControlType': 'Button',
                                    'processors': '',
                                    'interactions': '',
                                    'initialStateCheck': false
                                },
                                {
                                    'name': 'Tab',
                                    'type': 'Button',
                                    'id': 'e04ab66d-8c6e-4219-a2cf-81b38eaee429',
                                    'expectedControlType'): 'Button',
                                    'processors': '',
                                    'interactions': '',
                                    'initialStateCheck': false
                                },
                                {
                                    'name': 'SelectDirection',
                                    'type': 'Value',
                                    'id': '1dbaaa18-5492-4b12-b3df-cfa8d7dc1f0a',
                                    'expectedControlType': 'Axis',
                                    'processors': '',
                                    'interactions': '',
                                    'initialStateCheck': true
                                }
                            ],
                            'bindings': [
                                {
                                    'name': '',
                                    'id': 'c7b6e324-00e8-48a9-a886-b1c45b18d664',
                                    'path': '<Keyboard>/backquote',
                                    'interactions': '',
                                    'processors': '',
                                    'groups': 'IntegratedConsole',
                                    'action': 'ToggleConsole',
                                    'isComposite': false,
                                    'isPartOfComposite': false
                                },
                                {
                                    'name': '',
                                    'id': '4c31b343-1368-45e4-8192-2f96f464f943',
                                    'path': '<Keyboard>/tab',
                                    'interactions': '',
                                    'processors': '',
                                    'groups': '',
                                    'action': 'Tab',
                                    'isComposite': false,
                                    'isPartOfComposite': false
                                },
                                {
                                    'name': 'Arrows',
                                    'id': 'fd660627-1202-459f-9ba9-4726b593f6b2',
                                    'path': '1DAxis',
                                    'interactions': '',
                                    'processors': '',
                                    'groups': '',
                                    'action': 'SelectDirection',
                                    'isComposite': true,
                                    'isPartOfComposite': false
                                },
                                {
                                    'name': 'negative',
                                    'id': '327b8f91-c715-4e51-9d44-d5ae48dd5fa6',
                                    'path': '<Keyboard>/downArrow',
                                    'interactions': '',
                                    'processors': '',
                                    'groups': '',
                                    'action': 'SelectDirection',
                                    'isComposite': false,
                                    'isPartOfComposite': true
                                },
                                {
                                    'name': 'positive',
                                    'id': '950a169b-58cc-4c4b-9e95-48fa5296f955',
                                    'path': '<Keyboard>/upArrow',
                                    'interactions': '',
                                    'processors': '',
                                    'groups': '',
                                    'action': 'SelectDirection',
                                    'isComposite': false,
                                    'isPartOfComposite': true
                                }
                            ]
                        }".Replace('\'', '"'));

                        var needToInserConsoleMap = true;
                        
                        for (var i = 0; i < mapsArray.Count; i++)
                        {
                            if (mapsArray[i] is JSONObject mapObject)
                            {
                                if (mapObject["name"] == "Bagheads.Konsole")
                                {
                                    needToInserConsoleMap = false;
                                }
                                
                                // bindings
                                if (mapObject["bindings"] == null)
                                {
                                    // create from scratch
                                    var bindingsArray = new JSONArray();
                                    bindingsArray.Add(bindingNode);

                                    mapObject["bindings"] = new JSONArray();
                                    mapObject["bindings"].Add(bindingsArray);

                                    _lastResult = "Create binding and add Konsole binding";
                                }
                                else
                                {
                                    // add new if not exists
                                    var alreadyHasBinding = false;

                                    if (mapObject["bindings"] != null)
                                    {
                                        var bindingsArray = mapObject["bindings"].AsArray;
                                        for (var j = 0; j < bindingsArray.Count; j++)
                                        {
                                            if (bindingsArray[j] is JSONObject bindingObject && bindingObject["id"] == "c7b6e324-00e8-48a9-a886-b1c45b18d664")
                                            {
                                                alreadyHasBinding = true;
                                            }
                                        }
                                    }

                                    if (!alreadyHasBinding)
                                    {
                                        mapObject["bindings"].Add(bindingNode);
                                        _lastResult = "Added Konsole binding";
                                    }
                                }

                                // actions
                                if (mapObject["actions"] == null)
                                {
                                    // create from scratch
                                    var actionArray = new JSONArray();
                                    actionArray.Add(actionNode);

                                    mapObject["actions"] = new JSONArray();
                                    mapObject["actions"].Add(actionArray);

                                    _lastResult = string.IsNullOrEmpty(_lastResult)
                                        ? "Create actions and add Konsole actions"
                                        : $"{_lastResult}{Environment.NewLine}Create actions and add Konsole actions";
                                }
                                else
                                {
                                    // add new if not exist
                                    var alreadyHasAction = false;

                                    if (mapObject["actions"] != null)
                                    {
                                        var actionsArray = mapObject["actions"].AsArray;
                                        for (var j = 0; j < actionsArray.Count; j++)
                                        {
                                            if (actionsArray[j] is JSONObject bindingObject && bindingObject["name"] == "ToggleConsole")
                                            {
                                                alreadyHasAction = true;
                                            }
                                        }
                                    }

                                    if (!alreadyHasAction)
                                    {
                                        mapObject["actions"].Add(actionNode);
                                        _lastResult = string.IsNullOrEmpty(_lastResult)
                                            ? "Add Konsole actions"
                                            : $"{_lastResult}{Environment.NewLine}Add Konsole actions";
                                    }
                                }
                            }
                        }

                        if (needToInserConsoleMap)
                        {
                            mapsArray.Add(inConsoleMap);
                        }
                    }

                    // save previous version (in case of errors)
                    var previousFileVersion = path.Replace(System.IO.Path.GetFileNameWithoutExtension(path),
                        $"{System.IO.Path.GetFileNameWithoutExtension(path)}_BeforePatch");
                    if (System.IO.File.Exists(previousFileVersion))
                    {
                        System.IO.File.Delete(previousFileVersion);
                    }

                    System.IO.File.Copy(path, previousFileVersion);
                    _lastResult = string.IsNullOrEmpty(_lastResult) ? "Saved settings with _BeforePatch postfix" : $"{_lastResult}{Environment.NewLine}Saved settings with _BeforePatch postfix";

                    System.IO.File.WriteAllText(System.IO.Path.GetFullPath(path), rootObject.ToString());
                    AssetDatabase.Refresh();

                    _lastResult = string.IsNullOrEmpty(_lastResult) ? "<color=#00FF00><size=22>SUCCESS! </size></color>" : $"{_lastResult}{Environment.NewLine}<color=#00FF00><size=22>SUCCESS! </size></color>";
                }
                catch (Exception es)
                {
                    _lastResult = $"<color=#FF0000><size=22>ERROR!</size></color> {es.Message}.{Environment.NewLine}Please, check console logs.";
                    Debug.LogException(es);
                }
            }

            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;
        }
    }
#endif
}