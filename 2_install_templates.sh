dotnet new tool-manifest
dotnet new install QuickstartTemplate::2.0.0
dotnet tool install dotnet-aspnet-codegenerator --version 8.0.0
dotnet tool install dotnet-ef --version 8.0.0
dotnet add ./src/api package Microsoft.EntityFrameworkCore
dotnet add ./src/api package Microsoft.EntityFrameworkCore.Tools
dotnet add ./src/api package Microsoft.EntityFrameworkCore.Design
dotnet add ./src/api/example.api.csproj package  Microsoft.VisualStudio.Web.CodeGeneration.Design         
dotnet aspnet-codegenerator -p ./src/api --list 
# dotnet aspnet-codegenerator -p ./src/api minimalapi -m Person -dc AppDbContext -dbProvider sqlite -e PersonEndpoint -o