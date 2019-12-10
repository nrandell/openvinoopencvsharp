$VERSION="1.0.3"

docker buildx build --platform linux/arm/v7 -t "nrandell/arm-nick-builder:$VERSION" -f Dockerfile --push ../..


