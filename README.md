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
  - A single `GetContrastColor` extension method on `Color` and an `IsCompliant` method let you easily ensure accessibility for individual colors.
  - A `GenerateAccessibleRamp` method to produce an entire ramp of related colors that remain accessible.

## Getting Started

1. **Install**: Add the library as a reference to your project. Since it's published on NuGet:
   ```bash
   dotnet add package AccessibleColors
   ```

2. **Use for Single Contrast Colors**:
   ```csharp
   using System.Drawing;
   using AccessibleColors;

   // Suppose you have a background color:
   var background = Color.FromArgb(255, 0, 0); // Bright red

   // Get a compliant foreground color:
   Color foreground = background.GetContrastColor();
   
   // Check compliance explicitly if needed:
   bool isAccessible = WcagContrastColor.IsCompliant(background, foreground);
   ```
   
3. **Generate Accessible Color Ramps**:
   ```csharp
   using System.Drawing;
   using AccessibleColors;

   // Generate a 5-step ramp suitable for dark mode UI:
   Color baseColor = Color.FromArgb(0, 120, 215); // Your brand accent
   int steps = 5;
   bool darkMode = true;

   IReadOnlyList<Color> ramp = AccessibleColors.GenerateAccessibleRamp(baseColor, steps, darkMode);

   // Each color in 'ramp' should meet WCAG compliance against the chosen background.
   // Use them for various UI states or theme elements.
   ```

## Example

```csharp
using System.Drawing;
using AccessibleColors;

// Single Contrast Example:
var bg = Color.FromArgb(128,128,128); // Mid-gray background
var fg = bg.GetContrastColor();

Console.WriteLine($"Foreground: {fg}");
bool compliant = WcagContrastColor.IsCompliant(bg, fg);
Console.WriteLine($"Is compliant: {compliant}");

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

- **Single Contrast Calculations**: Instantly determine a compliant foreground color for any given background.
- **Ramps for Theming**: Dynamically produce multiple related colors that all maintain compliance, streamlining UI state and theme development.

## Contributing

Contributions are welcome! Feel free to open issues, suggest features, or submit pull requests.

## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/willibrandon/AccessibleColors/blob/main/LICENSE) file for details.