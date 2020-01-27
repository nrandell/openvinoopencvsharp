#! /bin/bash

. ../variables.sh

set -vx

docker pull "${ASPNET_IMAGE}:${X64_VERSION}" 
docker pull "${ASPNET_IMAGE}:${ARM32_VERSION}" 
docker pull "${ASPNET_IMAGE}:${ARM64_VERSION}"
docker manifest create "${ASPNET_IMAGE}:${VERSION}" "${ASPNET_IMAGE}:${X64_VERSION}" "${ASPNET_IMAGE}:${ARM32_VERSION}" "${ASPNET_IMAGE}:${ARM64_VERSION}"
docker manifest push "${ASPNET_IMAGE}:${VERSION}"

