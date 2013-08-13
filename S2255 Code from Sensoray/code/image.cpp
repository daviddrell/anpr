#include "stdafx.h"
/** saves an image to bmp file
 *  @param filename is the full file path to save( minus the extension)
 *  @param *image, pointer to image data in Y Cr Cb format
 *  @param type is the type( 0-- BMP,  1--ppm)
 *  @return 0 on success, -1 if file invalid
*/
int save_image_uncompressed(const unsigned char *image, TCHAR *szFilename, int height, int width, int stride,  int type)
{
    char ppmheader[256];
    TCHAR name[MAX_PATH];
    FILE *fptr;
    int val;
    int rlen; // row length or number of columns( unsigned chars== pixels * unsigned chars per pixel)
    int clen; // column length or number of rows
    int bpp; // unsigned chars per pixel! not bits
    int i;
     
    rlen = stride;
    clen = height;
    bpp = stride / width;
    
    // open the file
    _tcscpy(name, szFilename);
    
    fptr = _tfopen( name, _T("wb"));
    
    if (fptr == NULL)
        return -1;

    if (type == 1)
    {
        sprintf(ppmheader, "P6\n#ppm image\n%d %d\n255\n", width, height);
        fwrite(ppmheader, 1, strlen(ppmheader), fptr);
        // write out the rows
        for ( i=0; i< clen; i++)
        {
            fwrite( &image[rlen*i], 1, rlen, fptr);
        }
    }
    else
    {
        val = 0x4d42;                   fwrite(&val,1,2,fptr); //bmp signature
        val = (rlen * clen / bpp) + 54; fwrite(&val,1,4,fptr); //size of bmp file
        val = 0;                        fwrite(&val,1,2,fptr); //must be 0
        val = 0;                        fwrite(&val,1,2,fptr); //must be 0
        val = 54;                       fwrite(&val,1,4,fptr); //offset to image start
        val = 40;                       fwrite(&val,1,4,fptr); //must be 40
        val = rlen / bpp;               fwrite(&val,1,4,fptr); //image width in pixels
        val = clen ;                    fwrite(&val,1,4,fptr); //image height in pixels
        val = 1;                        fwrite(&val,1,2,fptr); //must be 1
        val = bpp * 8;                  fwrite(&val,1,2,fptr); //bits per pixel
        val = 0;                        fwrite(&val,1,4,fptr); //compression (none)
        val = (rlen * clen / bpp);      fwrite(&val,1,4,fptr); //size of image data
        val = 2835;                     fwrite(&val,1,4,fptr); //horizontal pixels/m
        val = 2835;                     fwrite(&val,1,4,fptr); //vertical pixels/m
        val = 0;                        fwrite(&val,1,4,fptr); //colors in image, or 0
        val = 0;                        fwrite(&val,1,4,fptr); //important colors,or 0
        for (i= 0; i<clen; i++) {
            fwrite( &image[i*rlen], 1, rlen, fptr);
        }
    }
 
    fclose( fptr);
    return 0;

}


/** saves an image to bmp file
 *  @param filename is the full file path to save( minus the extension)
 *  @param *image, pointer to image data in Y Cr Cb format
 *  @param type is the type( 0-- BMP,  1--ppm)
 *  @return 0 on success, -1 if file invalid
*/
int save_image_uncompressed_mono(const unsigned char *image, TCHAR *szFilename, int height, int width)
{
    TCHAR name[MAX_PATH];
    FILE *fptr;
    int val;
    int rlen; // row length or number of columns( unsigned chars== pixels * unsigned chars per pixel)
    int clen; // column length or number of rows
    int bpp; // unsigned chars per pixel! not bits
    int i;
     
    rlen = width;
    clen = height;
    bpp = 1;
   
    // open the file
    _tcscpy(name, szFilename);
    
    fptr = _tfopen( name, _T("wb"));
    
    if (fptr == NULL)
        return -1;

	val = 0x4d42;                   fwrite(&val,1,2,fptr); //bmp signature
	val = (rlen * clen / bpp) +256*4 + 54; fwrite(&val,1,4,fptr); //size of bmp file
	val = 0;                        fwrite(&val,1,2,fptr); //must be 0
	val = 0;                        fwrite(&val,1,2,fptr); //must be 0
	val = 54+256*4;                 fwrite(&val,1,4,fptr); //offset to image start
	val = 40;                       fwrite(&val,1,4,fptr); //must be 40
	val = rlen / bpp;               fwrite(&val,1,4,fptr); //image width in pixels
	val = clen ;                    fwrite(&val,1,4,fptr); //image height in pixels
	val = 1;                        fwrite(&val,1,2,fptr); //must be 1
	val = 8;	                 fwrite(&val,1,2,fptr); //bits per pixel
	val = 0;                        fwrite(&val,1,4,fptr); //compression (none)
	val = 0;					fwrite(&val,1,4,fptr); //size of image data
	val = 0;                     fwrite(&val,1,4,fptr); //horizontal pixels/m
	val = 0;                     fwrite(&val,1,4,fptr); //vertical pixels/m
	val = 256;                       fwrite(&val,1,4,fptr); //colors in image, or 0
	val = 0;                        fwrite(&val,1,4,fptr); //important colors,or 0
	/*create grey scale color table */
	for (i = 0; i < 256; i++) {
		val = i; fwrite(&val,1,1,fptr); 
		val = i; fwrite(&val,1,1,fptr); 
		val = i; fwrite(&val,1,1,fptr); 
		val = 0; fwrite(&val,1,1,fptr); 
	}

	for (i= (clen-1); i>=0; i--) {
		fwrite( &image[i*rlen], 1, rlen, fptr);
	}
 
    fclose( fptr);
    return 0;

}
