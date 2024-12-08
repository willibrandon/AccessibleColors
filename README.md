# AccessibleColors

[![NuGet](https://img.shields.io/nuget/v/AccessibleColors.svg?label=NuGet)](https://www.nuget.org/packages/AccessibleColors/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/AccessibleColors.svg)](https://www.nuget.org/packages/AccessibleColors/)
[![Build](https://github.com/willibrandon/AccessibleColors/actions/workflows/ci.yml/badge.svg)](https://github.com/willibrandon/AccessibleColors/actions/workflows/ci.yml)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

**AccessibleColors** is a lightweight C# library that provides O(1) methods to compute WCAG-compliant contrast colors for foreground text, icons, and other UI elements given a background color. It instantly returns a suitable foreground color that meets or exceeds the standard WCAG 2.2 contrast ratio of 4.5:1 for normal text, or 3:1 for larger text or UI components.

In addition to single contrast colors, **AccessibleColors** also offers a dynamic color ramp generator to produce a sequence ("ramp") of colors all meeting WCAG standards against a given background. This is especially useful for UI states (hover, pressed, disabled) or theming scenarios where you need multiple related accessible colors derived from a single base color.

## Key Features

- **WCAG Compliance**: Ensures at least a 4.5:1 contrast ratio by default for normal text, and supports the 3:1 ratio for large text or UI elements, helping you create accessible user interfaces.
- **O(1) Performance (Single Contrast Calculation)**: Uses a precomputed lookup table (LUT) for sRGB-to-linear conversions, allowing instant single-color calculations.
- **Dynamic Accessible Ramps**: Generate a sequence of WCAG-compliant colors from a single base color. The algorithm uses minimal searching and a few small adjustments to ensure compliance for every color in the ramp.
- **No External Dependencies**: Relies only on `System.Drawing` types for colors, making integration straightforward.
- **Simple API**:
  - `GetContrastColor`: Instantly find a compliant foreground color for a given background.
  - `IsCompliant`: Verify if a given foreground/background pair meets a required ratio.
  - `GetContrastColorForText`: Consider text size and boldness to automatically choose a foreground color that meets large text or normal text WCAG rules.
  - `IsTextCompliant`: Check if a given pair is compliant under WCAG rules for both normal and large/bold text conditions.
  - `GetContrastColorForUIElement`: Obtain a compliant color for non-text UI elements (icons, focus rings, shapes) that often require a 3:1 ratio.
  - `IsUIElementCompliant`: Verify if a UI element's foreground color meets the WCAG non-text contrast guideline.
  - `GenerateAccessibleRamp`: Produce an entire ramp of related colors that remain accessible.

## Getting Started

1. **Install**:
   ```bash
   dotnet add package AccessibleColors
   ```

2. **Single Contrast Colors**:
   ```csharp
   using AccessibleColors;
   using System.Drawing;

   // Suppose you have a background color:
   var background = Color.FromArgb(255, 0, 0); // Bright red

   // Get a compliant foreground color:
   Color foreground = background.GetContrastColor();
   
   // Check compliance explicitly if needed:
   bool isAccessible = WcagContrastColor.IsCompliant(background, foreground);
   ```

3. **Handling Text of Different Sizes and Weights**:
   ```csharp
   using AccessibleColors;
   using System.Drawing;

   var background = Color.FromArgb(32, 32, 32); // Dark background
   double textSizePt = 18.0;  // At or above 18pt is considered large text by WCAG
   bool isBold = false;       // If it were bold and >= 14pt, it would also be treated as large text

   // Automatically choose a foreground color compliant for large text:
   Color textForeground = background.GetContrastColorForText(textSizePt, isBold);

   // Check text compliance:
   bool isTextCompliant = WcagContrastColor.IsTextCompliant(background, textForeground, textSizePt, isBold);

   // textForeground now provides a readable, accessible color for large text on the given background.
   ```
4. **UI Elements (Non-Text) Contrast**:
   ```csharp
   using AccessibleColors;
   using System.Drawing;

   var uiBackground = Color.FromArgb(240, 240, 240); // Light background

   // Get a color that meets or exceeds the 3:1 ratio for UI elements:
   Color uiElementColor = uiBackground.GetContrastColorForUIElement(3.0);
   bool isUIElementAccessible = WcagContrastColor.IsUIElementCompliant(uiBackground, uiElementColor);

   // Use uiElementColor for icons, focus indicators, or other graphical components 
   // to ensure they're distinguishable and accessible.
   ```

5. **Generate Accessible Color Ramps**:
   ```csharp
   using AccessibleColors;
   using System.Drawing;
   
   // Generate a 5-step ramp suitable for dark mode UI:
   Color baseAccent = Color.FromArgb(0, 120, 215); // Your brand accent
   int steps = 5;
   bool darkMode = true;

   IReadOnlyList<Color> ramp = ColorRampGenerator.GenerateAccessibleRamp(baseAccent, steps, darkMode);

   // Use these ramp colors for various UI states, ensuring each remains accessible.
   ```

5. **Integrating Into Your UI**:

    For example:
   ```csharp
   // Suppose you want to theme your app's buttons for dark mode.
   var baseAccent = Color.FromArgb(0, 120, 215);
   bool darkMode = true;
   int steps = 5;

   IReadOnlyList<Color> accessibleRamp = AccessibleColors.GenerateAccessibleRamp(baseAccent, steps, darkMode);

   // Assign ramp colors to different states of a custom button:
   myButton.NormalColor = accessibleRamp[0];
   myButton.HoverColor = accessibleRamp[1];
   myButton.PressedColor = accessibleRamp[2];
   myButton.FocusColor = accessibleRamp[3];
   myButton.DisabledColor = accessibleRamp[4];

   // For icons or other UI elements:
   var background = Color.FromArgb(32, 32, 32); 
   var iconColor = background.GetContrastColorForUIElement(); // Defaults to 3:1 for non-text
   bool isAccessibleIcon = WcagContrastColor.IsUIElementCompliant(background, iconColor);

   // Ensure the icon remains readable and accessible:
   myIcon.ForeColor = iconColor;

   // For text with size/weight considerations:
   double textSizePt = 18.0;
   bool isBold = true;
   var textColor = myButton.NormalColor.GetContrastColorForText(textSizePt, isBold);
   bool isTextAccessible = WcagContrastColor.IsTextCompliant(myButton.NormalColor, textColor, textSizePt, isBold);

   // Ensure the text remains readable and accessible:
   myButton.TextColor = textColor;
   ```

   By leveraging all these methods, you can ensure that every UI element, text, icons, focus indicators, stateful ramps, remains readable, accessible, and fully WCAG-compliant as themes or background colors evolve.

## Example

```csharp
using AccessibleColors;
using System.Drawing;

// Single Contrast Example:
var bg = Color.FromArgb(128,128,128); // Mid-gray background
var fg = bg.GetContrastColor();
Console.WriteLine($"Foreground: {fg}");
bool compliant = WcagContrastColor.IsCompliant(bg, fg);
Console.WriteLine($"Is compliant: {compliant}");

// Text Compliance Example:
var textBg = Color.FromArgb(32,32,32);
var textSize = 18.0;
var bold = true;
var textFg = WcagContrastColor.GetContrastColorForText(textBg, textSize, bold);
bool isTextCompliant = WcagContrastColor.IsTextCompliant(textBg, textFg, textSize, bold);
Console.WriteLine($"Text Foreground: {textFg}, Is text compliant: {isTextCompliant}");

// UI Element Compliance Example:
var uiBg = Color.FromArgb(240, 240, 240);
var uiElementColor = uiBg.GetContrastColorForUIElement(); // 3:1 by default
bool isUIElementAccessible = WcagContrastColor.IsUIElementCompliant(uiBg, uiElementColor);
Console.WriteLine($"UI Element Foreground: {uiElementColor}, Is UI compliant: {isUIElementAccessible}");

// Ramp Example:
var brandAccent = Color.FromArgb(50, 50, 50);
var rampColors = ColorRampGenerator.GenerateAccessibleRamp(brandAccent, 5, darkMode: false);
foreach (var c in rampColors)
{
    Console.WriteLine($"Ramp color: {c}, Compliant: {WcagContrastColor.IsCompliant(Color.White, c)}");
}
```

## Why This Matters

Accessibility is essential for building inclusive applications. Ensuring proper contrast ratios improves readability and usability for everyone. **AccessibleColors** automates these standards for both text and non-text UI elements, allowing you to quickly achieve compliance with WCAG guidelines without manual guesswork.

## Contributing

Contributions are welcome! Feel free to open issues, suggest features, or submit pull requests.

## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/willibrandon/AccessibleColors/blob/main/LICENSE) file for details.