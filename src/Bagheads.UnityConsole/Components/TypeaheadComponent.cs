using Bagheads.UnityConsole;
using Bagheads.UnityConsole.Commands;
using Bagheads.UnityConsole.Utils;

using System;
using System.Collections.Generic;

#if KONSOLE_TEXT_MESH_PRO
using TMPro;
#endif

using UnityEngine;
using UnityEngine.UI;

namespace Bahgeads.UnityConsole.Components
{
    internal sealed class TypeaheadComponent : MonoBehaviour
    {
        public int AutocompleteLines { get; internal set; }
        public bool UseTMP { get; internal set; }
        public Font Font { get; internal set; }
#if KONSOLE_TEXT_MESH_PRO
        public TMPro.TMP_FontAsset TmpFont { get; internal set; }
#endif
        public int FontSize { get; internal set; }
        internal RectTransform InputTransform { get; set; }

        // internal state
        private readonly List<(RectTransform tra, Image background, MonoBehaviour textComponent)> _lines = new();
        private readonly List<ICommand> _typeaheadCommands = new();

        // selections
        private int _selectionIndex = -1;
        private int _typeaheadViewOffset = 0;
        private bool _ignoreNextChangeEvent;

        // unity

        private void Start()
        {
            var helpLineColor = new Color(0.9f, 0.9f, 0.9f, 0.9f);
            for (var i = 0; i < AutocompleteLines; i++)
            {
                var helpLineTransform = GoUtils.CreateRectGameObject($"typeahead_{i}", InputTransform);
                var background = helpLineTransform.gameObject.AddComponent<Image>();
                background.color = helpLineColor;

                var button = helpLineTransform.gameObject.AddComponent<Button>();
                button.targetGraphic = background;
                var buttonIndex = i;
                button.onClick.AddListener(() => OnTypeaheadClicked(buttonIndex));

                const float LINE_HEIGHT = 26;
                RectUtils.SetUseTopLine(helpLineTransform, new Vector2(0, LINE_HEIGHT));

                if (!UseTMP)
                {
                    if (GoUtils.TryAddComponentAsChild<Text>(helpLineTransform, out var textComponent))
                    {
                        textComponent.font = Font;
                        textComponent.color = Color.black;
                        textComponent.fontSize = FontSize;

                        RectUtils.SetUseAllSpace(textComponent.rectTransform, new Vector2(-8, 0), new Vector2(0, -4));
                        helpLineTransform.anchoredPosition = new Vector2(0, -(LINE_HEIGHT + 1) * (i + 1));

                        _lines.Add((helpLineTransform, background, textComponent));
                    }
                    else
                    {
                        Debug.LogWarning($"Konsole.{nameof(TypeaheadComponent)} - can't add default unity text to dropdown element. It's a bug, sorry for this :c", this);
                    }
                }
                else
                {
#if KONSOLE_TEXT_MESH_PRO
                    if (GoUtils.TryAddComponentAsChild<TMPro.TextMeshProUGUI>(helpLineTransform, out var textComponent))
                    {
                        textComponent.font = TmpFont;
                        textComponent.color = Color.black;
                        textComponent.fontSize = FontSize;
                        textComponent.autoSizeTextContainer = true;

                        RectUtils.SetUseAllSpace(textComponent.rectTransform, new Vector2(-8, 0), new Vector2(0, -4));
                        helpLineTransform.anchoredPosition = new Vector2(0, -(LINE_HEIGHT + 1) * (i + 1));

                        _lines.Add((helpLineTransform, background, textComponent));
                    }
                    else
                    {
                        Debug.LogWarning($"Konsole.{nameof(TypeaheadComponent)} - can't add default unity text to dropdown element. It's a bug, sorry for this :c", this);
                    }
#else
                    Debug.LogError($"Konsole.{nameof(TypeaheadComponent)} - can't find TMP Pro Text component. Please disable TMP in options", this);
#endif
                }
            }

            Typeahead(string.Empty);
        }

        // public

        public void Typeahead(string inputText)
        {
            if (_ignoreNextChangeEvent)
            {
                _ignoreNextChangeEvent = false;
                return;
            }

            _selectionIndex = -1;
            HighlightHoveredTypeahead(_selectionIndex);
            _typeaheadViewOffset = 0;
            var allCommands = Konsole.CommandsList;
            _typeaheadCommands.Clear();

            if (!string.IsNullOrEmpty(inputText))
            {
                for (var i = 0; i < allCommands.Count; i++)
                {
                    if (allCommands[i].Name.IndexOf(inputText, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        _typeaheadCommands.Add(allCommands[i]);
                    }
                }
            }

            ShowTypeaheadResults(0);
        }

        // internals

        private void OnTypeaheadClicked(int index)
        {
            if (Konsole.ConsoleInstance != null)
            {
                SetInputText(_typeaheadCommands[index].Name);

                Konsole.ConsoleInstance.FocusInput();
                Typeahead(string.Empty);
            }
        }

        private void SetInputText(string text)
        {
            if (Konsole.ConsoleInstance.InputField != null)
            {
                Konsole.ConsoleInstance.InputField.text = text;
                Konsole.ConsoleInstance.InputField.MoveTextEnd(false);
            }

#if KONSOLE_TEXT_MESH_PRO
            if (Konsole.ConsoleInstance.TMP_InputField != null)
            {
                Konsole.ConsoleInstance.TMP_InputField.text = text;
                Konsole.ConsoleInstance.TMP_InputField.caretPosition = text.Length;
            }
#endif
        }

        private void HighlightHoveredTypeahead(int selectedIndex)
        {
            for (var i = 0; i < _lines.Count; i++)
            {
                var actualIndex = i;
                switch (_lines[i].textComponent)
                {
                    case Text defaultUnityText:
                        defaultUnityText.fontStyle = selectedIndex == actualIndex
                            ? FontStyle.Bold
                            : FontStyle.Normal;
                        break;

#if KONSOLE_TEXT_MESH_PRO
                    case TMPro.TextMeshProUGUI tmpText:
                        tmpText.fontStyle = selectedIndex == actualIndex
                            ? FontStyles.Bold
                            : FontStyles.Normal;
                        break;
#endif

                    default:
#if KONSOLE_DEVELOPMENT
                        Debug.LogError($"Type {_lines[i].textComponent.GetType().Name} not implemented", this);
#endif
                        break;
                }
            }
        }

        private void ShowTypeaheadResults(int selectionOffset)
        {
            for (var i = 0; i < _lines.Count; i++)
            {
                var actualIndex = i + selectionOffset;
                var shouldBeVisible = _typeaheadCommands.Count > i;
                if (shouldBeVisible)
                {
                    switch (_lines[i].textComponent)
                    {
                        case Text defaultUnityText:
                            defaultUnityText.text = _typeaheadCommands[actualIndex].Name;
                            break;

#if KONSOLE_TEXT_MESH_PRO
                        case TMPro.TextMeshProUGUI tmpText:
                            tmpText.text = _typeaheadCommands[actualIndex].Name;
                            break;
#endif

                        default:
#if KONSOLE_DEVELOPMENT
                        Debug.LogError($"Type {_lines[i].textComponent.GetType().Name} not implemented", this);
#endif
                            break;
                    }
                }
                else
                {
                    // hide
                }

                _lines[i].background.enabled = shouldBeVisible;
                _lines[i].textComponent.enabled = shouldBeVisible;
            }
        }

        /// <summary>Move cursor down</summary>
        internal void OnInput_Tab()
        {
            OnInput_Direction(true);
        }

        /// <summary>
        /// Move cursor down and up with offset
        /// </summary>
        /// <param name="isDown">cursor direction</param>
        internal void OnInput_Direction(bool isDown)
        {
            if (_typeaheadCommands.Count > 0)
            {
                if (isDown)
                {
                    if (_typeaheadCommands.Count > _selectionIndex + 1)
                    {
                        _selectionIndex = Mathf.Clamp(_selectionIndex + 1, 0, _typeaheadCommands.Count);
                        if (_selectionIndex + _typeaheadViewOffset >= _lines.Count - 1
                            && _lines.Count + _typeaheadViewOffset < _typeaheadCommands.Count)
                        {
                            _typeaheadViewOffset = Mathf.Clamp(_typeaheadViewOffset + 1, 0, _typeaheadCommands.Count);
                        }

                        _ignoreNextChangeEvent = true;
                        SetInputText(_typeaheadCommands[_selectionIndex].Name);
                        HighlightHoveredTypeahead(_selectionIndex - _typeaheadViewOffset);
                        ShowTypeaheadResults(_typeaheadViewOffset);
                    }
                }
                else
                {
                    if (_selectionIndex > 0)
                    {
                        _selectionIndex = Mathf.Clamp(_selectionIndex - 1, 0, _typeaheadCommands.Count);
                        if (_selectionIndex <= _typeaheadViewOffset)
                        {
                            _typeaheadViewOffset = Mathf.Clamp(_typeaheadViewOffset - 1, 0, _typeaheadCommands.Count);
                        }

                        _ignoreNextChangeEvent = true;
                        SetInputText(_typeaheadCommands[_selectionIndex].Name);
                        HighlightHoveredTypeahead(_selectionIndex - _typeaheadViewOffset);
                        ShowTypeaheadResults(_typeaheadViewOffset);
                    }
                }
            }
        }
    }
}