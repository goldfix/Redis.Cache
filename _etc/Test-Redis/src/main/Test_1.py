'''
Created on 30/mar/2014

@author: goldfix
'''
import redis
import uuid
import datetime


class MyClass(object):

    def __init__(self):
        pass
    
    def Test1(self):
        
        p = redis.StrictRedis("192.168.1.51", db=0)
        
        p = p.pipeline()
        
        
        for i in range(0, 1000):
            k = uuid.uuid4()
            v = str( k ) + "---" + datetime.datetime.now().strftime("%Y%m%dT%H%M%SZ%f")
            
            #print  (k, v)
            p.append(k, v)
            print( "K: " + str(i) + "/1000: " + str(k))
        
        for z in range(0, 1000):
            k = uuid.uuid4()
            print( "SET K: " + str(z) + "/1000: " + str(k))
            for i in range(0, 1000):
                v = str( k ) + str(uuid.uuid4()) + "---" + datetime.datetime.now().strftime("%Y%m%dT%H%M%SZ%f")
                p.sadd(k, v)
                pass
        
        
        p.execute()
             
        print( "END" )
        pass
    
    def Test2(self):
        
        r = redis.StrictRedis("192.168.1.51")
        ks = r.keys()
        
        for k in ks:
            if(r.type(k) != b"string") and (r.scard(k)>999):
                print( r.scard(k) )
            #print( k )
            pass
        
        print(len( ks ) )
        
        pass
    
    def Test3(self):
        
        r = redis.StrictRedis("192.168.1.51")
        print (r.info()["redis_version"])
        
        pass
    
x = MyClass().Test1()



