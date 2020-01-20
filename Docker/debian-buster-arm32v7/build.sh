VERSION="1.1.1"

docker buildx build --platform linux/arm/v7 -t "nrandell/dotnet-arm-openvino:$VERSION" -f Dockerfile --push ../..


