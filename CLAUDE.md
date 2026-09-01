# Agent Instructions

Project-wide coding conventions also live in `.github/copilot-instructions.md`
(naming, exception handling, SSEM/FIASSE security principles). Follow those in
addition to what's below.

## Class design

- Never mark any class `sealed` in this repository unless explicitly
  instructed to do so for that specific class. This holds even when a class
  has zero derived types and no virtual/abstract members - the kind of
  evidence a "seal non-inherited classes" cleanup pass would otherwise treat
  as sufficient on its own. Exception-derived types are a specific case of
  this: consumers conventionally build their own error hierarchies on top of
  library exceptions, so sealing them is particularly likely to break someone
  even though nothing in this repo currently derives from one.
