# Contributing

Branch names are short, descriptive, and written in **kebab-case** (lowercase words separated by hyphens). Every working branch starts with a prefix from the list below so it matches the GitHub branch ruleset.

## Long-lived branches

| Branch | Role | Purpose |
|--------|------|---------|
| `dev` | Default branch | Integration branch. All work branches from here and merges back here. |
| `main` | Release branch | Stable, deployable code. It only receives releases merged from `dev`. |

## Prefixes

| Prefix | Use for |
|--------|---------|
| `feat/` | New features, gameplay, or UI |
| `fix/` | Bug fixes |
| `doc/` | Documentation only |
| `refactor/` | Restructuring existing code while keeping behavior the same |
| `chore/` | Tooling, configuration, and maintenance |
| `art/` | Visual assets: models, textures, materials, animation, and VFX |
| `audio/` | Sound assets: music, sound effects, and voice |

## Format

```
<prefix>/<short-description>
```

**Rules:**

- Use lowercase letters, numbers, and hyphens only
- Keep names short but clear (2–5 words)
- No spaces or underscores

## Examples

```
feat/planet-landing
feat/officer-dialogue
fix/checkpoint-respawn
doc/contribution
refactor/puzzle-state
chore/update-gitignore
art/officer-character
audio/ambient-station
```

## Workflow

1. Branch from `dev`
2. Open a pull request back into `dev`
3. Delete the branch after merge
4. When `dev` is ready to ship, merge `dev` into `main` to cut a release
