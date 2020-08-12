FROM mcr.microsoft.com/dotnet/runtime:5.0.0-preview.7-focal AS dotnet-runtime
    # apt-get install -y --no-install-recommends \
    # software-properties-common \
    # && add-apt-repository ppa:jonathonf/ffmpeg-4 \
    # && apt-get update \
    # && DEBIAN_FRONTEND=noninteractive apt-get install -y --no-install-recommends \
    # libgdk3.0-cil \
    # libgdiplus \

RUN DEBIAN_FRONTEND=noninteractive \
    apt-get update && \
    apt-get install -y --no-install-recommends \
    ffmpeg \
    cpio \
    lsb-release \
    wget

ARG DOWNLOAD_LINK=http://registrationcenter-download.intel.com/akdlm/irc_nas/16803/l_openvino_toolkit_p_2020.4.287.tgz
ARG INSTALL_DIR=/opt/intel/openvino
ARG TEMP_DIR=/tmp/openvino_installer

    
RUN mkdir -p $TEMP_DIR && cd $TEMP_DIR && \
    wget -c $DOWNLOAD_LINK && \
    tar xf l_openvino_toolkit*.tgz && \
    cd l_openvino_toolkit* 

RUN cd $TEMP_DIR && \
    cd l_openvino_toolkit* && \
    sed -i 's/decline/accept/g' silent.cfg && \
    ./install.sh -s silent.cfg && \
    rm -rf $TEMP_DIR

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
COPY --from=native-build ["/usr/local/lib/libusb-1.0.so.0.1.0", "/lib/x86_64-linux-gnu/libusb-1.0.so.0.1.0"]

ENV LD_LIBRARY_PATH=/opt:/opt/intel/openvino/opencv/lib:/opt/intel/openvino/deployment_tools/inference_engine/external/hddl/lib:/opt/intel/openvino/deployment_tools/inference_engine/external/gna/lib:/opt/intel/openvino/deployment_tools/inference_engine/external/mkltiny_lnx/lib:/opt/intel/openvino/deployment_tools/inference_engine/external/tbb/lib:/opt/intel/openvino/deployment_tools/inference_engine/lib/intel64:/opt/intel/openvino/openvx/lib
