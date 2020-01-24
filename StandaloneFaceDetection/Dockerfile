ARG IMAGE_VERSION
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/core/sdk:3.1.101-buster as build

WORKDIR /build

COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["StandaloneFaceDetection/StandaloneFaceDetection.csproj", "StandaloneFaceDetection/"]

WORKDIR /build/StandaloneFaceDetection
RUN dotnet restore

WORKDIR /build
COPY ["Shared/", "Shared/"]
COPY ["StandaloneFaceDetection/", "StandaloneFaceDetection/"]
WORKDIR /build/StandaloneFaceDetection

RUN dotnet build -c Release

FROM build as publish
RUN dotnet publish -c Release -o /publish

FROM nrandell/openvino-dotnet-core-runtime:${IMAGE_VERSION} as final

# RUN apt-get update && DEBIAN_FRONTEND=noninteractive apt-get install -y --no-install-recommends \
# 	libgdk3.0-cil

# RUN apt-get update && \
#     apt-get install -y --no-install-recommends \
#     software-properties-common \
#     gpg-agent \
#     dirmngr && \
#     apt-get clean

# RUN apt-get purge -y ffmpeg && apt-get autoremove -y

# RUN add-apt-repository ppa:jonathonf/ffmpeg-4 && \
#     apt-get update && apt-get install -y --no-install-recommends \
#     ffmpeg

WORKDIR /app
COPY --from=publish /publish .

ENTRYPOINT ["dotnet", "StandaloneFaceDetection.dll"]
