# Agent Instructions

Project-wide coding conventions also live in `.github/copilot-instructions.md`
(naming, exception handling, SSEM/FIASSE security principles). Follow those in
addition to what's below.

## Class design

- Never mark a class that inherits from `Exception` (directly or transitively)
  as `sealed` in this repository. Exception types are conventionally left open
  so consumers can derive more specific exceptions from them in their own
  error hierarchies; sealing forecloses that even when nothing in this repo
  currently does it. This applies regardless of what a "seal non-inherited
  classes" cleanup pass would otherwise conclude from an empty derived-types
  search.
