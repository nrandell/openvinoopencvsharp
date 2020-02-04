FROM mcr.microsoft.com/dotnet/core/runtime:3.1.1-buster-slim-arm32v7 AS dotnet-runtime

ARG DOWNLOAD_LINK=https://download.01.org/opencv/2019/openvinotoolkit/R3/l_openvino_toolkit_runtime_raspbian_p_2019.3.334.tgz
ARG INSTALL_DIR=/opt/intel/openvino
ARG TEMP_DIR=/tmp/openvino_installer

RUN apt-get update && apt-get install -y --no-install-recommends \
    ffmpeg \
    libgdiplus \
    libgdk3.0-cil \
    wget

RUN mkdir -p $TEMP_DIR \
    && mkdir -p $INSTALL_DIR \
    && cd $TEMP_DIR \
    && wget -c $DOWNLOAD_LINK \
    && tar -xf  l_openvino_toolkit_runtime_raspbian_p_*.tgz --strip 1 -C $INSTALL_DIR \
    && cd / \
    && rm -rf ${TEMP_DIR}

FROM dotnet-runtime as native-build
RUN apt-get update  && apt-get install -y --no-install-recommends \
    wget \
    build-essential \
    unzip \
    autoconf \
    automake \
    libtool

WORKDIR /build
RUN wget https://github.com/libusb/libusb/archive/v1.0.22.zip \
    && unzip v1.0.22.zip \
    && cd libusb-1.0.22 \
    && ./bootstrap.sh \
    && ./configure --disable-udev --enable-shared \
    && make -j4 \
    && make install


FROM dotnet-runtime
COPY --from=native-build ["/usr/local/lib/libusb-1.0.so.0.1.0", "/lib/arm-linux-gnueabihf/libusb-1.0.so.0.1.0"]

ENV LD_LIBRARY_PATH=/opt/intel/openvino/opencv/lib:/opt/intel/openvino/inference_engine/lib/armv7l

