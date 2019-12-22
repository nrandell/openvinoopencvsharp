del -rec published
dotnet pack -c Release -o published
dotnet nuget push published\*.nupkg  --api-key "776ead9a-3886-4411-a333-a536f2b505b6" --source https://www.myget.org/F/nrandell-feed/api/v2/package