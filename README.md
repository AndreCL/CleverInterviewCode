# CleverInterviewCode

## How I made it:

### Before starting I installed the following to my personal machine

- Azure development workload
- Azure Core Tools https://github.com/Azure/azure-functions-core-tools

### Create all I need through the VS Developer PowerShell

I used the following guide as a template: https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local

- Create a local project:

> func init CleverInterviewCode --dotnet

- Create a function

> func new --name RfidFunction --template "HTTP trigger" --authlevel "function"

### Open project in Visual Studio

- Add Nuget for table storage 

> dotnet add package Microsoft.Azure.Cosmos.Table

https://microsoft.github.io/AzureTipsAndTricks/blog/tip360.html

- Now the fun part: Code :)