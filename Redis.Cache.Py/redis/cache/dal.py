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
from redis.cache import config


class RedisDal(object):
    """
        
    """
    
    def __init__(self):
        self._db = redis.StrictRedis(host=config.RedisDatabase, port=config.RedisConnectionStringPort, db=config.RedisDatabase)
        pass

    def SetTTL(self, key, ttl):
        if( key is None or str(key).strip() == ""):
            raise errors.ArgumentError("Parameter is invalid (key)")

        try:
            result = self._db.expire(key, ttl)
            return result 
        except (Exception):
            raise
        finally:
            pass

        pass

    def DeleteTTL(self, key):
        if( key is None or str(key).strip() == ""):
            raise errors.ArgumentError("Parameter is invalid (key)")

        try:
            result = self._db.persist(key)
            return result 
        except (Exception):
            raise
        finally:
            pass

        pass


    def ItemDelete(self, key):
        if( key is None or str(key).strip() == ""):
            raise errors.ArgumentError("Parameter is invalid (key)")

        try:
            result = self._db.delete(key)
            return result 
        except (Exception):
            raise
        finally:
            pass

        pass

    def ItemExist(self, key):
        if( key is None or str(key).strip() == ""):
            raise errors.ArgumentError("Parameter is invalid (key)")

        try:
            result = self._db.exists(key)
            return result 
        except (Exception):
            raise
        finally:
            pass

        pass

    def AddListItem(self, key, value):
        if( key is None or str(key).strip() == "" or value is None or str(value).strip() == ""):
            raise errors.ArgumentError("Parameter is invalid (key or value)")

        try:
            result = self._db.rpush(key, value)
            return result 
        except (Exception):
            raise
        finally:
            pass

        pass

    def UpdateTTL_ListItem(self, key, ttl):
        if( key is None or str(key).strip() == "" or ttl is None or str(ttl).strip() == ""):
            raise errors.ArgumentError("Parameter is invalid (key or value)")

        try:
            result = self._db.lset(key, 0, ttl)
            return result 
        except (Exception):
            raise
        finally:
            pass

        pass

    def AddListItem(self, key, value, value_ttl):
        if( key is None or str(key).strip() == "" 
            or value is None or str(value).strip() == ""
            or value_ttl is None or str(value_ttl).strip() == ""
            ):
            raise errors.ArgumentError("Parameter is invalid (key or value or value_ttl)")

        try:
            v = (value_ttl, value)
            result = self._db.rpush(key, v)
            return result 
        except (Exception):
            raise
        finally:
            pass

        pass


    def GetListItem(self, key, type):
        if( key is None or str(key).strip() == "" ):
            raise errors.ArgumentError("Parameter is invalid (key)")

        try:
            val = self._db.lrange(key, 0, 1)
            result = (va[0], val[1])
            return result 
        except (Exception):
            raise
        finally:
            pass

        pass

