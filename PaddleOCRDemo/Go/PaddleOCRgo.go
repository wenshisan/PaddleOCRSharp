package main

import (
    "fmt"
    "syscall"
    "unsafe"
    "os"
    "bufio"
    "C"
	"path"
	"runtime"
)

// 获取字符串的长度指针
func lenPtr(s string) uintptr {
    return uintptr(len(s))
}

// 获取数字的指针
func intPtr(n int) uintptr {
    return uintptr(n)
}

// 获取字符串的指针
func strPtr(s string) uintptr {
    return uintptr(unsafe.Pointer(syscall.StringBytePtr(s)))
}

func getCurrentAbPathByCaller() string {
	var abPath string
	_, filename, _, ok := runtime.Caller(0)
	if ok {
		abPath = path.Dir(filename)
	}
	return abPath
}

func main() {  
   
 dll,_:= syscall.LoadDLL("PaddleOCR.dll")
 Initjson,_:=dll.FindProc("Initializejson")
 detect,_:=dll.FindProc("Detect")
 enableANSI,_:=dll.FindProc("EnableANSIResult")
root:=getCurrentAbPathByCaller();
Initjson.Call(strPtr(root+"\\inference\\ch_PP-OCRv3_det_infer"),
strPtr(root+"\\inference\\ch_ppocr_mobile_v2.0_cls_infer"),
strPtr(root+"\\inference\\ch_PP-OCRv3_rec_infer"),
strPtr(root+"\\inference\\ppocr_keys.txt"),strPtr("{}"))

//启用单字节编码返回json串（默认为Unicode，有空格，go不能显示全）
enableANSI.Call(1)//0，unicode编码，1，ANSI编码

 res, _, _:=detect.Call(strPtr(root+"\\image\\11.jpg"))  
  p_result := (*C.char)(unsafe.Pointer(res))
 ocrresult:= C.GoString(p_result)
 fmt.Println(ocrresult)

 input := bufio.NewScanner(os.Stdin)
 input.Scan() 




}