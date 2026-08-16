# Copilot Instructions

## Project Guidelines
- Server-side pages and layout should reflect the same authentication state as client-rendered pages in this Blazor app.
- CSS property values should use CSS variables (custom properties) rather than hardcoded literal values whenever possible. Prefer using variables defined in `wwwroot/css/base.css` for shared colors, spacing, sizing, radii, shadows, and transitions.
- Define animation keyframes in `wwwroot/css/keyframes.css` instead of component-scoped CSS files. Keyframe animations should be defined in `keyframes.css`.
