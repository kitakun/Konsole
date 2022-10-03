using Konsole.Commands;
using Konsole.Utils;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Konsole.Components
{
    internal sealed class TypeaheadComponent : MonoBehaviour
    {
        public int AutocompleteLines { get; internal set; }
        public bool UseTMP { get; internal set; }
        public Font Font { get; internal set; }
        public int FontSize { get; internal set; }
        internal RectTransform InputTransform { get; set; }

        // internal state
        private readonly List<(RectTransform tra, Image background, MonoBehaviour textComponent)> _lines = new();
        private readonly List<ICommand> _typeaheadCommands = new();

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

                if (!UseTMP)
                {
                    const float LINE_HEIGHT = 26;
                    RectUtils.SetUseTopLine(helpLineTransform, new Vector2(0, LINE_HEIGHT));

                    if (GoUtils.TryAddDefaultTextAsChild(helpLineTransform, out var textComponent))
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
                    // TODO
                }
            }

            Typeahead(string.Empty);
        }

        public void Typeahead(string inputText)
        {
            var allCommands = Konsole.CommandsList;
            _typeaheadCommands.Clear();

            if (!string.IsNullOrEmpty(inputText))
            {
                for (var i = 0; i < allCommands.Count; i++)
                {
                    if (allCommands[i].Name.IndexOf(inputText, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        _typeaheadCommands.Add(allCommands[i]);
                        if (_typeaheadCommands.Count >= _lines.Count)
                            break;
                    }
                }
            }

            for (var i = 0; i < _lines.Count; i++)
            {
                var shouldBeVisible = _typeaheadCommands.Count > i;
                if (shouldBeVisible)
                {
                    switch (_lines[i].textComponent)
                    {
                        case Text defaultUnityText:
                            defaultUnityText.text = _typeaheadCommands[i].Name;
                            break;

                        default:
                            // TODO
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

        private void OnTypeaheadClicked(int index)
        {
            if (Konsole.ConsoleInstance != null)
            {
                Konsole.ConsoleInstance.InputField.text = _typeaheadCommands[index].Name;
                Konsole.ConsoleInstance.FocusInput();
                Typeahead(string.Empty);
            }
        }
    }
}