import os
import ctypes
import Parameter
from ctypes import *
import json
from datetime import datetime
import numpy as np
paddleOCR=cdll.LoadLibrary(".\PaddleOCR.dll")
encode="gbk" 

root="./"
cls_infer =root+"/inference/ch_ppocr_mobile_v2.0_cls_infer"
rec_infer = root+"/inference/ch_PP-OCRv3_rec_infer"
det_infer = root+"/inference/ch_PP-OCRv3_det_infer"
ocrkeys = root+"/inference/ppocr_keys.txt"
parameter=Parameter.Parameter()
p_cls_infer=cls_infer.encode(encode)
p_rec_infer=rec_infer.encode(encode)
p_det_infer=det_infer.encode(encode)
p_ocrkeys=ocrkeys.encode(encode)

def main():
    parameterjson= json.dumps(parameter,default=Parameter.Parameter2dict)
    paddleOCR.Initializejson( p_det_infer,  p_cls_infer,  p_rec_infer,  p_ocrkeys, parameterjson.encode(encode))
    result=""
    paddleOCR.Detect.restype = ctypes.c_wchar_p
    imagepath=os.path.abspath('.')+"\\image\\"
    imagefiles=os.listdir(imagepath)
    total=[]
    for image in imagefiles:
       imagefile=imagepath+image
       t1= datetime.utcnow()
       result= paddleOCR.Detect(imagefile.encode(encode))
       t2=datetime.utcnow()
       c=t2-t1
       total.append(c)
       print("time:",c)
       print(result)
    print("平均时间:",   np.mean(total))
if __name__=="__main__":
    main()
    input() 
   
