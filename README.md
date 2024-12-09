# AccessibleColors

[![NuGet](https://img.shields.io/nuget/v/AccessibleColors.svg?label=NuGet)](https://www.nuget.org/packages/AccessibleColors/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/AccessibleColors.svg)](https://www.nuget.org/packages/AccessibleColors/)
[![Build](https://github.com/willibrandon/AccessibleColors/actions/workflows/ci.yml/badge.svg)](https://github.com/willibrandon/AccessibleColors/actions/workflows/ci.yml)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

**AccessibleColors** is a lightweight C# library that helps you ensure that your application's colors meet the WCAG 2.2 contrast ratios. It instantly determines a suitable, compliant foreground color for any given background color and provides tools to verify and generate accessible color palettes.

Key functionalities include:

- **Instant WCAG-Compliant Foregrounds**: Quickly get a foreground color that achieves at least a 4.5:1 contrast on normal text, or 3:1 for large text/UI elements.
- **Accessible Color Ramps**: Generate a sequence of related, contrast-compliant colors ("ramps") derived from a single base color. These ramps are useful for theming states (hover, pressed, disabled) or for ensuring multiple UI components share a visual theme while remaining accessible.

## Why AccessibleColors?

Ensuring accessibility compliance can be tedious. AccessibleColors automates contrast checking and selection, saving you from guesswork and manual tweaking. Whether you're implementing dark mode, creating brand-consistent themes, or just need to verify WCAG compliance, this library provides a straightforward, dependency-free solution.

## Key Features

- **WCAG Compliance**: Guarantees at least a 4.5:1 contrast ratio for normal text and supports 3:1 for large text/UI elements.
- **O(1) Single-Color Calculations**: Uses a precomputed LUT for sRGB-to-linear conversions, ensuring instant performance for single-color queries.
- **Accessible Ramps for Theming**:
  - **GenerateAccessibleRamp**: Produces a series of related colors (a "ramp") that remain accessible against a chosen background (dark or light).
  - **Already-Compliant Base Colors**: If the base color already meets WCAG standards, no adjustments are needed, resulting in little or no visible changes.
  - **Non-Compliant Base Colors**: If the base color is not compliant, each step is adjusted to ensure compliance, producing a clear gradient of accessible shades.
- **No External Dependencies**: Works directly with `System.Drawing`.
- **Simple API**:
  - **`GetContrastColor`**: Instantly find a compliant foreground color for a given background.
  - **`IsCompliant`**: Check if a given foreground and background pair meets the required WCAG ratio.
  - **`GetContrastColorForText`**: Choose a compliant text color based on text size and weight (large/bold rules).
  - **`IsTextCompliant`**: Verify whether a text foreground/background pair meets the WCAG text contrast criteria.
  - **`GetContrastColorForUIElement`**: Obtain a compliant color for non-text UI elements, meeting the 3:1 contrast guideline.
  - **`IsUIElementCompliant`**: Check if a UI element's foreground meets the WCAG non-text contrast ratio.
  - **`GenerateAccessibleRamp`**: Produces a series of accessible colors from a single base color.

## Getting Started

### Installation

```bash
dotnet add package AccessibleColors
```

### Single Contrast Colors

```csharp
using AccessibleColors;
using System.Drawing;

var background = Color.FromArgb(255, 0, 0); // Bright red background
Color foreground = background.GetContrastColor(); // Instantly get a compliant foreground

bool isAccessible = WcagContrastColor.IsCompliant(background, foreground);
```

### Handling Different Text Sizes/Weights

```csharp
var background = Color.FromArgb(32, 32, 32); 
double textSizePt = 18.0; // Large text threshold
bool isBold = false;

Color textForeground = background.GetContrastColorForText(textSizePt, isBold);
bool isTextCompliant = WcagContrastColor.IsTextCompliant(background, textForeground, textSizePt, isBold);
```

### UI Elements (Non-Text) Contrast

```csharp
var uiBackground = Color.FromArgb(240, 240, 240);
Color uiElementColor = uiBackground.GetContrastColorForUIElement(); // 3:1 ratio by default
bool isUIElementAccessible = WcagContrastColor.IsUIElementCompliant(uiBackground, uiElementColor);
```

### Generate Accessible Color Ramps

```csharp
// Example: Generating a 5-step ramp for a dark mode UI.
// When darkMode = true, the ramp is optimized for accessibility against a dark background.
// By default, the ramp generator considers Color.FromArgb(32, 32, 32) as the reference dark background.

Color baseAccent = Color.FromArgb(0, 120, 215);
int steps = 5;
bool darkMode = true;

IReadOnlyList<Color> ramp = ColorRampGenerator.GenerateAccessibleRamp(baseAccent, steps, darkMode);

// Verify compliance against the intended dark background:
Color darkBackground = Color.FromArgb(32, 32, 32);
foreach (var c in ramp)
{
    bool isCompliant = WcagContrastColor.IsCompliant(darkBackground, c);
    Console.WriteLine($"Ramp color: {c}, Compliant (dark bg): {isCompliant}");
}

// Use these ramp colors for various states in your UI:
myButton.NormalColor = ramp[0];
myButton.HoverColor = ramp[1];
myButton.PressedColor = ramp[2];
myButton.FocusColor = ramp[3];
myButton.DisabledColor = ramp[4];
```

**Important Note About Ramps**:  
If your base color is already compliant, the ramp may show little variation since no adjustments are needed. To see a noticeable gradient, start from a non-compliant color (e.g., a light gray on white). The ramp generator will then adjust each step to ensure accessibility, creating a visually distinct gradient.

For example:

```csharp
var startColor = Color.FromArgb(170,170,170); // Non-compliant on white
var rampColors = ColorRampGenerator.GenerateAccessibleRamp(startColor, 5, darkMode: false);
foreach (var c in rampColors)
{
    Console.WriteLine($"Ramp color: {c}, Compliant: {WcagContrastColor.IsCompliant(Color.White, c)}");
}
```

Here, you'll see a clear transition from lighter to darker, all accessible against white.

## Example

```csharp
using AccessibleColors;
using System.Drawing;

// Single Contrast Example:
var bg = Color.FromArgb(128,128,128); // Mid-gray background
var fg = bg.GetContrastColor();
Console.WriteLine($"Foreground: {fg} - Compliant: {WcagContrastColor.IsCompliant(bg, fg)}");

// Text Compliance Example (18pt large text on dark bg):
var textBg = Color.FromArgb(32,32,32);
double textSize = 18.0;
bool bold = true;
var textFg = textBg.GetContrastColorForText(textSize, bold);
Console.WriteLine($"Text Foreground: {textFg}, Compliant: {WcagContrastColor.IsTextCompliant(textBg, textFg, textSize, bold)}");

// UI Element Compliance Example:
var uiBg = Color.FromArgb(240, 240, 240);
var uiElementColor = uiBg.GetContrastColorForUIElement();
Console.WriteLine($"UI Element Foreground: {uiElementColor}, Compliant: {WcagContrastColor.IsUIElementCompliant(uiBg, uiElementColor)}");

// Ramp Example (compliance-driven adjustments with dark mode):
// Here we generate a ramp for a dark background scenario.
// The ramp will be tuned to be accessible on a dark background (e.g., Color.FromArgb(32, 32, 32)).
Color baseAccent = Color.FromArgb(0, 120, 215);
int steps = 5;
bool darkMode = true;

// This creates a 5-step ramp accessible against a dark background.
IReadOnlyList<Color> ramp = ColorRampGenerator.GenerateAccessibleRamp(baseAccent, steps, darkMode);

// Since darkMode = true, the intended background is a dark color:
Color darkBackground = Color.FromArgb(32, 32, 32);

foreach (var c in ramp)
{
    bool isCompliant = WcagContrastColor.IsCompliant(darkBackground, c);
    Console.WriteLine($"Ramp color: {c}, Compliant with dark background: {isCompliant}");
}
```

## Why This Matters

Accessibility is a cornerstone of inclusive design. Ensuring that text, icons, focus indicators, and other UI elements are distinguishable to everyone improves overall usability. **AccessibleColors** makes it simple to maintain WCAG compliance across your entire UI-no guesswork required.

## Contributing

Contributions are welcome! Please open issues, suggest features, or submit pull requests to help improve this library.

## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/willibrandon/AccessibleColors/blob/main/LICENSE) file for details.
