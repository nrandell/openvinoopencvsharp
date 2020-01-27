#! /bin/bash

. ../variables.sh


docker buildx build --platform linux/arm64 -t "nrandell/openvino-aspnet:$ARM64_VERSION" --build-arg BASE_IMAGE="${BASE_IMAGE}:${ARM64_VERSION}" -f Dockerfile --push .


