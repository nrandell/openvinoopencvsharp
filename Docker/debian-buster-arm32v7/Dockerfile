FROM mcr.microsoft.com/dotnet/core/runtime:3.1.0-buster-slim-arm32v7 AS dotnet-runtime

ARG DOWNLOAD_LINK=https://download.01.org/opencv/2019/openvinotoolkit/R3/l_openvino_toolkit_runtime_raspbian_p_2019.3.334.tgz
ARG INSTALL_DIR=/opt/intel/openvino
ARG TEMP_DIR=/tmp/openvino_installer

RUN apt-get update && apt-get install -y --no-install-recommends \
    ffmpeg \
    libgdiplus \
    libgdk3.0-cil \
    wget && \
    rm -rf /var/lib/apt/lists/*


RUN mkdir -p $TEMP_DIR && \
    mkdir -p $INSTALL_DIR && \
    cd $TEMP_DIR && \
    wget -c $DOWNLOAD_LINK && \
    tar -xf  l_openvino_toolkit_runtime_raspbian_p_*.tgz --strip 1 -C $INSTALL_DIR


FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/core/sdk:3.1.100-buster AS build
WORKDIR /build

COPY OpenVinoOpenCvSharp/OpenVinoOpenCvSharp.csproj OpenVinoOpenCvSharp/
COPY PeopleDetectionOpenCV/PeopleDetectionOpenCV.csproj PeopleDetectionOpenCV/
COPY Shared/Shared.csproj Shared/
COPY FaceDetectionOpenVino/FaceDetectionOpenVino.csproj FaceDetectionOpenVino/

RUN dotnet restore PeopleDetectionOpenCV/PeopleDetectionOpenCV.csproj
RUN dotnet restore FaceDetectionOpenVino/FaceDetectionOpenVino.csproj

COPY OpenVinoOpenCvSharp/ OpenVinoOpenCvSharp/
COPY PeopleDetectionOpenCV/ PeopleDetectionOpenCV/
COPY Shared/ Shared/
COPY FaceDetectionOpenVino/ FaceDetectionOpenVino/

WORKDIR /build/PeopleDetectionOpenCV
RUN dotnet build -c Release

WORKDIR /build/FaceDetectionOpenVino
RUN dotnet build -c Release

FROM mcr.microsoft.com/dotnet/core/sdk:3.1.100-buster-arm32v7 as build-native-env

COPY --from=dotnet-runtime /opt/intel/ /opt/intel/

RUN apt-get update && apt-get install -y --no-install-recommends \
    build-essential \
    cmake && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /OpenVinoOpenCvSharp
COPY CMakeLists.txt .
COPY OpenVinoOpenCvSharpExtern/ OpenVinoOpenCvSharpExtern/

WORKDIR /OpenVinoOpenCvSharp/build/
RUN cmake \
        -D OpenCV_DIR=/opt/intel/openvino/opencv/cmake \
        -D InferenceEngine_DIR=/opt/intel/openvino/deployment_tools/inference_engine/share \
        -D CMAKE_INSTALL_PREFIX=../install \
        ..

RUN cmake --build . --target install --config Release


# FROM build AS publish
# WORKDIR /build/PeopleDetectionOpenCV
# RUN dotnet publish --no-build -c Release -o /app/PeopleDetectionOpenCV/publish

# WORKDIR /build/FaceDetectionOpenVino
# RUN dotnet publish --no-build -c Release -o /app/FaceDetectionOpenVino/publish

FROM dotnet-runtime as final
WORKDIR /app
COPY --from=build-native-env /OpenVinoOpenCvSharp/install/lib/libOpenVinoOpenCvSharpExtern.so /opt/
ENV LD_LIBRARY_PATH=/opt:/opt/intel/openvino/opencv/lib:/opt/intel/openvino/deployment_tools/inference_engine/lib/armv7l/

# COPY --from=publish /app/ .

# COPY PeopleDetectionOpenCV/run.sh .
# RUN chmod a+x run.sh

# ENTRYPOINT [ "./run.sh" ]
