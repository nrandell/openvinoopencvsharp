
OPENCV_VERSION="4.1.2"
OPENCVSHARP_VERSION="4.1.1.20191217"

VERSION_INC=6
BASE_IMAGE="nrandell/openvino-dotnet-core-runtime"
ASPNET_IMAGE="nrandell/openvino-aspnet"
X64_VERSION="3.1.1-2019.3.376-${VERSION_INC}"
ARM32_VERSION="3.1.1-2019.3.334-${VERSION_INC}"
ARM64_VERSION="3.1.1_${OPENCV_VERSION}_2019_R3.1-${VERSION_INC}"

VERSION="3.1.1-${VERSION_INC}"
OPENCVSHARP_BUILD="4"
OPENCVSHARPEXTERN_VERSION="${OPENCVSHARP_VERSION}-${OPENCVSHARP_BUILD}"
