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
    tar -xf  l_openvino_toolkit_runtime_raspbian_p_*.tgz --strip 1 -C $INSTALL_DIR && \
    rm -rf ${INSTALL_DIR}/opencv

FROM dotnet-runtime as opencv-build
RUN apt-get update && apt-get install -y --no-install-recommends \
    build-essential \
    unzip \
    cmake && \
    rm -rf /var/lib/apt/lists/*

ARG OPENCV_VERSION=4.1.2
WORKDIR /opencvbuild

RUN wget https://github.com/opencv/opencv/archive/${OPENCV_VERSION}.zip && \
    unzip ${OPENCV_VERSION}.zip && \
    rm ${OPENCV_VERSION}.zip && \
    mv opencv-${OPENCV_VERSION} opencv && \
    wget https://github.com/opencv/opencv_contrib/archive/${OPENCV_VERSION}.zip && \
    unzip ${OPENCV_VERSION}.zip && \
    rm ${OPENCV_VERSION}.zip && \
    mv opencv_contrib-${OPENCV_VERSION} opencv_contrib

# Build OpenCV
WORKDIR /opencvbuild/build
RUN cmake \
    -D OPENCV_EXTRA_MODULES_PATH=/opencvbuild/opencv_contrib/modules \
    -D InferenceEngine_DIR=/opt/intel/openvino/deployment_tools/inference_engine/share \
    -D CMAKE_INSTALL_PREFIX=/opencv/install \
    -D CMAKE_BUILD_TYPE=RELEASE \
    -D ENABLE_NEON=ON \
    -D ENABLE_VFPV3=ON \
    -D WITH_OPENMP=ON \
    -D WITH_TBB=ON \
    -D BUILD_TBB=ON \
    -D WITH_VTK=OFF \
    -D WITH_INF_ENGINE=ON \
    -D BUILD_EXAMPLES=OFF \
    -D BUILD_DOCS=OFF \
    -D BUILD_PERF_TESTS=OFF \
    -D BUILD_TESTS=OFF \
    -D BUILD_JAVA=OFF \
    -D BUILD_opencv_app=OFF \
    -D BUILD_opencv_java=OFF \
    -D BUILD_opencv_python=OFF \
    -D BUILD_opencv_ts=OFF \
    -D BUILD_opencv_js=OFF \
    -D WITH_GSTREAMER=OFF \ 
    -D OPENCV_ENABLE_NONFREE=ON \
    ../opencv

RUN make && \
    make install


# Build OpenVinoOpenCvSharp
WORKDIR /OpenVinoOpenCvSharp
COPY CMakeLists.txt .
COPY OpenVinoOpenCvSharpExtern/ OpenVinoOpenCvSharpExtern/

WORKDIR /OpenVinoOpenCvSharp/build/
RUN cmake \
        -D OpenCV_DIR=/opencvbuild/install/cmake \
        -D InferenceEngine_DIR=/opt/intel/openvino/deployment_tools/inference_engine/share \
        -D CMAKE_INSTALL_PREFIX=../install \
        ..

RUN make -j4 && \
    make install


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

FROM build AS publish
WORKDIR /build/PeopleDetectionOpenCV
RUN dotnet publish --no-build -c Release -o /app/PeopleDetectionOpenCV/publish

WORKDIR /build/FaceDetectionOpenVino
RUN dotnet publish --no-build -c Release -o /app/FaceDetectionOpenVino/publish

FROM dotnet-runtime as final
WORKDIR /app
COPY --from=opencv-build /opencvsharp/install/ /opt/opencv/
COPY --from=opencv-build /OpenVinoOpenCvSharp/install/lib/libOpenVinoOpenCvSharpExtern.so /opt/

ENV LD_LIBRARY_PATH=/opt:/opt/opencv/lib:/opt/intel/openvino/deployment_tools/inference_engine/lib/armv7l

COPY --from=publish /app/ .

# COPY PeopleDetectionOpenCV/run.sh .
# RUN chmod a+x run.sh

# ENTRYPOINT [ "./run.sh" ]
