#! /bin/bash

. ../variables.sh


docker buildx build --platform linux/arm/v7 -t "nrandell/openvino-aspnet:$ARM32_VERSION" --build-arg BASE_IMAGE="${BASE_IMAGE}:${ARM32_VERSION}" -f Dockerfile --push .


