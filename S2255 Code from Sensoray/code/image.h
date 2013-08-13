#ifndef IMAGE_H
#define IMAGE_H

int save_image_uncompressed(const unsigned char *image, TCHAR *szFilename, int height, int width, int stride,  int type);
int save_image_uncompressed_mono(const unsigned char *image, TCHAR *szFilename, int height, int width);

#endif