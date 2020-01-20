
docker buildx build --platform linux/arm/v7 -t "nrandell/face-detection-openvino-arm:1.0.4" -f Dockerfile.arm32v7 --push ../