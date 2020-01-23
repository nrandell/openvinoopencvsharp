FROM mcr.microsoft.com/dotnet/core/runtime:3.1.1-bionic AS dotnet-runtime



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

ARG DOWNLOAD_LINK=http://registrationcenter-download.intel.com/akdlm/irc_nas/16057/l_openvino_toolkit_p_2019.3.376.tgz
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

ENV LD_LIBRARY_PATH=/opt:/opt/intel/openvino/opencv/lib:/opt/intel/openvino/deployment_tools/inference_engine/external/hddl/lib:/opt/intel/openvino/deployment_tools/inference_engine/external/gna/lib:/opt/intel/openvino/deployment_tools/inference_engine/external/mkltiny_lnx/lib:/opt/intel/openvino/deployment_tools/inference_engine/external/tbb/lib:/opt/intel/openvino/deployment_tools/inference_engine/lib/intel64:/opt/intel/openvino/openvx/lib
