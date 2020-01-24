#! /bin/bash

. ../variables.sh

docker manifest create "${BASE_IMAGE}:${VERSION}" "${BASE_IMAGE}:${X64_VERSION}" "${BASE_IMAGE}:${ARM32_VERSION}" "${BASE_IMAGE}:${ARM64_VERSION}"
docker manifest push "${BASE_IMAGE}:${VERSION}"

