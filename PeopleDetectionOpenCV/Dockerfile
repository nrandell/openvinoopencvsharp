FROM mcr.microsoft.com/dotnet/core/runtime:3.1.0 AS base
RUN apt update && \
	apt install -y \
		ffmpeg \
		libgdiplus \
		libgtk2.0-0 \
		libtbb2

WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/core/sdk:3.1.100-buster AS build
WORKDIR /src
COPY ["Directory.Build.props", "nuget.config", "./"]
COPY ["PeopleDetectionOpenCV/PeopleDetectionOpenCV.csproj", "PeopleDetectionOpenCV/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore --configfile nuget.config "PeopleDetectionOpenCV/PeopleDetectionOpenCV.csproj"

COPY ["GlobalSuppressions.cs", "./"]
COPY ["PeopleDetectionOpenCV/", "PeopleDetectionOpenCV/"]
COPY ["Shared/", "Shared/"]

WORKDIR "/src/PeopleDetectionOpenCV/"
RUN dotnet build --no-restore -c Release -o /app/build

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PeopleDetectionOpenCV.dll"]
