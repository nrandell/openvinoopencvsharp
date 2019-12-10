#ifndef _CPP_CORE_H_
#define _CPP_CORE_H_

#include "include_opencv.h"

#pragma region Miscellaneous

CVAPI(void) core_setNumThreads(int nthreads)
{
    cv::setNumThreads(nthreads);
}
CVAPI(int) core_getNumThreads()
{
    return cv::getNumThreads();
}
CVAPI(int) core_getThreadNum()
{
    return cv::getThreadNum();
}

CVAPI(void) core_getBuildInformation(char *buf, int maxLength)
{
    const cv::String &str = cv::getBuildInformation();
    copyString(str, buf, maxLength);
}
CVAPI(int) core_getBuildInformation_length()
{
    const cv::String &str = cv::getBuildInformation();
    return static_cast<int>(str.length());
}

CVAPI(void) core_getVersionString(char *buf, int bufLength)
{
    const std::string &str = cv::getVersionString();
    copyString(str, buf, bufLength);
}

CVAPI(int) core_getVersionMajor()
{
    return cv::getVersionMajor();
}

CVAPI(int) core_getVersionMinor()
{
    return cv::getVersionMinor();
}

CVAPI(int) core_getVersionRevision()
{
    return cv::getVersionRevision();
}

CVAPI(int64) core_getTickCount()
{
    return cv::getTickCount();
}
CVAPI(double) core_getTickFrequency()
{
    return cv::getTickFrequency();
}
CVAPI(int64) core_getCPUTickCount()
{
    return cv::getCPUTickCount();
}

CVAPI(int) core_checkHardwareSupport(int feature)
{
    return cv::checkHardwareSupport(feature) ? 1 : 0;
}

CVAPI(void) core_getHardwareFeatureName(int feature, char *buf, int bufLength)
{
    const cv::String &str = cv::getHardwareFeatureName(feature);
    copyString(str, buf, bufLength);
}

CVAPI(void) core_getCPUFeaturesLine(char *buf, int bufLength)
{
    const cv::String &str = cv::getCPUFeaturesLine();
    copyString(str, buf, bufLength);
}

CVAPI(int) core_getNumberOfCPUs()
{
    return cv::getNumberOfCPUs();
}

CVAPI(void*) core_fastMalloc(size_t bufSize)
{
    return cv::fastMalloc(bufSize);
}
CVAPI(void) core_fastFree(void *ptr)
{
    return cv::fastFree(ptr);
}

CVAPI(void) core_setUseOptimized(int onoff)
{
    cv::setUseOptimized(onoff != 0);
}
CVAPI(int) core_useOptimized()
{
    return cv::useOptimized() ? 1 : 0;
}

CVAPI(void) core_glob(const char *pattern, std::vector<std::string> *result, int recursive)
{
    cv::glob(pattern, *result, recursive != 0);
}

CVAPI(int) core_setBreakOnError(int flag)
{
    return cv::setBreakOnError(flag != 0) ? 1 : 0;
}

CVAPI(cv::ErrorCallback) redirectError(cv::ErrorCallback errCallback, void* userdata, void** prevUserdata)
{
    return cv::redirectError(errCallback, userdata, prevUserdata);
}

CVAPI(char*) core_format(cv::_InputArray *mtx, int fmt)
{
    auto formatted = cv::format(*mtx, static_cast<cv::Formatter::FormatType>(fmt));

    std::stringstream s;
    s << formatted;
    std::string str = s.str();

    const char *src = str.c_str();
    char *dst = new char[str.length() + 1];
    std::memcpy(dst, src, str.length() + 1);
    return dst;
}
CVAPI(void) core_char_delete(char *buf)
{
    delete[] buf;
}

#pragma endregion

#pragma region Array Operations

CVAPI(void) core_add(cv::_InputArray *src1, cv::_InputArray *src2, cv::_OutputArray *dst, cv::_InputArray *mask, int dtype)
{
    cv::add(*src1, *src2, *dst, entity(mask), dtype);
}

CVAPI(void) core_subtract_InputArray2(cv::_InputArray *src1, cv::_InputArray *src2, cv::_OutputArray *dst, cv::_InputArray *mask, int dtype)
{
    cv::subtract(*src1, *src2, *dst, entity(mask), dtype);
}
CVAPI(void) core_subtract_InputArrayScalar(cv::_InputArray *src1, MyCvScalar src2, cv::_OutputArray *dst, cv::_InputArray *mask, int dtype)
{
	cv::Scalar src2_ = cpp(src2);
	cv::subtract(*src1, src2_, *dst, entity(mask), dtype);
}
CVAPI(void) core_subtract_ScalarInputArray(MyCvScalar src1, cv::_InputArray *src2, cv::_OutputArray *dst, cv::_InputArray *mask, int dtype)
{
	cv::Scalar src1_ = cpp(src1);
	cv::subtract(src1_, *src2, *dst, entity(mask), dtype);
}

CVAPI(void) core_multiply(cv::_InputArray *src1, cv::_InputArray *src2, cv::_OutputArray *dst, double scale, int dtype)
{
    cv::multiply(*src1, *src2, *dst, scale, dtype);
}
CVAPI(void) core_divide1(double scale, cv::_InputArray *src2, cv::_OutputArray *dst, int dtype)
{
    cv::divide(scale, *src2, *dst, dtype);
}
CVAPI(void) core_divide2(cv::_InputArray *src1, cv::_InputArray *src2, cv::_OutputArray *dst, double scale, int dtype)
{
    cv::divide(*src1, *src2, *dst, scale, dtype);
}

CVAPI(void) core_scaleAdd(cv::_InputArray *src1, double alpha, cv::_InputArray *src2, cv::_OutputArray *dst)
{
    cv::scaleAdd(*src1, alpha, *src2, *dst);
}
CVAPI(void) core_addWeighted(cv::_InputArray *src1, double alpha, cv::_InputArray *src2,
    double beta, double gamma, cv::_OutputArray *dst, int dtype)
{
    cv::addWeighted(*src1, alpha, *src2, beta, gamma, *dst, dtype);
}

#pragma endregion

CVAPI(int) core_borderInterpolate(int p, int len, int borderType)
{
    return cv::borderInterpolate(p, len, borderType);
}

CVAPI(void) core_copyMakeBorder(cv::_InputArray *src, cv::_OutputArray *dst,
    int top, int bottom, int left, int right, int borderType, MyCvScalar value)
{
    cv::copyMakeBorder(*src, *dst, top, bottom, left, right, borderType, cpp(value));
}

CVAPI(void) core_convertScaleAbs(cv::_InputArray *src, cv::_OutputArray *dst, double alpha, double beta)
{
    cv::convertScaleAbs(*src, *dst, alpha, beta);
}

CVAPI(void) core_LUT(cv::_InputArray *src, cv::_InputArray *lut, cv::_OutputArray *dst)
{
    cv::LUT(*src, *lut, *dst);
}
CVAPI(MyCvScalar) core_sum(cv::_InputArray *src)
{
    return c(cv::sum(*src));
}
CVAPI(int) core_countNonZero(cv::_InputArray *src)
{
    return cv::countNonZero(*src);
}
CVAPI(void) core_findNonZero(cv::_InputArray *src, cv::_OutputArray *idx)
{
    cv::findNonZero(*src, *idx);
}

CVAPI(MyCvScalar) core_mean(cv::_InputArray *src, cv::_InputArray *mask)
{
    return c(cv::mean(*src, entity(mask)));
}

CVAPI(void) core_meanStdDev_OutputArray(
    cv::_InputArray *src, cv::_OutputArray *mean, cv::_OutputArray *stddev, cv::_InputArray *mask)
{
    cv::meanStdDev(*src, *mean, *stddev, entity(mask));
}
CVAPI(void) core_meanStdDev_Scalar(
    cv::_InputArray *src, MyCvScalar *mean, MyCvScalar *stddev, cv::_InputArray *mask)
{
    cv::Scalar mean0, stddev0;
    cv::meanStdDev(*src, mean0, stddev0, entity(mask));
    *mean = c(mean0);
    *stddev = c(stddev0);
}

CVAPI(double) core_norm1(cv::_InputArray *src1, int normType, cv::_InputArray *mask)
{
    return cv::norm(*src1, normType, entity(mask));
}
CVAPI(double) core_norm2(cv::_InputArray *src1, cv::_InputArray *src2,
                         int normType, cv::_InputArray *mask)
{
    return cv::norm(*src1, *src2, normType, entity(mask));
}

CVAPI(void) core_batchDistance(cv::_InputArray *src1, cv::_InputArray *src2,
                                cv::_OutputArray *dist, int dtype, cv::_OutputArray *nidx,
                                int normType, int K, cv::_InputArray *mask, 
                                int update, int crosscheck)
{
    cv::batchDistance(*src1, *src2, *dist, dtype, *nidx, normType, K, entity(mask), update, crosscheck != 0);
}

CVAPI(void) core_normalize(cv::_InputArray *src, cv::_InputOutputArray *dst, double alpha, double beta,
    int normType, int dtype, cv::_InputArray *mask)
{
    cv::InputArray maskVal = entity(mask);
    cv::normalize(*src, *dst, alpha, beta, normType, dtype, maskVal);
}

CVAPI(void) core_minMaxLoc1(cv::_InputArray *src, double *minVal, double *maxVal)
{
    cv::minMaxLoc(*src, minVal, maxVal);
}
CVAPI(void) core_minMaxLoc2(cv::_InputArray *src, double *minVal, double *maxVal,
    CvPoint *minLoc, CvPoint *maxLoc, cv::_InputArray *mask)
{
    cv::InputArray maskVal = entity(mask);
    cv::Point minLoc0, maxLoc0;
    cv::minMaxLoc(*src, minVal, maxVal, &minLoc0, &maxLoc0, maskVal);
    *minLoc = minLoc0;
    *maxLoc = maxLoc0;
}
CVAPI(void) core_minMaxIdx1(cv::_InputArray *src, double *minVal, double *maxVal)
{
    cv::minMaxIdx(*src, minVal, maxVal);
}
CVAPI(void) core_minMaxIdx2(cv::_InputArray *src, double *minVal, double *maxVal,
    int *minIdx, int *maxIdx, cv::_InputArray *mask)
{
    cv::InputArray maskVal = entity(mask);
    cv::minMaxIdx(*src, minVal, maxVal, minIdx, maxIdx, maskVal);
}

CVAPI(void) core_reduce(cv::_InputArray *src, cv::_OutputArray *dst, int dim, int rtype, int dtype)
{
    cv::reduce(*src, *dst, dim, rtype, dtype);
}
CVAPI(void) core_merge(cv::Mat **mv, uint32 count, cv::Mat *dst)
{
    std::vector<cv::Mat> vec((size_t)count);
    for (uint32 i = 0; i < count; i++)
        vec[i] = *mv[i];
    
    cv::merge(vec, *dst);
}
CVAPI(void) core_split(cv::Mat *src, std::vector<cv::Mat> **mv)
{
    *mv = new std::vector<cv::Mat>();
    cv::split(*src, **mv);
}
CVAPI(void) core_mixChannels(cv::Mat **src, uint32 nsrcs, cv::Mat **dst, uint32 ndsts, int *fromTo, uint32 npairs)
{
    std::vector<cv::Mat> srcVec((size_t)nsrcs);
    std::vector<cv::Mat> dstVec((size_t)ndsts);
    for (uint32 i = 0; i < nsrcs; i++)
        srcVec[i] = *(src[i]);
    for (uint32 i = 0; i < ndsts; i++)
        dstVec[i] = *(dst[i]);

    cv::mixChannels(srcVec, dstVec, fromTo, npairs);
}

CVAPI(void) core_extractChannel(cv::_InputArray *src, cv::_OutputArray *dst, int coi)
{
    cv::extractChannel(*src, *dst, coi);
}
CVAPI(void) core_insertChannel(cv::_InputArray *src, cv::_InputOutputArray *dst, int coi)
{
    cv::insertChannel(*src, *dst, coi);
}
CVAPI(void) core_flip(cv::_InputArray *src, cv::_OutputArray *dst, int flipCode)
{
    cv::flip(*src, *dst, flipCode);
}
CVAPI(void) core_repeat1(cv::_InputArray *src, int ny, int nx, cv::_OutputArray *dst)
{
    cv::repeat(*src, ny, nx, *dst);
}
CVAPI(cv::Mat*) core_repeat2(cv::Mat *src, int ny, int nx)
{
    cv::Mat ret = cv::repeat(*src, ny, nx);
    return new cv::Mat(ret);
}
CVAPI(void) core_hconcat1(cv::Mat **src, uint32 nsrc, cv::_OutputArray *dst)
{
    std::vector<cv::Mat> srcVec((size_t)nsrc);
    for (uint32 i = 0; i < nsrc; i++)
        srcVec[i] = *(src[i]);
    cv::hconcat(&srcVec[0], nsrc, *dst);
}
CVAPI(void) core_hconcat2(cv::_InputArray *src1, cv::_InputArray *src2, cv::_OutputArray *dst)
{
    cv::hconcat(*src1, *src2, *dst);
}
CVAPI(void) core_vconcat1(cv::Mat **src, uint32 nsrc, cv::_OutputArray *dst)
{
    std::vector<cv::Mat> srcVec((size_t)nsrc);
    for (uint32 i = 0; i < nsrc; i++)
        srcVec[i] = *(src[i]);
    cv::vconcat(&srcVec[0], nsrc, *dst);
}
CVAPI(void) core_vconcat2(cv::_InputArray *src1, cv::_InputArray *src2, cv::_OutputArray *dst)
{
    cv::vconcat(*src1, *src2, *dst);
}

CVAPI(void) core_bitwise_and(cv::_InputArray *src1, cv::_InputArray *src2,
                        cv::_OutputArray *dst, cv::_InputArray *mask)
{
    cv::bitwise_and(*src1, *src2, *dst, entity(mask));
}
CVAPI(void) core_bitwise_or(cv::_InputArray *src1, cv::_InputArray *src2,
                       cv::_OutputArray *dst, cv::_InputArray *mask)
{
    cv::bitwise_or(*src1, *src2, *dst, entity(mask));
}
CVAPI(void) core_bitwise_xor(cv::_InputArray *src1, cv::_InputArray *src2,
                        cv::_OutputArray *dst, cv::_InputArray *mask)
{
    cv::bitwise_xor(*src1, *src2, *dst, entity(mask));
}
CVAPI(void) core_bitwise_not(cv::_InputArray *src, cv::_OutputArray *dst,
                        cv::_InputArray *mask)
{
    cv::bitwise_not(*src, *dst, entity(mask));
}

CVAPI(void) core_absdiff(cv::_InputArray *src1, cv::_InputArray *src2, cv::_OutputArray *dst)
{
    cv::absdiff(*src1, *src2, *dst);
}

CVAPI(void) core_inRange_InputArray(cv::_InputArray *src, cv::_InputArray *lowerb, cv::_InputArray *upperb, cv::_OutputArray *dst)
{
    cv::inRange(*src, *lowerb, *upperb, *dst);
}
CVAPI(void) core_inRange_Scalar(cv::_InputArray *src, CvScalar lowerb, CvScalar upperb, cv::_OutputArray *dst)
{
    cv::inRange(*src, cv::Scalar(lowerb), cv::Scalar(upperb), *dst);
}

CVAPI(void) core_compare(cv::_InputArray *src1, cv::_InputArray *src2, cv::_OutputArray *dst, int cmpop)
{
    cv::compare(*src1, *src2, *dst, cmpop);
}
CVAPI(void) core_min1(cv::_InputArray *src1, cv::_InputArray *src2, cv::_OutputArray *dst)
{
    cv::min(*src1, *src2, *dst);
}
CVAPI(void) core_max1(cv::_InputArray *src1, cv::_InputArray *src2, cv::_OutputArray *dst)
{
    cv::max(*src1, *src2, *dst);
}
CVAPI(void) core_min_MatMat(cv::Mat *src1, cv::Mat *src2, cv::Mat *dst)
{
    cv::min(*src1, *src2, *dst);
}
CVAPI(void) core_min_MatDouble(cv::Mat *src1, double src2, cv::Mat *dst)
{
    cv::min(*src1, src2, *dst);
}
CVAPI(void) core_max_MatMat(cv::Mat *src1, const cv::Mat *src2, cv::Mat *dst)
{
    cv::max(*src1, *src2, *dst);
}
CVAPI(void) core_max_MatDouble(cv::Mat *src1, double src2, cv::Mat *dst)
{
    cv::max(*src1, src2, *dst);
}
CVAPI(void) core_sqrt(cv::_InputArray *src, cv::_OutputArray *dst)
{
    cv::sqrt(*src, *dst);
}
CVAPI(void) core_pow_Mat(cv::_InputArray *src, double power, cv::_OutputArray *dst)
{
    cv::pow(*src, power, *dst); 
}
CVAPI(void) core_exp_Mat(cv::_InputArray *src, cv::_OutputArray *dst)
{
    cv::exp(*src, *dst);
}
CVAPI(void) core_log_Mat(cv::_InputArray *src, cv::_OutputArray *dst)
{
    cv::log(*src, *dst);
}
CVAPI(float) core_cubeRoot(float val)
{
    return cv::cubeRoot(val);
}
CVAPI(float) core_fastAtan2(float y, float x)
{
    return cv::fastAtan2(y, x);
}

CVAPI(void) core_polarToCart(cv::_InputArray *magnitude, cv::_InputArray *angle,
    cv::_OutputArray *x, cv::_OutputArray *y, int angleInDegrees)
{
    cv::polarToCart(*magnitude, *angle, *x, *y, angleInDegrees != 0);
}
CVAPI(void) core_cartToPolar(cv::_InputArray *x, cv::_InputArray *y,
    cv::_OutputArray *magnitude, cv::_OutputArray *angle, int angleInDegrees)
{
    cv::cartToPolar(*x, *y, *magnitude, *angle, angleInDegrees != 0);
}
CVAPI(void) core_phase(cv::_InputArray *x, cv::_InputArray *y, cv::_OutputArray *angle, int angleInDegrees)
{
    cv::phase(*x, *y, *angle, angleInDegrees != 0);
}
CVAPI(void) core_magnitude_Mat(cv::_InputArray *x, cv::_InputArray *y, cv::_OutputArray *magnitude)
{
    cv::magnitude(*x, *y, *magnitude);
}
CVAPI(int) core_checkRange(cv::_InputArray *a, int quiet, CvPoint *pos, double minVal, double maxVal)
{
    cv::Point pos0;
    int ret = cv::checkRange(*a, quiet != 0, &pos0, minVal, maxVal);
    *pos = pos0;
    return ret;
}
CVAPI(void) core_patchNaNs(cv::_InputOutputArray *a, double val)
{
    cv::patchNaNs(*a, val);
}
CVAPI(void) core_gemm(cv::_InputArray *src1, cv::_InputArray *src2, double alpha,
    cv::_InputArray *src3, double gamma, cv::_OutputArray *dst, int flags)
{
    cv::gemm(*src1, *src2, alpha, *src3, gamma, *dst, flags);
}
CVAPI(void) core_mulTransposed(cv::_InputArray *src, cv::_OutputArray *dst, int aTa,
    cv::_InputArray *delta, double scale, int dtype)
{
    cv::mulTransposed(*src, *dst, aTa != 0, entity(delta), scale, dtype);
}
CVAPI(void) core_transpose(cv::_InputArray *src, cv::_OutputArray *dst)
{
    cv::transpose(*src, *dst);
}
CVAPI(void) core_transform(cv::_InputArray *src, cv::_OutputArray *dst, cv::_InputArray *m)
{
    cv::transform(*src, *dst, *m);
}

CVAPI(void) core_perspectiveTransform(cv::_InputArray *src, cv::_OutputArray *dst, cv::_InputArray *m)
{
    cv::perspectiveTransform(*src, *dst, *m);
}
CVAPI(void) core_perspectiveTransform_Mat(cv::Mat *src, cv::Mat *dst, cv::Mat *m)
{
    cv::perspectiveTransform(*src, *dst, *m);
}
CVAPI(void) core_perspectiveTransform_Point2f(cv::Point2f *src, int srcLength, cv::Point2f *dst, int dstLength, cv::_InputArray *m)
{
    std::vector<cv::Point2f> srcVector(src, src + srcLength);
    std::vector<cv::Point2f> dstVector(dst, dst + dstLength);
    cv::perspectiveTransform(srcVector, dstVector, *m);
}
CVAPI(void) core_perspectiveTransform_Point2d(cv::Point2d *src, int srcLength, cv::Point2d *dst, int dstLength, cv::_InputArray *m)
{
    std::vector<cv::Point2d> srcVector(src, src + srcLength);
    std::vector<cv::Point2d> dstVector(dst, dst + dstLength);
    cv::perspectiveTransform(srcVector, dstVector, *m);
}
CVAPI(void) core_perspectiveTransform_Point3f(cv::Point3f *src, int srcLength, cv::Point3f *dst, int dstLength, cv::_InputArray *m)
{
    std::vector<cv::Point3f> srcVector(src, src + srcLength);
    std::vector<cv::Point3f> dstVector(dst, dst + dstLength);
    cv::perspectiveTransform(srcVector, dstVector, *m);
}
CVAPI(void) core_perspectiveTransform_Point3d(cv::Point3d *src, int srcLength, cv::Point3d *dst, int dstLength, cv::_InputArray *m)
{
    std::vector<cv::Point3d> srcVector(src, src + srcLength);
    std::vector<cv::Point3d> dstVector(dst, dst + dstLength);
    cv::perspectiveTransform(srcVector, dstVector, *m);
}

CVAPI(void) core_completeSymm(cv::_InputOutputArray *mtx, int lowerToUpper)
{
    cv::completeSymm(*mtx, lowerToUpper != 0);
}
CVAPI(void) core_setIdentity(cv::_InputOutputArray *mtx, MyCvScalar s)
{
    cv::setIdentity(*mtx, cpp(s));
}
CVAPI(double) core_determinant(cv::_InputArray *mtx)
{
    return cv::determinant(*mtx);
}
CVAPI(MyCvScalar) core_trace(cv::_InputArray *mtx)
{
    return c(cv::trace(*mtx));
}
CVAPI(double) core_invert(cv::_InputArray *src, cv::_OutputArray *dst, int flags)
{
    return cv::invert(*src, *dst, flags);
}


CVAPI(int) core_solve(cv::_InputArray *src1, cv::_InputArray *src2, cv::_OutputArray *dst, int flags)
{
    return cv::solve(*src1, *src2, *dst, flags);
}

CVAPI(int) core_solveLP(cv::Mat *Func, cv::Mat *Constr, cv::Mat *z)
{
	return cv::solveLP(*Func, *Constr, *z);
}

CVAPI(void) core_sort(cv::_InputArray *src, cv::_OutputArray *dst, int flags)
{
    cv::sort(*src, *dst, flags);
}
CVAPI(void) core_sortIdx(cv::_InputArray *src, cv::_OutputArray *dst, int flags)
{
    cv::sortIdx(*src, *dst, flags);
}
CVAPI(int) core_solveCubic(cv::_InputArray *coeffs, cv::_OutputArray *roots)
{
    return cv::solveCubic(*coeffs, *roots);
}
CVAPI(double) core_solvePoly(cv::_InputArray *coeffs, cv::_OutputArray *roots, int maxIters)
{
    return cv::solvePoly(*coeffs, *roots, maxIters);
}

CVAPI(int) core_eigen(cv::_InputArray *src, cv::_OutputArray *eigenvalues,    cv::_OutputArray *eigenvectors)
{
    return cv::eigen(*src, *eigenvalues, *eigenvectors) ? 1 : 0;
}

CVAPI(void) core_calcCovarMatrix_Mat(cv::Mat **samples, int nsamples, cv::Mat *covar, 
    cv::Mat *mean, int flags, int ctype)
{
    std::vector<cv::Mat> samplesVec(nsamples);
    for (int i = 0; i < nsamples; i++)    
        samplesVec[i] = *samples[i];
    
    cv::calcCovarMatrix(&samplesVec[0], nsamples, *covar, *mean, flags, ctype);
}
CVAPI(void) core_calcCovarMatrix_InputArray(cv::_InputArray *samples, cv::_OutputArray *covar, 
    cv::_InputOutputArray *mean, int flags, int ctype)
{
    cv::calcCovarMatrix(*samples, *covar, *mean, flags, ctype);
}


CVAPI(void) core_PCACompute(cv::_InputArray *data, cv::_InputOutputArray *mean,
    cv::_OutputArray *eigenvectors, int maxComponents)
{
    cv::PCACompute(*data, *mean, *eigenvectors, maxComponents);
}
CVAPI(void) core_PCAComputeVar(cv::_InputArray *data, cv::_InputOutputArray *mean,
    cv::_OutputArray *eigenvectors, double retainedVariance)
{
    cv::PCACompute(*data, *mean, *eigenvectors, retainedVariance);
}
CVAPI(void) core_PCAProject(cv::_InputArray *data, cv::_InputArray *mean,
    cv::_InputArray *eigenvectors, cv::_OutputArray *result)
{
    cv::PCAProject(*data, *mean, *eigenvectors, *result);
}
CVAPI(void) core_PCABackProject(cv::_InputArray *data, cv::_InputArray *mean,
    cv::_InputArray *eigenvectors, cv::_OutputArray *result)
{
    cv::PCABackProject(*data, *mean, *eigenvectors, *result);
}

CVAPI(void) core_SVDecomp(cv::_InputArray *src, cv::_OutputArray *w,
    cv::_OutputArray *u, cv::_OutputArray *vt, int flags)
{
    cv::SVDecomp(*src, *w, *u, *vt, flags);
}

CVAPI(void) core_SVBackSubst(cv::_InputArray *w, cv::_InputArray *u, cv::_InputArray *vt,
    cv::_InputArray *rhs, cv::_OutputArray *dst)
{
    cv::SVBackSubst(*w, *u, *vt, *rhs, *dst);
}

CVAPI(double) core_Mahalanobis(cv::_InputArray *v1, cv::_InputArray *v2, cv::_InputArray *icovar)
{
    return cv::Mahalanobis(*v1, *v2, *icovar);
}
CVAPI(void) core_dft(cv::_InputArray *src, cv::_OutputArray *dst, int flags, int nonzeroRows)
{
    cv::dft(*src, *dst, flags, nonzeroRows);
}
CVAPI(void) core_idft(cv::_InputArray *src, cv::_OutputArray *dst, int flags, int nonzeroRows)
{
    cv::idft(*src, *dst, flags, nonzeroRows);
}
CVAPI(void) core_dct(cv::_InputArray *src, cv::_OutputArray *dst, int flags)
{
    cv::dct(*src, *dst, flags); 
}
CVAPI(void) core_idct(cv::_InputArray *src, cv::_OutputArray *dst, int flags)
{
    cv::idct(*src, *dst, flags);
}
CVAPI(void) core_mulSpectrums(cv::_InputArray *a, cv::_InputArray *b, cv::_OutputArray *c, int flags, int conjB)
{
    cv::mulSpectrums(*a, *b, *c, flags, conjB != 0);
}
CVAPI(int) core_getOptimalDFTSize(int vecsize)
{
    return cv::getOptimalDFTSize(vecsize);
}

CVAPI(double) core_kmeans(cv::_InputArray *data, int k, cv::_InputOutputArray *bestLabels,
    MyCvTermCriteria criteria, int attempts, int flags, cv::_OutputArray *centers)
{
    return cv::kmeans(*data, k, *bestLabels, cpp(criteria), attempts, flags, entity(centers));
}

CVAPI(uint64) core_theRNG()
{
    cv::RNG &rng = cv::theRNG();
    return rng.state;
}

CVAPI(void) core_randu_InputArray(cv::_InputOutputArray *dst, cv::_InputArray *low, cv::_InputArray *high)
{
    cv::randu(*dst, *low, *high);
}
CVAPI(void) core_randu_Scalar(cv::_InputOutputArray *dst, MyCvScalar low, MyCvScalar high)
{
    cv::randu(*dst, cpp(low), cpp(high));
}

CVAPI(void) core_randn_InputArray(cv::_InputOutputArray *dst, cv::_InputArray *mean, cv::_InputArray *stddev)
{
    cv::randn(*dst, *mean, *stddev);
}
CVAPI(void) core_randn_Scalar(cv::_InputOutputArray *dst, MyCvScalar mean, MyCvScalar stddev)
{
    cv::randn(*dst, cpp(mean), cpp(stddev));
}

CVAPI(void) core_randShuffle(cv::_InputOutputArray *dst, double iterFactor, uint64 *rng)
{
    cv::RNG rng0;
    cv::randShuffle(*dst, iterFactor, &rng0);
    *rng = rng0.state;
}

#endif