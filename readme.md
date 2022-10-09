# Konsole

## Easy solution for ingame integration

Benefits:
 - no prefabs
 - single line of code
 - easy custom commands with arguments integration

# How to install
Window - Package Manager - Add Package from Git URL - **`https://github.com/kitakun/Konsole.git`** - Add

# How to include:

 - You can add component `CreateKonsole` on any object, this script will call method `IntegrateInExistingCanvas` on start.
 
 ![Create Konsole (Script) in Unity](/docs/how_to_add_1.jpg)
___
 - In any place, in any method you can create console yourself
 ```cs
protected void Awake()
{
    Konsole.IntegrateInExistingCanvas();
}
 ```

# commands

### - Variant 1 - Anonymouse registration

```cs
Konsole.RegisterCommand("quit", _ =>
{
    // Here could be your code for this command
    Application.Quit();
});
```

If you need to log something back or work with arguments you can do the following:
```cs
Konsole.RegisterCommand("ping", context =>
{
    context.Log("Pong");
});
```
___
### - Variant 2 - Independent class
#### **`Command_Quit.cs`**
```cs
public class Command_Quit : ICommand
{
    public string Name => "quit";
    public string Description => "Will quit from the Application";
    
    public void Launch(CommandContext context)
    {
        Application.Quit();
    }
}
```
In any place:
#### **`MyGameManager.cs`**
```cs
protected void Start()
{
    Konsole.CommandsList.Add(new Command_Quit());
}
```

# All parameters

```cs
protected void Awake()
{
    Konsole.IntegrateInExistingCanvas(new IntegrationOptions
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


# Supports:
- ✔️ - Default unity text
- ❌ - Unity's hot-reloading
- ✔️  - TMP Text support
- ☐  - Multilne in logs
- ☐  - Support for Unity.InputSystem
    - ✔️ - InputActions patcher
    - ✔️ - Input Events - SendMessage/BroadcastMessage
    - ☐ - Unity Events
    - ☐ - CShart Events
- ☐  - Console styles (GoldSRC, Source1 like, UnityDefaults)
- ☐  - Customization (background image)
