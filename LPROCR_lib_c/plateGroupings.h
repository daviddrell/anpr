

#define MAX_ALTERNATE_READINGS 4   // one plate can be read several different ways over multiple frames

bool PlateGroupCallBackRegistered;

void (*PlateGroupCallBack)(int chan, char *contatonatedPlateStrings, int serialNumber, int len) ;

struct STRING
{
	int count;
	char str[MAX_CANDIDATE_CHARS];
};


struct GROUP
{
	bool hasAString;
	int AltReadingIndex;
	int SerialNum;
	STRING AlternateReading [MAX_ALTERNATE_READINGS ];	
};

struct GROUP_CONTROL
{
	int CurrentNewGroup;
	GROUP Groups[ MAX_ACTUAL_PLATES ];

} ;

struct GROUP_BY_CHANNEL
{
	bool lastFrameWasEmpty;
	GROUP_CONTROL gc;
};

void ClearString(char * s, int count);
void ClearGroup (GROUP * group );
void Groups_Init(int chan);
void AddToNewGroup (int chan, char * currentReading, int charCount, int serialNumber );
//void CopyString(char * s, char * d, int count);
int GetNewGroupIndex (int chan );
int GetNextAltReadingIndex (int chan, int groupIndex);
void CopyString(char * s, char * d, int count);
int ConCat ( char * outstring, int outMaxLen, int offset, char * inStr, int inStrLen);

