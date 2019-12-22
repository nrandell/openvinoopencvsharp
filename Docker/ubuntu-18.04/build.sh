VERSION="1.1.2"
IMAGE="nrandell/dotnet-openvino-opencv:$VERSION"

docker build -t "$IMAGE" -f Dockerfile ../..
docker push "$IMAGE"



