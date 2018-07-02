# TestCosmosDistinctBug
Repro for https://github.com/Azure/azure-documentdb-dotnet/issues/536

## How to run

Update appsettings.json with your CosmosDB endpoint, account key, database id and collection id.
Run with `dotnet run`

When target framework is `netcoreapp2.1`: Error

> Unhandled Exception: Microsoft.Azure.Documents.BadRequestException: Message: {"errors":[{"severity":"Error","location":{"start":7,"end":15},"code":"SC1001","message":"Syntax error, incorrect syntax near 'DISTINCT'."}]}, Windows/10.0.17134 documentdb-netcore-sdk/1.9.1 ---> System.Runtime.InteropServices.COMException: Exception from HRESULT: 0x800A0B00

When target is `net461`: Success
