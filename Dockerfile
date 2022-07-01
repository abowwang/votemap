FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
RUN apt-get update && apt-get install -y apt-utils libgdiplus libc6-dev
RUN mkdir /web
WORKDIR /web
COPY $PWD/out/ /web

CMD ["dotnet", "./votemap.dll"]
