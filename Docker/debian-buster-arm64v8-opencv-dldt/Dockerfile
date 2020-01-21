FROM mcr.microsoft.com/dotnet/core/runtime:3.1.1-buster-slim-arm64v8 as dotnet-runtime

RUN apt-get update && apt-get install -y --no-install-recommends \
    ffmpeg \
    libgdiplus \
    libgdk3.0-cil

FROM dotnet-runtime as native-build
RUN apt-get update  && apt-get install -y --no-install-recommends \
    wget \
    build-essential \
    unzip \
    cmake

WORKDIR /source

ARG OPENCV_VERSION=4.2.0

# Get opencv
RUN wget https://github.com/opencv/opencv/archive/${OPENCV_VERSION}.zip && \
    unzip ${OPENCV_VERSION}.zip && \
    rm ${OPENCV_VERSION}.zip && \
    mv opencv-${OPENCV_VERSION} opencv 
    # && wget https://github.com/opencv/opencv_contrib/archive/${OPENCV_VERSION}.zip && \
    # unzip ${OPENCV_VERSION}.zip && \
    # rm ${OPENCV_VERSION}.zip && \
    # mv opencv_contrib-${OPENCV_VERSION} opencv_contrib


# Build opencv
#    -D OPENCV_EXTRA_MODULES_PATH=/source/opencv_contrib/modules \

WORKDIR /build/opencv
RUN cmake \
    -DCMAKE_TOOLCHAIN_FILE="/source/opencv/platforms/linux/aarch64-gnu.toolchain.cmake" \
    -D CMAKE_INSTALL_PREFIX=/install/opencv \
    -D CMAKE_BUILD_TYPE=RELEASE \
    -D BUIlD_SHARED_LIBS=OFF \
    -D BUILD_DOCS=OFF \
    -D BUILD_EXAMPLES=OFF \
    -D BUILD_opencv_apps=OFF \
    -D WITH_CAROTENE=OFF \
    -D BUILD_opencv_python2=OFF \
    -D BUILD_opencv_python3=OFF \
    -D BUILD_PERF_TESTS=OFF \
    -D BUILD_TESTS=OFF \
    -D FORCE_VTK=OFF \
    -D WITH_FFMPEG=OFF \
    -D WITH_GDAL=OFF \
    -D WITH_IPP=OFF \
    -D WITH_OPENEXR=OFF \
    -D WITH_OPENGL=OFF \
    -D WITH_QT=OFF \
    -D WITH_XINE=OFF \
    -D BUILD_JPEG=ON \
    -D BUILD_ZLIB=ON \
    -D BUILD_PNG=ON \
    -D BUILD_TIFF=OFF \
    -D BUILD_JASPER=OFF \
    -D WITH_ITT=OFF \
    -D WITH_LAPACK=OFF \
    -D WITH_OPENCL=OFF \
    -D WITH_TIFF=OFF \
    -D WITH_PNG=ON \
    -D WITHOPENCLAMDFFT=OFF \
    -D WITHOPENCLAMDBLAS=OFF \
    -D WITH_VA_INTEL=OFF \
    -D WITH_WEBP=OFF \
    -D WITH_JASPER=OFF \
    /source/opencv

RUN make -j$(nproc)
RUN make install

ARG DLDT_VERSION=2019_R3.1

WORKDIR /source

# Get dldt
RUN wget https://github.com/opencv/dldt/archive/${DLDT_VERSION}.zip && \
    unzip ${DLDT_VERSION}.zip && \
    rm ${DLDT_VERSION}.zip && \
    mv dldt-${DLDT_VERSION} dldt

WORKDIR /build/inference_engine

RUN cmake \
    -D CMAKE_TOOLCHAIN_FILE=/source/dldt/inference_engine/cmake/arm64.toolchain.cmake \
    -D CMAKE_BUILD_TYPE=Release \
    -D ENABLE_GNA=OFF \
    /source/dldt/inference_engine
