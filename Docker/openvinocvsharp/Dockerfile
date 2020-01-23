FROM --platform=linux/amd64 nrandell/openvino-dotnet-core-runtime:3.1.1-2019.3.376-1 as ubuntu-18.04-x64-build

ARG OPENCVSHARP_VERSION=4.2.0.20200108

RUN apt-get update && apt-get install -y --no-install-recommends \
    build-essential \
    unzip \
    cmake

WORKDIR /source

RUN wget https://github.com/shimat/opencvsharp/archive/${OPENCVSHARP_VERSION}.zip && \
    unzip ${OPENCVSHARP_VERSION}.zip && \
    rm ${OPENCVSHARP_VERSION}.zip && \
    mv opencvsharp-${OPENCVSHARP_VERSION} opencvsharp 

WORKDIR /source/opencvsharp/src/OpenCvSharpExtern/
COPY ["CMakeLists.txt", "include_opencv.h", "/source/opencvsharp/src/OpenCvSharpExtern/"]

WORKDIR /build


RUN cmake \
    -D OpenCV_DIR=/opt/intel/openvino/opencv/cmake \
    -D InferenceEngine_DIR=/opt/intel/openvino/inference_engine/share \
    -D CMAKE_INSTALL_PREFIX=/install \
    /source/opencvsharp/src

RUN make 
RUN make install


FROM --platform=linux/arm64 nrandell/openvino-dotnet-core-runtime:3.1.1_4.2.0_2019_R3.1-1 as debian-10-arm64-build

ARG OPENCVSHARP_VERSION=4.2.0.20200108

RUN apt-get update && apt-get install -y --no-install-recommends \
    build-essential \
    unzip \
    cmake

WORKDIR /source

RUN wget https://github.com/shimat/opencvsharp/archive/${OPENCVSHARP_VERSION}.zip && \
    unzip ${OPENCVSHARP_VERSION}.zip && \
    rm ${OPENCVSHARP_VERSION}.zip && \
    mv opencvsharp-${OPENCVSHARP_VERSION} opencvsharp 

WORKDIR /source/opencvsharp/src/OpenCvSharpExtern/
COPY ["CMakeLists.txt", "include_opencv.h", "/source/opencvsharp/src/OpenCvSharpExtern/"]

WORKDIR /build


RUN cmake \
    -D OpenCV_DIR=/install/opencv/lib/cmake/opencv4 \
    -D InferenceEngine_DIR=/install/inference-engine/share \
    -D CMAKE_INSTALL_PREFIX=/install \
    /source/opencvsharp/src

RUN make 
RUN make install

FROM --platform=$BUILD_PLATFORM mcr.microsoft.com/dotnet/core/sdk:3.1.101-buster as build-dotnet

ARG OPENCVSHARP_VERSION=4.2.0.20200108

RUN apt-get update && apt-get install -y --no-install-recommends \
    unzip \
    cmake

WORKDIR /source

RUN wget https://github.com/shimat/opencvsharp/archive/${OPENCVSHARP_VERSION}.zip && \
    unzip ${OPENCVSHARP_VERSION}.zip && \
    rm ${OPENCVSHARP_VERSION}.zip && \
    mv opencvsharp-${OPENCVSHARP_VERSION} opencvsharp 

WORKDIR /source/opencvsharp/src/OpenCvSharp/
COPY ["OpenCvSharp.csproj", "./"]
COPY --from=ubuntu-18.04-x64-build /install/lib/libOpenVinoOpenCvSharpExtern.so ./ubuntu-18.04-x64-build/
COPY --from=debian-10-arm64-build /install/lib/libOpenVinoOpenCvSharpExtern.so ./debian-10-arm64-build/

RUN dotnet restore
RUN dotnet build -c Release --no-restore
RUN dotnet pack -c Release --no-build -o /publish

WORKDIR /publish
ARG NUGET_API_KEY
RUN dotnet nuget push *.nupkg --source "https://api.nuget.org/v3/index.json" --api-key ${NUGET_API_KEY}
