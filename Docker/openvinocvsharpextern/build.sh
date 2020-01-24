#! /bin/bash

set -evx

. ../variables.sh

docker buildx build --platform linux/arm/v7 -t "nrandell/openvinocvsharpextern:arm32-${OPENCVSHARPEXTERN_VERSION}" --build-arg VERSION=${VERSION} --build-arg OPENCVSHARP_VERSION=${OPENCVSHARP_VERSION} -f Dockerfile.debian-buster-arm32v7 --push .

docker buildx build --platform linux/amd64 -t "nrandell/openvinocvsharpextern:x64-${OPENCVSHARPEXTERN_VERSION}" --build-arg VERSION=${VERSION} --build-arg OPENCVSHARP_VERSION=${OPENCVSHARP_VERSION} -f Dockerfile.ubuntu-bionic-x64 --push .

docker buildx build --platform linux/arm64 -t "nrandell/openvinocvsharpextern:arm64-${OPENCVSHARPEXTERN_VERSION}" --build-arg VERSION=${VERSION} --build-arg OPENCVSHARP_VERSION=${OPENCVSHARP_VERSION} -f Dockerfile.debian-buster-arm64v8 --push .

