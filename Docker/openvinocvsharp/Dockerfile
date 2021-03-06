ARG OPENCVSHARPEXTERN_VERSION
FROM nrandell/openvinocvsharpextern:x64-${OPENCVSHARPEXTERN_VERSION} as ubuntu-18.04-x64-build
FROM nrandell/openvinocvsharpextern:arm64-${OPENCVSHARPEXTERN_VERSION} as debian-10-arm64-build
FROM nrandell/openvinocvsharpextern:arm32-${OPENCVSHARPEXTERN_VERSION} as debian-10-arm32-build

FROM mcr.microsoft.com/dotnet/core/sdk:3.1.101-buster as build-dotnet

ARG OPENCVSHARP_VERSION

RUN apt-get update && apt-get install -y --no-install-recommends \
    unzip \
    cmake

WORKDIR /source

RUN wget https://github.com/shimat/opencvsharp/archive/${OPENCVSHARP_VERSION}.zip && \
    unzip ${OPENCVSHARP_VERSION}.zip && \
    rm ${OPENCVSHARP_VERSION}.zip && \
    mv opencvsharp-${OPENCVSHARP_VERSION} opencvsharp 

WORKDIR /source/opencvsharp/src/OpenCvSharp/
COPY ["OpenCvSharp.csproj", "OpenVinoOpenCvSharpExtern.dll", "./"]
COPY --from=ubuntu-18.04-x64-build /install/lib/libOpenVinoOpenCvSharpExtern.so ./ubuntu-18.04-x64-build/
COPY --from=debian-10-arm64-build /install/lib/libOpenVinoOpenCvSharpExtern.so ./debian-10-arm64-build/
COPY --from=debian-10-arm32-build /install/lib/libOpenVinoOpenCvSharpExtern.so ./debian-10-arm32-build/

RUN dotnet restore
RUN dotnet build -c Release --no-restore
RUN dotnet pack -c Release --no-build -o /publish

WORKDIR /publish
ARG NUGET_API_KEY
RUN dotnet nuget push *.nupkg --source "https://api.nuget.org/v3/index.json" --api-key ${NUGET_API_KEY}
