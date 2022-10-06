using Bagheads.UnityConsole.Commands;
using Bagheads.UnityConsole.Components;
using Bagheads.UnityConsole.Data;
using Bagheads.UnityConsole.Utils;

using Bahgeads.UnityConsole.Components;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
#if KONSOLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Bagheads.UnityConsole
{
    [UnityEditor.InitializeOnLoadAttribute]
    public static class Konsole
    {
        private const float CONSOLE_HEIGHT = 300f;
        private const float INPUT_HEIGHT = 28;
        private const int DEFAULT_FONT_SIZE = 14;

        public static PreventFromEditor.KonsoleComponent ConsoleInstance { get; internal set; }

        public static List<ICommand> CommandsList { get; } = new()
        {
            new Command_Help(),
            new Command_HelloWorld(),
            new Command_Clear(),
            new Command_Quit(),
            new Command_setHeight(),
        };

        /// <summary>
        /// Register Anonymous command
        /// </summary>
        /// <param name="name">Command name</param>
        /// <param name="description">Command description (for Help method)</param>
        /// <param name="action">Command action itself</param>
        public static void RegisterCommand(string name, string description, Action<CommandContext> action)
        {
            if (action == null)
            {
                Debug.LogError($"Konsole.{nameof(RegisterCommand)} - parameter:{nameof(action)} can't be empty!");
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError($"Konsole.{nameof(RegisterCommand)} - parameter:{nameof(name)} can't be empty!");
                return;
            }

            CommandsList.Add(new Command_Anonymous(name, action, description));
        }

        /// <summary>
        /// Register Anonymous command
        /// </summary>
        /// <param name="name">Command name</param>
        /// <param name="action">Command action itself</param>
        public static void RegisterCommand(string name, Action<CommandContext> action)
        {
            if (action == null)
            {
                Debug.LogError($"Konsole.{nameof(RegisterCommand)} - parameter:{nameof(action)} can't be empty!");
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError($"Konsole.{nameof(RegisterCommand)} - parameter:{nameof(name)} can't be empty!");
                return;
            }

            CommandsList.Add(new Command_Anonymous(name, action));
        }

        /// <summary>
        /// Make console visible or hide
        /// </summary>
        public static void ToggleConsole()
        {
            if (ConsoleInstance != null)
            {
                ConsoleInstance.ToggleConsole();
            }
            else
            {
                Debug.LogWarning($"Konsole.{nameof(ToggleConsole)} - there is no injected console in scene. Please call {nameof(IntegrateInExistingCanvas)} method before using it");
            }
        }

        /// <summary>
        /// Create Console object in scene
        /// </summary>
        /// <param name="options">Creation parameters</param>
        public static void IntegrateInExistingCanvas(IntegrationOptions options = default)
        {
            if (ConsoleInstance != null)
            {
                Debug.LogWarning($"Console already exists! Skip...", ConsoleInstance);
                return;
            }

            if (!GoUtils.TryFindComponentInActiveScene(out Canvas existingCanvas))
            {
                Debug.LogError($"Konsole.{nameof(IntegrateInExistingCanvas)} - Can't find <{nameof(Canvas)}> in ActiveScene!");
                return;
            }

#if KONSOLE_TEXT_MESH_PRO
            if (options is {UseTextMeshPro: true} && options.TMpFontAsset == null)
            {
                if (options.DefaultTextFont == null)
                {
                    var fontPaths = Font.GetPathsToOSFonts();
                    var osFont = new Font(fontPaths[124]);
                    var fontAsset = TMPro.TMP_FontAsset.CreateFontAsset(osFont);
                    options.TMpFontAsset = fontAsset;
                }
                else
                {
                    var fontAsset = TMPro.TMP_FontAsset.CreateFontAsset(options.DefaultTextFont);
                    options.TMpFontAsset = fontAsset;
                }
            }
#endif

            Internal_IntegrateConsole((RectTransform) existingCanvas.transform, options);
        }

        /// <summary>
        /// Remove all anonymous commands. Could be useful if you have registered your commands inside changeable scene
        /// </summary>
        public static void RemoveAllAnonymousCommands()
        {
            for (var i = 0; i < CommandsList.Count; i++)
            {
                if (CommandsList[i] is Command_Anonymous)
                {
                    CommandsList.RemoveAt(i);
                    i--;
                }
            }
        }

        // internals

        static Konsole()
        {
            if (GoUtils.TryFindComponentInActiveScene<PreventFromEditor.KonsoleComponent>(out var existingConsole))
            {
                UnityEngine.Object.Destroy(existingConsole.gameObject);
                Debug.LogError($"Konsole.static - we don't support unity's hot-reloading and wish the same to you! otherwise you will face a lot of bugs, be care.");
            }
        }

        private static void Internal_IntegrateConsole(RectTransform parent, IntegrationOptions options)
        {
            var uiLayerMask = LayerMask.NameToLayer("UI");
            var consoleOwnerObject = new GameObject("[RootKonsole]", typeof(RectTransform), typeof(CanvasGroup), typeof(PreventFromEditor.KonsoleComponent))
            {
                layer = uiLayerMask
            };
            var ownerRectTransform = (RectTransform) consoleOwnerObject.transform;
            ownerRectTransform.SetParent(parent);

            // KonsoleComponent
            ConsoleInstance = consoleOwnerObject.GetComponent<PreventFromEditor.KonsoleComponent>();
            ConsoleInstance.UseTMP = options.UseTextMeshPro;
            ConsoleInstance.DefaultFont = options.DefaultTextFont != null
                ? options.DefaultTextFont
                : Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            ConsoleInstance.FontSize = options.FontSize > 0
                ? options.FontSize
                : DEFAULT_FONT_SIZE;
            ConsoleInstance.LogTime = options.WriteLogTime;
#if KONSOLE_INPUT_SYSTEM
            ConsoleInstance.UseNewInputSystem = options.UseNewInputSystem;
#endif

            if (options.Theme.StatusError.HasValue)
                ConsoleInstance.Theme.StatusError = options.Theme.StatusError.Value;
            if (options.Theme.StatusWarning.HasValue)
                ConsoleInstance.Theme.StatusWarning = options.Theme.StatusWarning.Value;
            if (options.Theme.StatusLog.HasValue)
                ConsoleInstance.Theme.StatusLog = options.Theme.StatusLog.Value;

#if KONSOLE_INPUT_SYSTEM
            if (options.UseNewInputSystem)
            {
                if (!GoUtils.TryFindComponentInActiveScene(out PlayerInput playerInput))
                {
                    Debug.LogError($"Konsole.{nameof(Internal_IntegrateConsole)} - Can't find <{nameof(PlayerInput)}> component!");
                    return;
                }

                switch (playerInput.notificationBehavior)
                {
                    case PlayerNotifications.SendMessages:
                    case PlayerNotifications.BroadcastMessages:
#if UNITY_EDITOR
                        Debug.LogWarning($"Konsole.{nameof(Internal_IntegrateConsole)} - you need to toggle console yourself by {nameof(Konsole)}.{nameof(ToggleConsole)}", ConsoleInstance);
#endif
                        break;

                    case PlayerNotifications.InvokeUnityEvents:
                    case PlayerNotifications.InvokeCSharpEvents:
                        foreach (var actionInMap in playerInput.currentActionMap.actions)
                        {
                            if (actionInMap.enabled
                                && actionInMap.name == options.NewInputSystemToggleAction)
                            {
                                // TODO
                                playerInput.onActionTriggered += context => { Debug.Log($"I see an action! {context.action.name}"); };
                                break;
                            }
                        }

                        break;

                    default:
                        Debug.LogError($"Konsole.{nameof(Internal_IntegrateConsole)} - Control behavior = {playerInput.notificationBehavior} not implemented!");
                        break;
                }
            }
#endif

            // positioning
            ownerRectTransform.pivot = new Vector2(0.5f, 1f);
            ownerRectTransform.anchorMin = new Vector2(0, 1);
            ownerRectTransform.anchorMax = new Vector2(1, 1);
            ownerRectTransform.localScale = Vector3.one;
            ownerRectTransform.sizeDelta = new Vector2(0f, CONSOLE_HEIGHT);
            ownerRectTransform.anchoredPosition = Vector3.zero;

            // graphics
            var backgroundImage = consoleOwnerObject.AddComponent<Image>();
            backgroundImage.color = new Color(0.5f, 0.5f, 0.5f, 0.6f);

            // input field
            var inputGo = new GameObject("Input_Konsole", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            var inputRect = (RectTransform) inputGo.transform;
            inputRect.SetParent(ownerRectTransform);

            inputRect.localScale = Vector3.one;

            // input field - positioning
            // no button, we do we need it?
            const float BUTTON_WIDTH = 0;
            RectUtils.SetUseBottomLine(inputRect, new Vector2(-BUTTON_WIDTH, INPUT_HEIGHT));

            var inputImage = inputGo.GetComponent<Image>();


#if KONSOLE_TEXT_MESH_PRO
            if (!options.UseTextMeshPro)
            {
#endif
                // default input
                var inputComponent = inputGo.AddComponent<InputField>();
                inputComponent.targetGraphic = inputImage;
                inputComponent.onSubmit.AddListener(ConsoleInstance.OnSubmit);

                if (options.AutoCompleteLines > 0)
                {
                    var typeahead = ConsoleInstance.gameObject.AddComponent<TypeaheadComponent>();
                    typeahead.AutocompleteLines = options.AutoCompleteLines;
                    typeahead.InputTransform = inputRect;
                    typeahead.UseTMP = options.UseTextMeshPro;
                    typeahead.Font = options.DefaultTextFont != null
                        ? options.DefaultTextFont
                        : ConsoleInstance.DefaultFont;
                    typeahead.FontSize = options.FontSize > 0
                        ? options.FontSize
                        : DEFAULT_FONT_SIZE;
                    inputComponent.onValueChanged.AddListener(typeahead.Typeahead);
                }

                // default unity engine text
                if (GoUtils.TryAddComponentAsChild<Text>(inputComponent.transform as RectTransform, out var textComponent))
                {
                    textComponent.font = options.DefaultTextFont != null
                        ? options.DefaultTextFont
                        : ConsoleInstance.DefaultFont;

                    textComponent.color = Color.black;
                    textComponent.supportRichText = false;

                    // input field - text - positioning
                    RectUtils.SetUseAllSpace(textComponent.rectTransform, new Vector2(-10, -10));
                    inputComponent.textComponent = textComponent;

                    ConsoleInstance.FocusInput();
                }
#if KONSOLE_TEXT_MESH_PRO
            }
            else
            {
                var inputComponent = inputGo.AddComponent<TMPro.TMP_InputField>();
                inputComponent.targetGraphic = inputImage;
                inputComponent.onSubmit.AddListener(ConsoleInstance.OnSubmit);

                if (options.AutoCompleteLines > 0)
                {
                    var typeahead = ConsoleInstance.gameObject.AddComponent<TypeaheadComponent>();
                    typeahead.AutocompleteLines = options.AutoCompleteLines;
                    typeahead.InputTransform = inputRect;
                    typeahead.UseTMP = options.UseTextMeshPro;
                    typeahead.Font = options.DefaultTextFont != null
                        ? options.DefaultTextFont
                        : ConsoleInstance.DefaultFont;
                    typeahead.FontSize = options.FontSize > 0
                        ? options.FontSize
                        : DEFAULT_FONT_SIZE;
                    typeahead.TmpFont = options.TMpFontAsset;
                    inputComponent.onValueChanged.AddListener(typeahead.Typeahead);
                }

                if (GoUtils.TryAddComponentAsChild<TMPro.TextMeshProUGUI>(inputComponent.transform as RectTransform, out var textComponent))
                {
                    textComponent.font = options.TMpFontAsset;
                    textComponent.fontSize = options.FontSize > 0
                        ? options.FontSize
                        : DEFAULT_FONT_SIZE;
                    textComponent.autoSizeTextContainer = true;
                    textComponent.color = Color.black;
                    textComponent.richText = false;

                    // input field - text - positioning
                    RectUtils.SetUseAllSpace(textComponent.rectTransform, new Vector2(-10, -10));
                    inputComponent.textComponent = textComponent;

                    ConsoleInstance.FocusInput();
                }
            }
#endif

            // scroll-rect
            var scrollRectGo = new GameObject("LogsScrollRect", typeof(RectTransform), typeof(ScrollRect), typeof(CanvasRenderer));
            var scrollRectTra = (RectTransform) scrollRectGo.transform;
            scrollRectTra.SetParent(ownerRectTransform);
            scrollRectTra.localScale = Vector3.one;
            RectUtils.SetUseAllSpace(scrollRectTra, new Vector2(0, -INPUT_HEIGHT), pivot: new Vector2(0.5f, 1f));
            var scrollRect = scrollRectGo.GetComponent<ScrollRect>();

            // scroll-rect-viewpoint
            var scrollRectViewpoint = new GameObject("Viewpoint", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Mask));
            var scrollRectViewpointTra = (RectTransform) scrollRectViewpoint.transform;
            scrollRectViewpointTra.SetParent(scrollRectTra);
            scrollRectViewpointTra.localScale = Vector3.one;
            RectUtils.SetUseAllSpace(scrollRectViewpointTra, new Vector2(-10, 0));
            var viewpointImageMask = scrollRectViewpoint.GetComponent<Mask>();
            viewpointImageMask.showMaskGraphic = false;

            // scroll-rect-viewpoint-content
            var scrollRectViewpointContent = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup));
            var scrollRectViewpointContentTra = (RectTransform) scrollRectViewpointContent.transform;
            scrollRectViewpointContentTra.SetParent(scrollRectViewpointTra);
            scrollRectViewpointContentTra.localScale = Vector3.one;
            RectUtils.SetUseTopLine(scrollRectViewpointContentTra, new Vector2(0, 600));
            var contentLayoutGroup = scrollRectViewpointContent.GetComponent<VerticalLayoutGroup>();
            contentLayoutGroup.childAlignment = TextAnchor.LowerCenter;
            contentLayoutGroup.childForceExpandWidth = true;
            contentLayoutGroup.childForceExpandHeight = false;
            contentLayoutGroup.childControlHeight = false;
            contentLayoutGroup.childControlWidth = true;
            contentLayoutGroup.spacing = 4;

            // scroll-rect-scrollbar
            var verticalScrollBarGo = new GameObject("VerticalScrollBar", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Scrollbar));
            var verticalScrollBarTra = (RectTransform) verticalScrollBarGo.transform;
            verticalScrollBarTra.SetParent(scrollRectTra);
            verticalScrollBarTra.localScale = Vector3.one;
            RectUtils.SetUseRightLine(verticalScrollBarTra, new Vector2(20, 0));
            var verticalScrollBar = verticalScrollBarGo.GetComponent<Scrollbar>();

            // scroll-rect-scrollbar-area
            var verticalScrollBarAreaGo = new GameObject("ScrollArea", typeof(RectTransform));
            var verticalScrollBarAreaTra = (RectTransform) verticalScrollBarAreaGo.transform;
            verticalScrollBarAreaTra.SetParent(verticalScrollBarTra);
            verticalScrollBarAreaTra.localScale = Vector3.one;
            RectUtils.SetUseAllSpace(verticalScrollBarAreaTra, new Vector2(-20, -20), Vector2.zero);

            // scroll-rect-scrollbar-area-handle
            var verticalScrollBarAreaHandleGo = new GameObject("Handle", typeof(RectTransform), typeof(Image));
            var verticalScrollBarAreaHandleTra = (RectTransform) verticalScrollBarAreaHandleGo.transform;
            verticalScrollBarAreaHandleTra.SetParent(verticalScrollBarAreaTra);
            verticalScrollBarAreaHandleTra.localScale = Vector3.one;
            RectUtils.SetUseAllSpace(verticalScrollBarAreaHandleTra, new Vector2(20, 20), Vector2.zero);
            var vscrollHandleImage = verticalScrollBarAreaHandleGo.GetComponent<Image>();
            vscrollHandleImage.color = Color.gray;
            verticalScrollBar.direction = Scrollbar.Direction.BottomToTop;
            verticalScrollBar.handleRect = verticalScrollBarAreaHandleTra;

            scrollRect.content = scrollRectViewpointContentTra;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.verticalScrollbar = verticalScrollBar;
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.viewport = scrollRectViewpointTra;
            scrollRect.CalculateLayoutInputVertical();

            ConsoleInstance.ScrollViewContentTra = scrollRectViewpointContentTra;
            ConsoleInstance.VerticalScrollBar = verticalScrollBar;
        }

        internal static bool TryGetCommandByName(string data, out ICommand command)
        {
            var commandInput = data.AsSpan();
            if (data.Contains(" "))
            {
                var indexOfSpace = data.IndexOf(" ", StringComparison.Ordinal);
                commandInput = data.AsSpan()[..indexOfSpace];
            }

            for (var i = 0; i < CommandsList.Count; i++)
            {
                if (CommandsList[i].Name.AsSpan().SequenceEqual(commandInput))
                {
                    // found our command
                    command = CommandsList[i];
                    return true;
                }
            }

            command = default;
            return false;
        }
    }
}