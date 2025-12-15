# TelegramEasyProcedure

A .NET library that enables you to create procedural forms and multi-stage workflows in Telegram bots using just a JSON configuration file. No complex code required - define your entire bot flow declaratively!

## Purpose

TelegramEasyProcedure simplifies the creation of multi-stage procedural forms in Telegram bots. Instead of writing complex state management code, you define your entire bot flow in a single JSON file (`botconfig.json`). The library handles navigation, button callbacks, language support, and stage transitions automatically.

## Features

- **JSON-Based Configuration**: Define entire bot workflows in a single JSON file
- **Multi-Language Support**: Built-in support for multiple languages per stage
- **Automatic Navigation**: Automatic "To Root" and "To Previous" button generation
- **Dynamic Stage Transitions**: Navigate between stages based on button clicks
- **Custom Button Handlers**: Optional C# handlers for button click events
- **Type-Safe**: Full validation of JSON configuration
- **Easy Integration**: Simple setup with minimal code

## Requirements

- .NET 8.0 or later
- Telegram Bot Token (get one from [@BotFather](https://t.me/botfather))
- Telegram.Bot NuGet package

## Quick Start

### 1. Installation

Clone the repository and build the solution:

```bash
dotnet build
```

### 2. Configuration

#### Configure Bot Token

Edit `Presentation/appsettings.json` or `Presentation/appsettings.Local.json`:

```json
{
  "Bot": {
    "Token": "YOUR_TELEGRAM_BOT_TOKEN_HERE"
  }
}
```

#### Create Your Bot Configuration

Edit `Presentation/botconfig.json` to define your procedural flow. See the [Configuration Guide](#configuration-guide) below.

### 3. Run

```bash
cd Presentation
dotnet run
```

## Configuration Guide

The `botconfig.json` file is the heart of your bot. It defines all procedures, stages, buttons, and their relationships.

### Basic Structure

```json
{
  "supportedLanguages": ["english", "farsi"],
  "procedures": [...],
  "buttons": [...]
}
```

### Components

#### 1. Supported Languages

Define all languages your bot supports:

```json
"supportedLanguages": [
  "english",
  "farsi",
  "spanish"
]
```

#### 2. Procedures

A procedure is a complete workflow with multiple stages:

```json
{
  "id": 0,
  "title": "My Procedure",  // Optional, for documentation
  "rootStageId": 0,         // The starting stage ID
  "addToRootButton": true,  // Auto-add "To Root" button to non-root stages
  "addToPreviousButton": true,  // Auto-add "To Previous" button to non-root stages
  "toRootButtonId": -2,     // Button ID for "To Root" button
  "toPreviousButtonId": -1, // Button ID for "To Previous" button
  "stages": [...]
}
```

#### 3. Stages

Each stage represents a step in your procedure:

```json
{
  "id": 0,
  "title": "Welcome Stage",  // Optional
  "text": {
    "english": "Welcome! Choose an option:",
    "farsi": "خوش آمدید! گزینه ای انتخاب کنید:"
  },
  "options": [
    [
      {
        "id": 0,
        "buttonId": 1,
        "nextStageId": 1
      }
    ]
  ]
}
```

**Stage Properties:**
- `id`: Unique identifier for the stage
- `title`: Optional description (for documentation)
- `text`: Multi-language text displayed to users (key = language name)
- `previousStageId`: ID of the previous stage (for "To Previous" navigation)
- `removeToRootButton`: Set to `true` to hide "To Root" button on this stage
- `removeToPreviousButton`: Set to `true` to hide "To Previous" button on this stage
- `options`: Array of button rows (each inner array is a row)

**Note:** If a stage has no `options`, only the default navigation buttons ("To Root" and "To Previous") will be shown (if enabled).

#### 4. Options (Buttons in Stages)

Options define buttons within a stage:

```json
{
  "id": 0,              // Unique option ID within the stage
  "buttonId": 1,        // References a button definition
  "nextStageId": 1      // Optional: ID of next stage to navigate to
}
```

**Option Properties:**
- `id`: Unique identifier for the option within the stage
- `buttonId`: References a button from the `buttons` array
- `nextStageId`: Optional - if specified, clicking this button navigates to the specified stage

#### 5. Buttons

Define reusable button text for multiple languages:

```json
{
  "id": 1,
  "title": "Continue Button",  // Optional
  "text": {
    "english": "Continue",
    "farsi": "ادامه"
  }
}
```

**Special Button IDs:**
- `-2`: "To Root Stage" button (used with `toRootButtonId`)
- `-1`: "To Previous Stage" button (used with `toPreviousButtonId`)

### Example Configuration

See `Presentation/botconfig.json` for a complete example with:
- Multiple stages
- Navigation between stages
- Multi-language support
- Custom buttons
- Default navigation buttons

## Code Integration

### Basic Setup

The library automatically loads `botconfig.json` and creates a `ProcedureManager`:

```csharp
// In Program.cs or your startup code
var bot = BotConfigurationHelper.CreateClient(cts.Token);
bot.AddEvents(); // This creates ProcedureManager and sets up event handlers
```

### Starting a Procedure

Create message handlers to start your procedures:

```csharp
[OnMessage("/start_English")]
public async Task HandleStartEnglishCommand(Message msg)
{
    const string language = "english";
    if (!procedureManager.TryGetStageMessage("0", "0", language, out var stageMessage))
        return;

    await bot.SendMessage(
        msg.Chat,
        stageMessage.MultilanguageText[language],
        parseMode: stageMessage.TextParseMode,
        replyMarkup: stageMessage.MultilanguageReplyMarkup?[language]
    );
}
```

### Custom Button Handlers

Add custom logic when buttons are clicked:

```csharp
[OnOptionClick(procedureId: 0, stageId: 0, optionId: 0)]
public Task<OptionHandlerResult> OnClickExample(
    TelegramBotClient bot, Update update, OptionOnClickDetails details
)
{
    Console.WriteLine($"User clicked option {details.OptionId} in stage {details.StageId}");
    
    // Perform custom logic here
    // Return OptionHandlerResult(true) to prevent automatic navigation
    // Return OptionHandlerResult(false) or null to allow automatic navigation
    
    return Task.FromResult(new OptionHandlerResult(false));
}
```

**OptionHandlerResult:**
- `AvoidMovingNextStage = true`: Prevents automatic navigation to next stage
- `AvoidMovingNextStage = false`: Allows automatic navigation (default behavior)

## License

This project is licensed with the [MIT License](LICENSE).

## 🤝 Contributing

This project is in its early stages and has a lot of room for refactoring and debugging. If you're interested, we can work together to improve this library and add more features to it :)
