FROM mcr.microsoft.com/dotnet/core/runtime:3.1.0-bionic AS dotnet-runtime

ARG DOWNLOAD_LINK=http://registrationcenter-download.intel.com/akdlm/irc_nas/16057/l_openvino_toolkit_p_2019.3.376.tgz
ARG INSTALL_DIR=/opt/intel/openvino
ARG TEMP_DIR=/tmp/openvino_installer

RUN apt-get update && apt-get install -y --no-install-recommends \
    ffmpeg \
    libgdiplus \
    cpio \
    lsb-release \
    wget && \
    rm -rf /var/lib/apt/lists/*
    
RUN mkdir -p $TEMP_DIR && cd $TEMP_DIR && \
    wget -c $DOWNLOAD_LINK && \
    tar xf l_openvino_toolkit*.tgz && \
    cd l_openvino_toolkit* 

RUN cd $TEMP_DIR && \
    cd l_openvino_toolkit* && \
    sed -i 's/decline/accept/g' silent.cfg && \
    ./install.sh -s silent.cfg && \
    rm -rf $TEMP_DIR


FROM dotnet-runtime as opencv-build

RUN apt-get update && apt-get install -y --no-install-recommends \
    build-essential \
    cmake && \
    rm -rf /var/lib/apt/lists/*


# Build OpenVinoOpenCvSharp
WORKDIR /OpenVinoOpenCvSharp
COPY CMakeLists.txt .
COPY OpenVinoOpenCvSharpExtern/ OpenVinoOpenCvSharpExtern/

WORKDIR /OpenVinoOpenCvSharp/build/
RUN cmake \
        -D OpenCV_DIR=/opt/intel/openvino/opencv/cmake \
        -D CMAKE_INSTALL_PREFIX=../install \
        ..

RUN make -j4 && \
    make install

FROM dotnet-runtime as final
WORKDIR /app

RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    software-properties-common \
    gpg-agent \
    dirmngr && \
    apt-get clean

RUN apt-key adv --keyserver keyserver.ubuntu.com --recv-keys B9732172C4830B8F && \
    add-apt-repository -y ppa:savoury1/ffmpeg4 && \
    add-apt-repository -y ppa:savoury1/graphics && \
    add-apt-repository -y ppa:savoury1/multimedia

RUN echo "deb http://ppa.launchpad.net/intel-opencl/intel-opencl/ubuntu bionic main" >/etc/apt/sources.list.d/intel-opencl.list

RUN apt-get update && apt-get install -y --no-install-recommends \
    ffmpeg \
    intel-opencl-icd \
    clinfo && \
    apt autoremove -y && \
    rm -rf /var/lib/apt/lists/*

COPY --from=opencv-build /OpenVinoOpenCvSharp/install/lib/libOpenVinoOpenCvSharpExtern.so /opt/

ENV LD_LIBRARY_PATH=/opt:/opt/intel/openvino/opencv/lib:/opt/intel/openvino/deployment_tools/inference_engine/external/hddl/lib:/opt/intel/openvino/deployment_tools/inference_engine/external/gna/lib:/opt/intel/openvino/deployment_tools/inference_engine/external/mkltiny_lnx/lib:/opt/intel/openvino/deployment_tools/inference_engine/external/tbb/lib:/opt/intel/openvino/deployment_tools/inference_engine/lib/intel64:/opt/intel/openvino/openvx/lib
