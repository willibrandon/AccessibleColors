# AccessibleColors

[![NuGet](https://img.shields.io/nuget/v/AccessibleColors.svg?label=NuGet)](https://www.nuget.org/packages/AccessibleColors/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/AccessibleColors.svg)](https://www.nuget.org/packages/AccessibleColors/)
[![Build](https://github.com/willibrandon/AccessibleColors/actions/workflows/ci.yml/badge.svg)](https://github.com/willibrandon/AccessibleColors/actions/workflows/ci.yml)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

**AccessibleColors** is a lightweight C# library that provides O(1) methods to compute WCAG-compliant contrast colors for foreground text or icons given a background color. It instantly returns a suitable foreground color that meets or exceeds the standard WCAG 2.2 contrast ratio of 4.5:1.

In addition to single contrast colors, **AccessibleColors** also offers a dynamic color ramp generator to produce a sequence ("ramp") of colors all meeting WCAG standards against a given background. This is especially useful for UI states (hover, pressed, disabled) or theming scenarios where you need multiple related accessible colors derived from a single base color.

## Key Features

- **WCAG Compliance**: Ensures at least a 4.5:1 contrast ratio by default, helping you create accessible user interfaces.
- **O(1) Performance (Single Contrast Calculation)**: Uses a precomputed lookup table (LUT) for sRGB-to-linear conversions, allowing instant single-color calculations.
- **Dynamic Accessible Ramps**: Generate a sequence of WCAG-compliant colors from a single base color. The algorithm uses minimal searching and a few small adjustments to ensure compliance for every color in the ramp.
- **No External Dependencies**: Relies only on `System.Drawing` types for colors, making integration straightforward.
- **Simple API**:
  - `GetContrastColor`: Instantly find a compliant foreground color for a given background.
  - `IsCompliant`: Verify if a given foreground/background pair meets a required ratio.
  - `GetContrastColorForText`: Consider text size and boldness to automatically choose a foreground color that meets large text or normal text WCAG rules.
  - `IsTextCompliant`: Check if a given pair is compliant under WCAG rules for both normal and large/bold text conditions.
  - `GenerateAccessibleRamp`: Produce an entire ramp of related colors that remain accessible.

## Getting Started

1. **Install**: Add the library as a reference to your project. Since it's published on NuGet:
   ```bash
   dotnet add package AccessibleColors
   ```

2. **Use for Single Contrast Colors**:
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

   var bg = Color.FromArgb(32, 32, 32); // Dark background
   double textSizePt = 18.0;  // At or above 18pt is considered large text by WCAG
   bool isBold = false;       // If it were bold and >= 14pt, it would also be treated as large text

   // Automatically choose a foreground color compliant for large text:
   Color textForeground = WcagContrastColor.GetContrastColorForText(bg, textSizePt, isBold);

   // Check if the chosen color meets WCAG contrast ratios for large text:
   bool isTextCompliant = WcagContrastColor.IsTextCompliant(bg, textForeground, textSizePt, isBold);

   // textForeground now provides a readable, accessible color for large text on the given background.
   ```

4. **Generate Accessible Color Ramps**:
   ```csharp
   using AccessibleColors;
   using System.Drawing;
   
   // Generate a 5-step ramp suitable for dark mode UI:
   Color baseColor = Color.FromArgb(0, 120, 215); // Your brand accent
   int steps = 5;
   bool darkMode = true;

   IReadOnlyList<Color> ramp = AccessibleColors.GenerateAccessibleRamp(baseColor, steps, darkMode);

   // Each color in 'ramp' should meet WCAG compliance against the chosen background.
   // Use them for various UI states or theme elements.
   ```

5. **Integrate Into Your UI**:

    Use `GetContrastColor`, `GenerateAccessibleRamp`, `GetContrastColorForText`, and `IsTextCompliant` anywhere you need accessible colors dynamically, custom themes, responsive adjustments, or design tools.

    For example:
   ```csharp
   // Suppose you have a brand accent color and you want to theme your app's buttons for dark mode.
   // First, generate a 5-step accessible ramp:
   var baseAccent = Color.FromArgb(0, 120, 215);
   bool darkMode = true;
   int steps = 5;

   IReadOnlyList<Color> accessibleRamp = AccessibleColors.GenerateAccessibleRamp(baseAccent, steps, darkMode);

   // Now assign these ramp colors to different states of a custom button:
   myButton.NormalColor = accessibleRamp[0];
   myButton.HoverColor = accessibleRamp[1];
   myButton.PressedColor = accessibleRamp[2];
   myButton.FocusColor = accessibleRamp[3];
   myButton.DisabledColor = accessibleRamp[4];

   // For icons or other elements over a known background, directly use GetContrastColor:
   var bg = Color.FromArgb(32, 32, 32); // Dark background
   var iconColor = bg.GetContrastColor(); 
   bool isAccessibleIcon = WcagContrastColor.IsCompliant(bg, iconColor);

   // Ensure the icon remains readable and accessible:
   myIcon.ForeColor = iconColor;

   // For text, consider text size and boldness using GetContrastColorForText:
   double textSizePt = 18.0;
   bool isBold = true;
   var textColor = WcagContrastColor.GetContrastColorForText(myButton.NormalColor, textSizePt, isBold);
   bool isTextAccessible = WcagContrastColor.IsTextCompliant(myButton.NormalColor, textColor, textSizePt, isBold);

   // Ensure the text remains readable and accessible:
   myButton.TextColor = textColor;
   ```

   By leveraging `GenerateAccessibleRamp`, `GetContrastColor`, `GetContrastColorForText`, and `IsTextCompliant`, you ensure that every UI element-whether a button state, icon, or text-remains accessible, readable, and adheres to WCAG guidelines, even as themes or background colors evolve.

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

// Ramp Example:
var brandAccent = Color.FromArgb(50, 50, 50);
var rampColors = AccessibleColors.GenerateAccessibleRamp(brandAccent, 5, darkMode: false);
foreach (var c in rampColors)
{
    Console.WriteLine($"Ramp color: {c}, Compliant: {WcagContrastColor.IsCompliant(Color.White, c)}");
}
```

## Why This Matters

Accessibility is not just a nice-to-have; it's an essential part of building inclusive applications. Ensuring proper contrast ratios improves readability for everyone, including users with visual impairments. **AccessibleColors** automates these standards:

- **Single Contrast Calculations & Text-Aware Methods**: Instantly determine a compliant foreground color, factoring in text size and weight.
- **Ramps for Theming**: Dynamically produce multiple related colors that all maintain compliance, streamlining UI state and theme development.

## Contributing

Contributions are welcome! Feel free to open issues, suggest features, or submit pull requests.

## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/willibrandon/AccessibleColors/blob/main/LICENSE) file for details.