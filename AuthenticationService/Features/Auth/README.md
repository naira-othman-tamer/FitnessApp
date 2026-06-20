# Authentication vertical slices

Each use case belongs in its own folder and owns its request, response, handler,
validation, and endpoint mapping. Business slices will be added once their
requirements are defined.

Example layout:

```text
Features/Auth/
  Register/
    RegisterCommand.cs
    RegisterHandler.cs
    RegisterEndpoint.cs
  Login/
    LoginQuery.cs
    LoginHandler.cs
    LoginEndpoint.cs
```
