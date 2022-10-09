using Bagheads.UnityConsole.Commands;
using Bagheads.UnityConsole.Data;
using Bagheads.UnityConsole.Utils;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bagheads.UnityConsole.Components
{
    public static partial class PreventFromEditor
    {
        [RequireComponent(typeof(CanvasGroup))]
        public class KonsoleComponent : MonoBehaviour
        {
            private const int MAX_LOGS_COUNT = 32;

            internal readonly List<string> ReusableListString = new(4);
            internal readonly List<RichParameter> ReusableRichParameters = new(4);
            internal readonly List<Component> InternalComponents = new(4);

            private CanvasGroup _canvas;
            private RectTransform _transform;

            private InputField _inputField;
            internal InputField InputField => _inputField;

#if KONSOLE_TEXT_MESH_PRO
            private TMPro.TMP_InputField _tmpText;
            internal TMPro.TMP_InputField TMP_InputField => _tmpText;
#endif

            // internal state
            private bool _shouldBeVisible;
            private float _showLerp;
            private float _frameHeight;
            private readonly Dictionary<RectTransform, (Image icon, MonoBehaviour textComponent)> _logs = new(MAX_LOGS_COUNT);
            private CommandContext _reusableCommandContext;
            private float _scrollToBottomAfter;
#if KONSOLE_INPUT_SYSTEM
            private string _inputActionBefore;
#endif

            // public properties
            public bool IsShouldBeVisible => _shouldBeVisible;

            // public state
            [Header("public state")]
            public Font DefaultFont;
#if KONSOLE_TEXT_MESH_PRO
            public TMPro.TMP_FontAsset TMpFontAsset;
#endif
            public int FontSize;
            public bool UseTMP;
            public bool LogTime;
            public ThemeOptions Theme;
            public bool UseNewInputSystem;

            // refs
            public RectTransform ScrollViewContentTra { get; internal set; }
            public Scrollbar VerticalScrollBar { get; internal set; }

            // unity methods

            protected void Awake()
            {
                _canvas = GetComponent<CanvasGroup>();
                _transform = (RectTransform) transform;

                _canvas.blocksRaycasts = false;
                _canvas.interactable = false;
                _canvas.alpha = 0;
                _shouldBeVisible = false;

                // default theme
                Theme.StatusError ??= new Color(0.7882353f, 0.1478235f, 0, 1);
                Theme.StatusWarning ??= new Color(0.7882354f, 0.5921569f, 0, 1);
                Theme.StatusLog ??= new Color(0.7216981f, 0.9891032f, 1, 1);
            }

            protected void Start()
            {
                _inputField = GetComponentInChildren<InputField>();
#if KONSOLE_TEXT_MESH_PRO
                _tmpText = GetComponentInChildren<TMPro.TMP_InputField>();
#endif
                _frameHeight = ((RectTransform) transform).sizeDelta.y;
                _reusableCommandContext = new CommandContext(this);

                ScrollViewContentTra.sizeDelta = Vector3.zero;
            }

            protected void OnEnable()
            {
                Application.logMessageReceived += OnLogMessageReceived;
            }

            protected void OnDisable()
            {
                Application.logMessageReceived -= OnLogMessageReceived;
                UnfocusInput();
            }

            protected void OnDestroy()
            {
                if (Konsole.ConsoleInstance == this)
                {
                    Konsole.ConsoleInstance = null;
                }
            }

            protected void Update()
            {
                Internal_ShowHideTick();
                Internal_ScrollTick();
                if (!UseNewInputSystem)
                {
                    Internal_ConsoleDefaultInput();
                }
            }

            private void Internal_ShowHideTick()
            {
                const float SHOW_HIDE_SPEED_MODIFIER = 3f;
                var updateGraphics = false;
                if (_shouldBeVisible && _showLerp < 1)
                {
                    _showLerp = Mathf.Clamp01(_showLerp + Time.unscaledDeltaTime * SHOW_HIDE_SPEED_MODIFIER);
                    updateGraphics = true;
                }
                else if (!_shouldBeVisible && _showLerp > 0)
                {
                    _showLerp = Mathf.Clamp01(_showLerp - Time.unscaledDeltaTime * SHOW_HIDE_SPEED_MODIFIER);
                    updateGraphics = true;

                    if (_canvas.interactable)
                    {
                        UnfocusInput();
                        _canvas.interactable = false;
                        _canvas.blocksRaycasts = false;
                    }
                }

                if (updateGraphics)
                {
                    _canvas.alpha = _showLerp;
                    _transform.anchoredPosition = new Vector2(0, (_frameHeight * 0.5f) * (1 - _showLerp));

                    TryScrollToBottom();

                    if (_showLerp == 1)
                    {
                        _canvas.blocksRaycasts = true;
                        _canvas.interactable = true;
                        FocusInput();
                    }
                }
            }

            private void Internal_ScrollTick()
            {
                if (_scrollToBottomAfter > 0)
                {
                    _scrollToBottomAfter -= Time.unscaledDeltaTime;
                    if (_scrollToBottomAfter < 0)
                    {
                        _scrollToBottomAfter = 0;
                        TryScrollToBottom(false);
                    }
                }
            }

            private void Internal_ConsoleDefaultInput()
            {
                if (Input.GetKeyDown(KeyCode.BackQuote))
                {
                    Konsole.ToggleConsole();
                }
            }

            // public methods

            public void ToggleConsole()
            {
                _shouldBeVisible = !_shouldBeVisible;
            }

            public void FocusInput()
            {
                if (_inputField != null)
                {
                    EventSystem.current.SetSelectedGameObject(_inputField.gameObject, null);
                    _inputField.ActivateInputField();
                }
#if KONSOLE_TEXT_MESH_PRO
                if (TMP_InputField != null)
                {
                    EventSystem.current.SetSelectedGameObject(TMP_InputField.gameObject, null);
                    TMP_InputField.ActivateInputField();
                }
#endif

#if KONSOLE_INPUT_SYSTEM
                if (TryGetInternalComponent<UnityEngine.InputSystem.PlayerInput>(out var playerInput)
                    && playerInput.currentActionMap.name != PlayerInputUtils.KONSOLE_ACTION_NAME)
                {
                    _inputActionBefore = playerInput.SwitchOn(PlayerInputUtils.KONSOLE_ACTION_NAME);
                }
#endif
            }

            public void Clear()
            {
                foreach (var logPair in _logs)
                {
                    Destroy(logPair.Key.gameObject);
                }

                _logs.Clear();

                ScrollViewContentTra.sizeDelta = Vector3.zero;
            }

            // log creation methods

            public void CommandResponse(ICommand caller, string response)
            {
                ReusableRichParameters.Clear();

                const string format = "{0}: {1}";
                ReusableRichParameters.Add(RichParameter.Bold());

                ReusableListString.Clear();
                ReusableListString.Add(caller.Name);
                ReusableListString.Add(response);

                Internal_WriteMessageToConsole(format, ReusableListString, ReusableRichParameters);
            }

            internal void OnSubmit(string data)
            {
                if (_inputField != null)
                {
                    _inputField.text = string.Empty;
                }
#if KONSOLE_TEXT_MESH_PRO
                if (TMP_InputField != null)
                {
                    TMP_InputField.text = string.Empty;
                }
#endif
                ReusableRichParameters.Clear();

                const string format = "{0}: {1}";
                ReusableRichParameters.Add(RichParameter.Bold());

                ReusableListString.Clear();

                if (!Konsole.TryGetCommandByName(data, out var command))
                {
                    ReusableListString.Add("Unknown Command!");
                    ReusableListString.Add(data);

                    Internal_WriteMessageToConsole(format, ReusableListString, ReusableRichParameters);
                }
                else
                {
                    _reusableCommandContext.Set(command, StringUtils.GetParametersFromCommand(data));
                    command.Launch(_reusableCommandContext);
                    TryScrollToBottom();
                }

                FocusInput();
            }

            private void OnLogMessageReceived(string message, string stackTrace, LogType type)
            {
                ReusableRichParameters.Clear();

                const string format = "[{0}] : {1}";
                ReusableRichParameters.Add(RichParameter.Bold());

                ReusableListString.Clear();
                ReusableListString.Add(TextTags.WithColor(type, Enum.GetName(typeof(LogType), type)));
                ReusableListString.Add(message);

                Internal_WriteMessageToConsole(format, ReusableListString, ReusableRichParameters);
            }

            // private 

            private void TryScrollToBottom(bool defaultCall = true)
            {
                if (VerticalScrollBar != null)
                {
                    VerticalScrollBar.value = 0;
                }

                if (defaultCall)
                {
                    _scrollToBottomAfter = 0.1f;
                }
            }

            internal void Internal_WriteMessageToConsole(string format, IReadOnlyList<string> parameters, List<RichParameter> richParameters = null)
            {
                if (_logs.Count < MAX_LOGS_COUNT)
                {
                    // add new log
                    var logRecordTra = GoUtils.CreateRectGameObject($"LogRecord_{_logs.Count}", ScrollViewContentTra);
                    const float LINE_HEIGHT = 26;
                    RectUtils.SetUseTopLine(logRecordTra, new Vector2(0, LINE_HEIGHT));

                    if (!UseTMP)
                    {
                        // default unity3d text
                        var logText = logRecordTra.gameObject.AddComponent<Text>();
                        logText.font = DefaultFont;
                        logText.text = logText.supportRichText
                            ? StringUtils.BuildRich(format, parameters, richParameters, LogTime)
                            : StringUtils.Build(format, parameters, LogTime);
                        logText.raycastTarget = false;

                        if (FontSize > 0)
                        {
                            logText.fontSize = FontSize;
                        }

                        _logs.Add(logRecordTra, (null, logText));
                    }
                    else
                    {
#if KONSOLE_TEXT_MESH_PRO
                        var logText = logRecordTra.gameObject.AddComponent<TMPro.TextMeshProUGUI>();
                        logText.font = TMpFontAsset;
                        logText.text = logText.richText
                            ? StringUtils.BuildRich(format, parameters, richParameters, LogTime)
                            : StringUtils.Build(format, parameters, LogTime);
                        logText.raycastTarget = false;

                        if (FontSize > 0)
                        {
                            logText.fontSize = FontSize;
                        }

                        _logs.Add(logRecordTra, (null, logText));
#else
                        Debug.LogError($"Konsole.{nameof(Internal_WriteMessageToConsole)} - can't find TextMeshPRO package! Do you have it?");
#endif
                    }

                    if (ScrollViewContentTra != null)
                    {
                        ScrollViewContentTra.sizeDelta = new Vector2(0, (LINE_HEIGHT + 4) * _logs.Count);
                    }

                    TryScrollToBottom();
                }
                else
                {
                    var lastLog = (RectTransform) ScrollViewContentTra.GetChild(0);
                    lastLog.SetAsLastSibling();
                    if (!_logs.TryGetValue(lastLog, out var data))
                    {
                        Debug.LogError($"Konsole.{nameof(Internal_WriteMessageToConsole)} - got transform in content which is not log!");
                        return;
                    }

                    switch (data.textComponent)
                    {
                        case Text simpleText:
                            simpleText.text = simpleText.supportRichText
                                ? StringUtils.BuildRich(format, parameters, richParameters, LogTime)
                                : StringUtils.Build(format, parameters, LogTime);
                            TryScrollToBottom();
                            break;
#if KONSOLE_TEXT_MESH_PRO
                        case TMPro.TMP_Text textMeshText:
                            textMeshText.text = textMeshText.richText
                                ? StringUtils.BuildRich(format, parameters, richParameters, LogTime)
                                : StringUtils.Build(format, parameters, LogTime);
                            break;
#endif

                        default:
                            Debug.LogError($"Konsole.{nameof(Internal_WriteMessageToConsole)} - type of text ({data.textComponent.GetType().Name}) not implemented.");
                            break;
                    }
                }
            }

            internal void UnfocusInput()
            {
                if (InputField != null)
                {
                    InputField.OnDeselect(new BaseEventData(EventSystem.current));
                }
#if KONSOLE_TEXT_MESH_PRO
                if (TMP_InputField != null)
                {
                    TMP_InputField.OnDeselect(new BaseEventData(EventSystem.current));
                }
#endif
#if KONSOLE_INPUT_SYSTEM
                if (TryGetInternalComponent<UnityEngine.InputSystem.PlayerInput>(out var playerInput))
                {
                    if (playerInput != null && playerInput.currentActionMap != null && _inputActionBefore != PlayerInputUtils.KONSOLE_ACTION_NAME)
                    {
                        // move back
                        playerInput.SwitchOn(_inputActionBefore);
                        _inputActionBefore = string.Empty;
                    }
                    else if (playerInput != null && playerInput.currentActionMap is {name: PlayerInputUtils.KONSOLE_ACTION_NAME})
                    {
                        // switch on default
                        playerInput.SwitchOn(playerInput.defaultActionMap);
                        _inputActionBefore = string.Empty;
                    }
                }
#endif
            }

            internal bool TryGetInternalComponent<T>(out T component)
            {
                for (int i = 0; i < InternalComponents.Count; i++)
                {
                    if (InternalComponents[i] is T tRef)
                    {
                        component = tRef;
                        return true;
                    }
                }

                component = default;
                return false;
            }
        }
    }
}