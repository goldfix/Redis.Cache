# -*- coding: utf-8 -*- 
'''
Copyright (c) 2014, pietro partescano
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the Redis.Cache.Py nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
'''

import redis
import uuid
import datetime

from unittest.case import TestCase
import zlib
import pickle
from redis.cache import utilities
from redis.cache.utilities import _SERIALIZE, _DESERIALIZE

class MyUnitTest(TestCase):

    def __init__(self):
        pass
    
    def LoadData_Test_4(self):
        p = redis.StrictRedis("127.0.0.1", db=0)
        
        test_dic = dict()
        test_dic["A"] = "V_A"
        test_dic["B"] = "V_B"
        test_dic["C"] = "V_C"
        print test_dic 
        
        test_dic_ser = utilities._Serialize(test_dic, _SERIALIZE)
        p.set("DIC", test_dic_ser)
        test_dic_result = p.get("DIC")
        
        test_dic_result = utilities._Serialize(test_dic_result, _DESERIALIZE)
        self.assertDictEqual(test_dic, test_dic_result)
        
        
        t = "{'A': 'V_A', 'C': 'V_C', 'B': 'V_B'}"
        print type(t)
        t = eval(t)
        print type(t)
        pass
    
    def LoadData_Test_1(self):
        
        p = redis.StrictRedis("127.0.0.1", db=0)
        
        p = p.pipeline()
        
        
        for i in range(0, 10):
            k = uuid.uuid4()
            v = str( k ) + "---" + datetime.datetime.now().strftime("%Y%m%dT%H%M%SZ%f")
            
            #print  (k, v)
            p.append(k, v)
            print( "K: " + str(i) + "/1000: " + str(k))
        
        for z in range(0, 10):
            k = uuid.uuid4()
            print( "SET K: " + str(z) + "/1000: " + str(k))
            for i in range(0, 10):
                v = str( k ) + str(uuid.uuid4()) + "---" + datetime.datetime.now().strftime("%Y%m%dT%H%M%SZ%f")
                p.sadd(k, v)
                pass
        
        
        p.execute()
             
        print( "END" )
        pass
    
    def LoadData_Test_2(self):
        
        r = redis.StrictRedis("127.0.0.1", db=0)
        ks = r.keys()
        
        for k in ks:
            if(r.type(k) != b"string") and (r.scard(k)>2):
                print( r.scard(k) )
            #print( k )
            pass
        
        print(len( ks ) )
        
        pass
    
    def LoadData_Test_3(self):
        
        r = redis.StrictRedis("127.0.0.1")
        print (r.info()["redis_version"])
        
        pass
    

    def CSharpTest(self):
        """
        Conversion test function from c#
        """    
        
        r = redis.StrictRedis("127.0.0.1")

        result = r.lrange("k_string", 0, 1)
        self.assertTrue(result[1] == "Test 1234567890 Test 0987654321 Test 1234567890 Test 0987654321 Test 1234567890 Test 0987654321 メンズア")
        print "assertTrue :: k_string :: OK"
        
        result = r.lrange("k_int16", 0, 1)
        self.assertTrue(int( result[1]) == 12345)
        print "assertTrue :: k_int16 :: OK"        
        
        result = r.lrange("k_double", 0, 1)
        self.assertTrue( float( result[1]) == 12345.06789)
        print "assertTrue :: k_double :: OK"        
        
        result = r.lrange("k_byte[]", 0, 1)
        tmp = result[1].decode("utf-8") 
        self.assertTrue(result[1] == "Test 1234567890 Test 0987654321 Test 1234567890 Test 0987654321 Test 1234567890 Test 0987654321 メンズア")
        print "assertTrue :: k_byte[] :: OK"
        
        pass
    

    
    def DeflateTest(self):

        f = open("txt_test_long.txt", "rb")        
        str_to_compress = f.read()
        f.flush()
        f.close()
        
#         str_to_compress = "Test 1234567890 Test 0987654321 Test 1234567890 Test 0987654321 Test 1234567890 Test 0987654321 メンズア"
        print len(str_to_compress)
        cmpss = zlib.compressobj(6,zlib.DEFLATED,-zlib.MAX_WBITS)
        bytes_to_compressed = cmpss.compress(str_to_compress)
        bytes_to_compressed += cmpss.flush()
        print len(bytes_to_compressed)
        
        f = file("/tmp/out_p.bin", "wb")
        f.write(bytes_to_compressed)
        f.flush()
        f.close()
        
        decmpss = zlib.decompressobj(-zlib.MAX_WBITS)
        bytes_to_decompressed = decmpss.decompress(bytes_to_compressed) 
        bytes_to_decompressed += decmpss.flush()
        self.assertTrue(( bytes_to_decompressed==str_to_compress ))
        
        
        print "test list..."        #----------------------------------------------------------------------------------------------------------------
        
        test_list = list()
        for i in range(10):
            test_list.append("Test 1234567890 Test 0987654321 Test 1234567890 Test 0987654321 Test 1234567890 Test 0987654321 メンズア -- " + str(i))
#             print i
        
        
        print "test list serialization  ..."
        test_list_serz = pickle.dumps(test_list, pickle.HIGHEST_PROTOCOL)
        test_list_de_serz = pickle.loads(test_list_serz)
        self.assertListEqual(test_list, test_list_de_serz)
        print "test list serialization : OK"
        
        print "test list compression  ..."
        print len(test_list_serz)
        cmpss = zlib.compressobj()
        test_list_comp = cmpss.compress(test_list_serz)
        test_list_comp += cmpss.flush()
        print len(test_list_comp)

        
        decmpss = zlib.decompressobj()
        test_list_decomp = decmpss.decompress(test_list_comp)
        test_list_decomp += decmpss.flush()
        
        test_list_decomp = pickle.loads(test_list_decomp)
        self.assertListEqual(test_list, test_list_decomp)
        print "test list compression : OK"
        
        pass


    def DecodeDataFrom_C_Test(self):
        print "Load data from data c# file..."
        f = open("/tmp/out_c.bin", "rb")        
        bytes_to_compressed = f.read()
        f.flush()
        f.close()
        print len(bytes_to_compressed)
        
        print "Load data from txt file..."
        f = open("txt_test_long.txt", "rb")        
        str_to_decompress = f.read()
        f.flush()
        f.close()
        
        print "Decompress data c# file..."
        decmpss = zlib.decompressobj(-zlib.MAX_WBITS)
        bytes_to_decompressed = decmpss.decompress(bytes_to_compressed) 
        bytes_to_decompressed += decmpss.flush()
        print len(bytes_to_decompressed)
        
        self.assertTrue(( bytes_to_decompressed==str_to_decompress ))
        print "Check file: OK"
        
        pass


# x = MyUnitTest().LoadData_Test_1()
x = MyUnitTest().LoadData_Test_4()
# x = MyUnitTest().CSharpTest()
# x = MyUnitTest().DeflateTest()
# x = MyUnitTest().DecodeDataFrom_C_Test()


# z = datetime.datetime.utcnow()
# zz = datetime.timedelta(hours=2, minutes=22, seconds=13)
# print str( zz).split(":")[0].zfill(2) , str( zz).split(":")[1].zfill(2), str( zz).split(":")[2].zfill(2)


# z = str(3).zfill(2)
# print z



