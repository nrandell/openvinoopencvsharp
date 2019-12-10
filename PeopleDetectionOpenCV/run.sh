#! /bin/bash
ARGS="$@"

. /opt/intel/openvino/bin/setupvars.sh

exec dotnet PeopleDetectionOpenCV.dll $ARGS
