FROM microsoft/aspnetcore

WORKDIR /app
COPY ./HelloWorld/bin/Debug/netcoreapp1.1/ /app

ENTRYPOINT ["dotnet", "HelloWorld.dll"]