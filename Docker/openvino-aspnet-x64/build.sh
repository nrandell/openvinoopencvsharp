#! /bin/bash

. ../variables.sh


docker buildx build --platform linux/amd64 -t "nrandell/openvino-aspnet:$X64_VERSION" --build-arg BASE_IMAGE="${BASE_IMAGE}:${X64_VERSION}" -f Dockerfile --push .


