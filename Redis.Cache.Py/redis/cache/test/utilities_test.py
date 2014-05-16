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

from redis.cache import utilities
from redis.cache.utilities import _COMPRESS, _DECOMPRESS, _SERIALIZE,\
    _DESERIALIZE
from unittest.case import TestCase
from redis.cache.errors import NotProvidedError
import datetime


class Test(TestCase):
    
    def __init__(self):
        print "\n"
        pass

    def CompressionTest(self):
        print "CompressionTest :: Init..."
        f = open("txt_test_long.txt", "rb")        
        str_to_compress = f.read()
        f.flush()
        f.close()
        
        

        str_compressed = utilities._Deflate(str_to_compress, _COMPRESS)
        str_decompressed = utilities._Deflate(str_compressed, _DECOMPRESS)
        self.assertTrue((str_to_compress==str_decompressed))
        
        try:
            str_compressed = utilities._Deflate(str_to_compress, _COMPRESS)
            str_decompressed = utilities._Deflate(str_compressed, 3)
            self.assertTrue((str_to_compress==str_decompressed))
        except (NotProvidedError):
            print "CompressionTest :: OK"

    def SerializationTest(self):
        print "SerializationTest :: Init..."
        f = open("txt_test_long.txt", "rb")        
        str_to_serialize = f.read()
        f.flush()
        f.close()
        
        str_serialized = utilities._Serialize(str_to_serialize, _SERIALIZE)
        str_deserialized = utilities._Serialize(str_serialized, _DESERIALIZE)
        self.assertTrue((str_to_serialize == str_deserialized ) )
        
        try:
            str_serialized = utilities._Serialize(str_to_serialize, _SERIALIZE)
            str_deserialized = utilities._Serialize(str_serialized, 5)
            self.assertTrue((str_to_serialize == str_deserialized ) )
        except (NotProvidedError):
            print "SerializationTest :: OK"
        
        pass

    def TTL_Test(self):
        
        ttl_sli =  datetime.timedelta(hours=11, minutes=22, seconds=33)
        ttl_abs =  datetime.timedelta(hours=22, minutes=11, seconds=00)
        
        result = utilities._TTLSerialize(ttl_sli, ttl_abs, datetime.datetime.max)
        print result

        pass
    
    def TTL_TS_DeSerialize_Test(self):
        
        result = utilities._TTL_TS_DeSerialize("20140512T183812|010203|20140512T214115|040506")
        print result
        result = utilities._TTL_DT_DeSerialize("20140512T183812|010203|20140512T214115|040506")
        self.assertTrue((result[0] == datetime.datetime.strptime("20140512T183812", "%Y%m%dT%H%M%S") ) )
        self.assertTrue((result[1] == datetime.datetime.strptime("20140512T214115", "%Y%m%dT%H%M%S") ) )
        print "TTL_TS_DeSerialize_Test : OK"
                
        pass


    def TTL_Is_Expired_Test(self):
        
        result = utilities._TTL_Is_Expired("20150512T183812|010203|20150512T214115|040506")
        self.assertTrue(result ==False)
        
        result = utilities._TTL_Is_Expired("20130512T183812|010203|20150512T214115|040506")
        self.assertTrue(result ==True)
        
        print "TTL_Is_Expired_Test : OK"
                
        pass


# if __name__ == "__main__":
#     import sys;sys.argv = ['Test.CompressionTest']
#     unittest.main()
    
# p = Test().CompressionTest()
# p = Test().SerializationTest()
# p = Test().TTL_Test()
# p = Test().TTL_TS_DeSerialize_Test()
# p = Test().TTL_Is_Expired_Test()



