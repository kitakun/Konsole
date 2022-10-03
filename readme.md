# Konsole

## Easy solution for ingame integration

Benefits:
 - no prefabs
 - single line of code
 - easy custom commands with arguments integration

## How to include:

In any place, in any method
 ```cs
protected void Awake()
{
    Konsole.Konsole.IntegrateInExistingCanvas();
}
 ```

 If you need change colors or other settings:

```cs
protected void Awake()
{
    Konsole.Konsole.IntegrateInExistingCanvas(new IntegrationOptions
    {
        DefaultTextFont = m_defaultFont, // here you can set custom font
        FontSize = m_fontSize, // here you can adjust font size
        AutoCompleteLines = 3, // default=3, will be shown under input field as auto-complete
        WriteLogTime = true, // default=true, show logged time in console
        Theme = new ThemeOptions // here you can adjust different status colors
        {
            StatusError = Color.red,
            StatusLog = Color.blue,
            StatusWarning = Color.yellow
        },
        // not implemented features:
        UseNewInputSystem = false, // integrate with Unity.InputSystem
        UseTextMeshPro = false, // use TextMeshPro instead default unity text component
        NewInputSystemToggleAction = "ToggleConsole", // will catch action to toggle console
    });
}
```

## commands

### Variant 1 - Anonymouse registration

```cs
Konsole.Konsole.RegisterCommand("quit", (_) =>
{
    // Here could be your code for this command
    Application.Quit();
});
```

If you need to log something back or work with arguments you can do the following:
```cs
Konsole.Konsole.RegisterCommand("ping", (context) =>
{
    context.Log($"Pong");
});
```


## Supports:
- ✔️ - Default unity text
- ☐  -  TMP Text support
- ☐  - Multilne in logs
- ☐  - Support for all Unity.InputSystem events
