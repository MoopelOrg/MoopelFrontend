# css-theme-enforcer

Use this skill to audit and enforce reusable, theme-driven CSS for MoopelFrontend.

## Purpose
- Require a repository-wide CSS/class audit before concluding compliance.
- Ensure CSS is reusable, tokenized, and centrally theme-driven.
- Enforce that the active user theme drives nearly all visual CSS and all color values.
- Treat any hardcoded or forced CSS as an issue.

## Scope
- `MoopelFrontend/wwwroot/css/*.css`
- `MoopelFrontend.Client/**/*.razor.css`
- `MoopelFrontend/**/*.razor.css`
- `MoopelFrontend.Client/**/*.razor`
- `MoopelFrontend/**/*.razor`
- `MoopelFrontend/**/*.html`

## Required Rules
1. Replace all hardcoded color literals with CSS variables from `MoopelFrontend/wwwroot/css/base.css`.
2. Replace hardcoded visual literals (spacing, radii, shadows, sizing, transitions) with shared tokens wherever applicable.
3. Keep all keyframes in `MoopelFrontend/wwwroot/css/keyframes.css` (no component-local `@keyframes`).
4. Perform a class reuse audit and prefer shared styles from `buttons.css`, `cards.css`, `forms.css`, `framework.css`, `modules.css`, and `utilities.css` before component duplication.
5. Treat forced CSS as violations (`!important`, inline visual styles, JS-injected fixed visual literals).
6. Ensure active theme authority: all color outputs and nearly all visual styling must resolve through theme variables.
7. Any new theme-relevant variable must be defined in `:root` in `base.css` and surfaced in the Settings theme editor + persistence flow.

## Theme Editor Requirement
When implementing Settings theme support, include all theme-relevant variables as editable controls and ensure updates apply live and persist.

## Output Expectations
- Report violations with file path, line number, offending snippet, and concrete fix.
- Include repository-wide totals for hardcoded values, forced CSS, and theme-authority gaps.
- When fixing, make surgical edits that preserve behavior.

