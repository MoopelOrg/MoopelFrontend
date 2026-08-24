# Copilot Instructions

## Project Guidelines
- Server-side pages and layout should reflect the same authentication state as client-rendered pages in this Blazor app.
- CSS property values should use CSS variables (custom properties) rather than hardcoded literal values whenever possible. Prefer using variables defined in `wwwroot/css/base.css` for shared colors, spacing, sizing, radii, shadows, and transitions. For the dashboard, define a dedicated CSS variable for every app group color and use group-specific variable overrides to drive shared tile gradients.
- Require repository-wide CSS/class audits; the active user theme must drive nearly all visual CSS and all color values. Any hardcoded or forced CSS should be treated as an issue.
- Define animation keyframes in `wwwroot/css/keyframes.css` instead of component-scoped CSS files. Keyframe animations should be defined in `keyframes.css`.
- Use shared constants for UI glyphs like the dashboard settings symbol instead of inline HTML entities.
- When adding files outside the app directories (e.g. in `.github/`), notify the user that these files will not appear in Visual Studio's Solution Explorer automatically — they must manually show all files or add them to the solution.
