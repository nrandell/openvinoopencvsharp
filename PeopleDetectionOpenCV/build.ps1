$BUILD_VERSION="0.0.23"

docker buildx build --platform linux/arm/v7,linux/amd64 -t nrandell/pdopencv:${BUILD_VERSION} --push -f Dockerfile ..

