FROM mcr.microsoft.com/dotnet/core/runtime:3.1.2-bionic-arm64v8 AS dotnet-runtime

RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    software-properties-common \
    && add-apt-repository ppa:jonathonf/ffmpeg-4 \
    && apt-get update \
    && DEBIAN_FRONTEND=noninteractive apt-get install -y --no-install-recommends \
    ffmpeg \
    libgdk3.0-cil \
    libgdiplus \
    cpio \
    lsb-release \
    wget

FROM dotnet-runtime as native-build
ARG DLDT_VERSION

RUN apt-get update  && apt-get install -y --no-install-recommends \
    wget \
    build-essential \
    git \
    unzip \
    autoconf \
    cmake \
    automake \
    libtool

# Build lib usb without udev support to run in docker
WORKDIR /build
RUN wget https://github.com/libusb/libusb/archive/v1.0.22.zip \
    && unzip v1.0.22.zip \
    && cd libusb-1.0.22 \
    && ./bootstrap.sh \
    && ./configure --disable-udev --enable-shared \
    && make -j4 \
    && make install

# Build inference engine
WORKDIR /source
RUN git clone --no-checkout https://github.com/opencv/dldt.git
WORKDIR /source/dldt
RUN git checkout tags/${DLDT_VERSION}

WORKDIR /source/dldt/inference-engine
RUN git submodule init
RUN git submodule update --recursive

WORKDIR /source/dldt/build

RUN cmake \
    -D CMAKE_TOOLCHAIN_FILE=../cmake/arm64.toolchain.cmake \
    -D CMAKE_BUILD_TYPE=Release \
    -D ENABLE_OPENCV=OFF \
    -DENABLE_TESTS=OFF \
    -DENABLE_SAMPLES=OFF \
    -DENABLE_GNA=OFF \
    -DENABLE_PROFILING_ITT=OFF \
    -DENABLE_SAMPLES_CORE=OFF \
    -DENABLE_SEGMENTATION_TESTS=OFF \
    -DENABLE_OBJECT_DETECTION_TESTS=OFF \
    -DBUILD_TESTS=OFF \
    -DNGRAPH_UNIT_TEST_ENABLE=OFF \
    -DNGRAPH_TEST_UTIL_ENABLE=OFF \
    -DENABLE_C=ON \
    ..

RUN make -j$(nproc)


# FROM dotnet-runtime
# COPY --from=native-build ["/usr/local/lib/libusb-1.0.so.0.1.0", "/lib/aarch64-linux-gnu/libusb-1.0.so.0.1.0"]
# ENV LD_LIBRARY_PATH=/install/inference-engine/bin/aarch64/Release/lib


