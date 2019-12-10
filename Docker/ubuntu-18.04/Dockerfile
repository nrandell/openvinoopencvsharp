FROM ubuntu:18.04 as build-native-env

ARG DOWNLOAD_LINK=http://registrationcenter-download.intel.com/akdlm/irc_nas/16057/l_openvino_toolkit_p_2019.3.376.tgz
ARG INSTALL_DIR=/opt/intel/openvino
ARG TEMP_DIR=/tmp/openvino_installer
RUN apt-get update && apt-get install -y --no-install-recommends \
    wget \
    cpio \
    sudo \
    lsb-release && \
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

RUN $INSTALL_DIR/install_dependencies/install_openvino_dependencies.sh

WORKDIR /OpenVinoOpenCvSharp
COPY CMakeLists.txt .
COPY OpenVinoOpenCvSharpExtern/ OpenVinoOpenCvSharpExtern/

WORKDIR /OpenVinoOpenCvSharp/build/
RUN cmake \
        -D OpenCV_DIR=/opt/intel/openvino/opencv/cmake \
        -D CMAKE_INSTALL_PREFIX=../install \
        ..

RUN cmake --build . --target install --config Release

FROM mcr.microsoft.com/dotnet/core/sdk:3.1.100-bionic AS build-dotnet-env
WORKDIR /build

COPY OpenVinoOpenCvSharp/OpenVinoOpenCvSharp.csproj OpenVinoOpenCvSharp/
COPY PeopleDetectionOpenCV/PeopleDetectionOpenCV.csproj PeopleDetectionOpenCV/
COPY Shared/Shared.csproj Shared/
RUN dotnet restore PeopleDetectionOpenCV/PeopleDetectionOpenCV.csproj

COPY OpenVinoOpenCvSharp/ OpenVinoOpenCvSharp/
COPY PeopleDetectionOpenCV/ PeopleDetectionOpenCV/
COPY Shared/ Shared/

WORKDIR /build/PeopleDetectionOpenCV
RUN dotnet build -c Release

FROM build-dotnet-env AS publish
RUN dotnet publish --no-build -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/core/runtime:3.1.0-bionic AS dotnet-runtime

RUN apt update
RUN apt install -y --no-install-recommends software-properties-common
RUN add-apt-repository -y ppa:jonathonf/ffmpeg-4
RUN apt update
RUN apt install -y --no-install-recommends ffmpeg

WORKDIR /app
COPY --from=build-native-env /opt/intel/ /opt/intel/
RUN /opt/intel/openvino/install_dependencies/install_openvino_dependencies.sh

RUN apt install -y --no-install-recommends libgdiplus
RUN echo "deb http://ppa.launchpad.net/intel-opencl/intel-opencl/ubuntu bionic main" >/etc/apt/sources.list.d/intel-opencl.list
RUN apt-key adv --keyserver keyserver.ubuntu.com --recv-keys B9732172C4830B8F
RUN apt update
RUN apt install -y intel-opencl-icd clinfo
COPY --from=build-native-env /OpenVinoOpenCvSharp/install/lib/libOpenVinoOpenCvSharpExtern.so .


COPY --from=publish /app/publish .

COPY PeopleDetectionOpenCV/run.sh .
RUN chmod a+x run.sh

ENTRYPOINT [ "./run.sh" ]
