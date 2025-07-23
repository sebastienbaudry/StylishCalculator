# Stylish Calculator UI Design Mockup

## Overall Theme
- **Base Color**: Dark charcoal (#121212) for the main background
- **Accent Colors**: 
  - Neon cyan (#00FFFF) for number buttons
  - Neon magenta (#FF00FF) for operation buttons
  - Neon green (#00FF00) for equals button
  - Neon yellow (#FFFF00) for currency conversion toggle
- **Text Color**: White (#FFFFFF) for high contrast
- **Glass Effect**: 15-20% opacity with blur effect on all controls

## Layout Structure

```
+-----------------------------------------------+
|  +---------------------------------------+    |
|  |                                       |    |
|  |             Display Area              |    |
|  |                                       |    |
|  +---------------------------------------+    |
|                                               |
|  +---------------------------------------+    |
|  |                                       |    |
|  |          Secondary Display            |    |
|  |        (Currency Conversion)          |    |
|  |                                       |    |
|  +---------------------------------------+    |
|                                               |
|  +-------+  +-------+  +-------+  +-------+  |
|  |       |  |       |  |       |  |       |  |
|  |   C   |  |  +/-  |  |   %   |  |   ÷   |  |
|  |       |  |       |  |       |  |       |  |
|  +-------+  +-------+  +-------+  +-------+  |
|                                               |
|  +-------+  +-------+  +-------+  +-------+  |
|  |       |  |       |  |       |  |       |  |
|  |   7   |  |   8   |  |   9   |  |   ×   |  |
|  |       |  |       |  |       |  |       |  |
|  +-------+  +-------+  +-------+  +-------+  |
|                                               |
|  +-------+  +-------+  +-------+  +-------+  |
|  |       |  |       |  |       |  |       |  |
|  |   4   |  |   5   |  |   6   |  |   -   |  |
|  |       |  |       |  |       |  |       |  |
|  +-------+  +-------+  +-------+  +-------+  |
|                                               |
|  +-------+  +-------+  +-------+  +-------+  |
|  |       |  |       |  |       |  |       |  |
|  |   1   |  |   2   |  |   3   |  |   +   |  |
|  |       |  |       |  |       |  |       |  |
|  +-------+  +-------+  +-------+  +-------+  |
|                                               |
|  +-------+  +-------+  +-------+  +-------+  |
|  |       |  |       |  |       |  |       |  |
|  |   0   |  |   .   |  |  DEL  |  |   =   |  |
|  |       |  |       |  |       |  |       |  |
|  +-------+  +-------+  +-------+  +-------+  |
|                                               |
|  +---------------------------------------+    |
|  |                                       |    |
|  |        Currency Converter Toggle      |    |
|  |                                       |    |
|  +---------------------------------------+    |
|                                               |
+-----------------------------------------------+
```

## Currency Converter Panel (Expanded)

```
+-----------------------------------------------+
|  +---------------------------------------+    |
|  |                                       |    |
|  |          Currency Selection           |    |
|  |                                       |    |
|  +---------------------------------------+    |
|                                               |
|  +---------------+     +---------------+      |
|  |               |     |               |      |
|  |  From: USD    |     |    To: EUR    |      |
|  |               |     |               |      |
|  +---------------+     +---------------+      |
|                                               |
|  +---------------------------------------+    |
|  |                                       |    |
|  |          Conversion Result            |    |
|  |                                       |    |
|  +---------------------------------------+    |
|                                               |
|  +---------------------------------------+    |
|  |                                       |    |
|  |           Convert Button              |    |
|  |                                       |    |
|  +---------------------------------------+    |
|                                               |
+-----------------------------------------------+
```

## Visual Effects

### Button Hover Effect
- Slight increase in brightness (10-15%)
- Subtle glow effect matching the button's neon color
- Slight scale increase (102-105%)

### Button Press Effect
- Brief flash of increased brightness
- Scale down to 98% of original size
- Quick bounce-back animation

### Display Area
- Subtle text glow effect
- Smooth transitions when numbers change
- Right-aligned text with appropriate padding

### Glass Effect Implementation
- Semi-transparent background (15-20% opacity)
- Blur effect behind the controls
- Subtle border highlight (1px) with gradient matching the control's color

## Animation Details

### Number Input Animation
- Numbers slide in from right to left
- Subtle bounce effect when they settle into position

### Operation Selection
- Selected operation button pulses briefly with increased glow
- Operation symbol appears with fade-in effect in the display area

### Calculation Result
- Brief flash effect on the display when equals is pressed
- Result appears with a quick fade-in transition

### Currency Conversion Toggle
- Smooth slide-down animation to reveal the currency panel
- Slide-up animation to hide it

## Responsive Design
- The calculator will maintain its aspect ratio
- Minimum size constraints to ensure readability
- Maximum size constraints to prevent excessive scaling on large displays