#! /bin/bash

. ../variables.sh

docker buildx build --platform linux/arm64 --build-arg DLDT_VERSION=${DLDT_VERSION} -t "${BASE_DLDT_IMAGE}:${DLDT_ARM64_VERSION}" -f Dockerfile --push .


