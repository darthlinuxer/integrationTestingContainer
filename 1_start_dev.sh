docker run --rm -it --network host --hostname container --name example -v /home/darthlinuxer/integrationTestingContainer:/app --workdir /app mcr.microsoft.com/dotnet/sdk:8.0 bash
