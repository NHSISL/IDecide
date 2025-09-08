# Copilot Instructions

## Code Style Rules

### Line Length
- All `.cs` source files must adhere to the following rule:
  - No line of code should exceed **120 characters** in length.
  - This includes comments, string literals, and code.
  - Exception: automatically generated files may be ignored if they cannot be reformatted safely.

### Enforcement
- Copilot should **not generate or suggest code** that exceeds the 120-character line limit.
- When writing new C# code, Copilot should:
  - Break up long method calls across multiple lines.
  - Use string interpolation or verbatim strings with proper line breaks if a literal would otherwise exceed 120 characters.
  - Format long LINQ queries across multiple lines.
  - Suggest wrapping parameters and arguments for readability.

### Review Guidelines
- When reviewing or completing code suggestions, Copilot should:
  - Scan `.cs` files for lines longer than 120 characters.
  - Highlight or flag any violations.
  - Recommend a multiline formatting fix for flagged lines.

### Examples

#### ✅ Correct (within 120 characters)
```csharp
var result = someService.DoSomething(param1, param2, param3)
    .Where(x => x.IsValid)
    .Select(x => x.Value);
❌ Incorrect (over 120 characters)
csharp
Copy code
var result = someService.DoSomething(param1, param2, param3).Where(x => x.IsValid).Select(x => x.Value).ToList();
yaml
Copy code

---
