# AccessibleColors

**AccessibleColors** is a lightweight C# library that provides O(1) methods to compute WCAG-compliant contrast colors. Given a background color, it instantly returns a suitable foreground color that meets or exceeds the standard WCAG 2.2 contrast ratio of 4.5:1 for normal text.

## Key Features

- **WCAG Compliance**: Ensures at least a 4.5:1 contrast ratio by default, helping you create accessible user interfaces.
- **O(1) Performance**: Uses a precomputed lookup table (LUT) for sRGB-to-linear conversions, allowing instant calculations.
- **No External Dependencies**: Relies only on `System.Drawing` types for colors, making integration straightforward.
- **Simple API**: A single `GetContrastColor` extension method on `Color` and a `IsCompliant` method let you easily ensure accessibility.

## Getting Started

1. **Install**: Add the library as a reference to your project. If published as a NuGet package, install via:
   ```bash
   dotnet add package AccessibleColors
   ```

2. **Use**:
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

3. **Integrate Into Your UI**:  
   Use `GetContrastColor` anywhere you need to select text or icon colors based on a dynamically changing background, such as in custom theming, responsive UI adjustments, or design tools.

## Example

```csharp
using System.Drawing;
using AccessibleColors;

var bg = Color.FromArgb(128,128,128); // Mid-gray background
var fg = bg.GetContrastColor();

Console.WriteLine($"Foreground: {fg}");

// Verify compliance
bool compliant = WcagContrastColor.IsCompliant(bg, fg);
Console.WriteLine($"Is compliant: {compliant}");
```

## Why This Matters

Accessibility is not just a nice-to-have; it's an essential part of building inclusive applications. Ensuring proper contrast ratios improves readability for everyone, including users with visual impairments. With **AccessibleColors**, you can enforce these standards automatically and efficiently.

## Contributing

Contributions are welcome! Feel free to open issues, suggest features, or submit pull requests.

## License

This project is licensed under the MIT License. See the [LICENSE](/LICENSE) file for details.