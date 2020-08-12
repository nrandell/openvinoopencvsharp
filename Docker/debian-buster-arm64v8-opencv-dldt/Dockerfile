FROM mcr.microsoft.com/dotnet/core/runtime:3.1.1-buster-slim-arm64v8 as dotnet-runtime

RUN apt-get update && apt-get install -y --no-install-recommends \
    ffmpeg \
    libgdiplus \
    libgdk3.0-cil

FROM dotnet-runtime as native-build

ARG OPENCV_VERSION=4.1.2
ARG DLDT_VERSION=2019_R3.1


RUN apt-get update  && apt-get install -y --no-install-recommends \
    wget \
    build-essential \
    unzip \
    cmake \
    libusb-1.0-0-dev \
    git \
    libpython3-dev \
    python3-pip

# Build inference engine
WORKDIR /source
RUN git clone --no-checkout https://github.com/openvinotoolkit/openvino
WORKDIR /source/dldt
RUN git checkout tags/${DLDT_VERSION}

WORKDIR /source/dldt/inference-engine
RUN git submodule init
RUN git submodule update --recursive

WORKDIR /install
RUN mv /source/dldt/inference-engine .
WORKDIR /install/inference-engine/build

RUN cmake \
    -D CMAKE_TOOLCHAIN_FILE=../cmake/arm64.toolchain.cmake \
    -D CMAKE_BUILD_TYPE=Release \
    -D ENABLE_OPENCV=OFF \
    -D ENABLE_ROCKHOPER=OFF \
    -D ENABLE_SAMPLES=OFF \
    -D ENABLE_TESTS=OFF \
    -D ENABLE_CLDNN=OFF \
    -D ENABLE_GNA=OFF \
    -D ENABLE_SSE42=OFF \
    -D THREADING=SEQ \
    -D ENABLE_MKL_DNN=OFF \
    -D GEMM=JIT \
    -D THREADS_PTHREAD_ARG="-pthread" \
    ..

RUN make -j$(nproc)
   
# Get opencv
WORKDIR /source
RUN wget https://github.com/opencv/opencv/archive/${OPENCV_VERSION}.zip && \
    unzip ${OPENCV_VERSION}.zip && \
    rm ${OPENCV_VERSION}.zip && \
    mv opencv-${OPENCV_VERSION} opencv 
    # && wget https://github.com/opencv/opencv_contrib/archive/${OPENCV_VERSION}.zip && \
    # unzip ${OPENCV_VERSION}.zip && \
    # rm ${OPENCV_VERSION}.zip && \
    # mv opencv_contrib-${OPENCV_VERSION} opencv_contrib

WORKDIR /build/opencv
RUN cmake \
    -D InferenceEngine_DIR=/install/inference-engine/build \
    -D CMAKE_TOOLCHAIN_FILE="/source/opencv/platforms/linux/aarch64-gnu.toolchain.cmake" \
    -D CMAKE_INSTALL_PREFIX=/install/opencv \
    -D CMAKE_BUILD_TYPE=RELEASE \
    -D BUIlD_SHARED_LIBS=OFF \
    -D BUILD_DOCS=OFF \
    -D BUILD_EXAMPLES=OFF \
    -D BUILD_opencv_apps=OFF \
    -D BUILD_opencv_python2=OFF \
    -D BUILD_opencv_python3=OFF \
    -D BUILD_PERF_TESTS=OFF \
    -D BUILD_TESTS=OFF \
    -D FORCE_VTK=OFF \
    -D WITH_INF_ENGINE=ON \
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

WORKDIR /build

RUN apt-get update && apt-get install -y --no-install-recommends \
    automake \
    libtool


RUN  wget https://github.com/libusb/libusb/archive/v1.0.22.zip \
    && unzip v1.0.22.zip && cd libusb-1.0.22 \
    && ./bootstrap.sh \
    && ./configure --disable-udev --enable-shared \
    && make -j4 \
    && make install

FROM dotnet-runtime as final


COPY --from=native-build /install /install
COPY --from=native-build ["/usr/local/lib/libusb-1.0.so.0.1.0", "/lib/aarch64-linux-gnu/libusb-1.0.so.0.1.0"]
ENV LD_LIBRARY_PATH=/install/opencv/lib:/install/inference-engine/bin/aarch64/Release/lib

