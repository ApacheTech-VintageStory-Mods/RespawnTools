# Copilot Instructions

## Project Guidelines
- Prefer avoiding reflection and expression-tree boilerplate when a simpler direct service overload can support runtime-discovered setting properties.
- Prefer using ApacheTech.Common.Extensions.Harmony utilities for cleaner reflection in Gantry rather than raw System.Reflection APIs where the helpers apply.