VERSION="1.0.4"

docker buildx build --platform linux/arm/v7 -t "nrandell/arm-opencv-nick-builder:$VERSION" -f Dockerfile --push ../..


